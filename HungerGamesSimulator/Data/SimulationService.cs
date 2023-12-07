namespace HungerGamesSimulator.Data
{
    public class SimulationService
    {
        private Simulation _simulation;
        private IMessageCenter _messageCenter;
        private CombatService _combatService;
        private PartyService _partyService;
        private MovementService _movementService;
        private PartyFinder _partyFinder;
        private EventService _eventService;

        private int actionsToTake;

        public SimulationService( Simulation simulation )
        {
            _simulation = simulation;
            _combatService = new CombatService();
            _movementService = new MovementService();
            _partyFinder = new PartyFinder();
            _messageCenter = new MessageCenter();
            _eventService = new EventService(_simulation, _messageCenter);
            _partyService = new PartyService( _combatService, _eventService );
        }

        /// <summary>
        /// Acts a single day in the hunger games
        /// </summary>
        public void Act()
        {
            ClearMessageCenter();

            var aliveActors = _simulation.GetAliveActors();
            if (aliveActors == null || !aliveActors.Any())
            {
                _messageCenter.AddMessage("There are not any alive actors to act with.");
                return;
            }
            _messageCenter.AddMessage( $"<h2>Day {_simulation.Day}</h2>" );

            // number of actions tributes can take reduces if there are events
            actionsToTake = _simulation.ActionsPerDay - DecideEvents();

            if (actionsToTake <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"Day {_simulation.Day}");
                foreach (var actor in aliveActors)
                {
                    System.Diagnostics.Debug.WriteLine($"Actor with name: {actor.Name} has party: {actor.PartyId} and health: {actor.Health} with location: {actor.Location}");
                }
            }

            var partiesWent = new HashSet<Guid>();
            for (int i = 0; i < actionsToTake; i++)
            {
                _messageCenter.AddMessage($"Part {i + 1}/{actionsToTake} of Day {_simulation.Day}");

                foreach (var actor in aliveActors)
                {
                    if ((actor.IsInParty() && !partiesWent.Contains(actor.PartyId)) || !actor.IsInParty())
                    {
                        Act(actor);
                    }

                    if (actor.IsInParty())
                    {
                        partiesWent.Add(actor.PartyId);
                    }
                }

                // diagnostic data for debugging
                System.Diagnostics.Debug.WriteLine($"Day {_simulation.Day}");
                foreach (var actor in aliveActors)
                {
                    System.Diagnostics.Debug.WriteLine($"Actor with name: {actor.Name} has party: {actor.PartyId} and health: {actor.Health} with location: {actor.Location}");
                }

                // get next state of alive actors
                aliveActors = _simulation.GetAliveActors();
                if (aliveActors == null || !aliveActors.Any())
                {
                    _messageCenter.AddMessage("There are not any alive actors to act with.");
                    break;
                }

                partiesWent.Clear();
            }

            // increase simulation day
            _simulation.IncreaseDay();

        }

        /// <summary>
        /// Handles the different actions the actor could have
        /// </summary>
        /// <param name="actor"></param>
        private void Act( IActor actor )
        {
            var state = actor.GetNextAction( _simulation.GetSimulationSnapshot() );
            switch ( state )
            {
                case ActorAction.Attacking:
                CombatRequest( actor );
                break;
                case ActorAction.Moving:
                MovementRequest( actor );
                break;
                case ActorAction.JoinParty:
                PartyRequest( actor, PartyRequestType.Join );
                break;
                case ActorAction.LeaveParty:
                PartyRequest( actor, PartyRequestType.Leave );
                break;
                case ActorAction.Dead:
                break;
                default:
                _messageCenter.AddMessage( $"There is no way for the simulation to handle state ({state}) for {actor.Name}" );
                break;
            }
        }

        /// <summary>
        /// Leave the current party the actor is a part of
        /// </summary>
        /// <param name="actor"></param>
        private void PartyRequest( IActor actor, PartyRequestType partyRequestType )
        {
            PartyResponse? response;

            switch ( partyRequestType )
            {
                case PartyRequestType.Join:
                {
                    IActor? otherActor = GetRandomActor( actor );
                    if ( otherActor == null )
                    {
                        // joining a party cannot be processed without a tribute
                        var concatedPartyNames = SimulationUtils.GetConcatenatedActorNames( GetParty( actor ) );
                        _messageCenter.AddMessage( $"{concatedPartyNames} searched for tributes to join their party but couldn't find anyone" );
                        return;
                    }

                    var actorsParty = GetParty( actor );
                    var otherActorsParty = GetParty( otherActor );
                    var request = new PartyRequest( partyRequestType, actor, actorsParty, otherActorsParty );
                    response = _partyService.HandlePartyRequest( request );
                    break;
                }


                case PartyRequestType.Leave:
                {
                    var actorsParty = GetParty( actor );
                    var request = new PartyRequest( partyRequestType, actor, actorsParty );
                    response = _partyService.HandlePartyRequest( request );
                    break;
                }

                default:
                response = null;
                break;
            }

            if ( response == null )
            {
                return;
            }

            _messageCenter.AddMessage( response.Message );
        }

        /// <summary>
        /// Initializes the data to request combat 
        /// </summary>
        /// <param name="actor"></param>
        private void CombatRequest( IActor actor )
        {
            IActor? otherActor = GetRandomActor( actor );
            if ( otherActor == null )
            {

                // combat cannot be processed without a tribute
                var concatedPartyNames = SimulationUtils.GetConcatenatedActorNames( GetParty( actor ) );
                _messageCenter.AddMessage( $"{concatedPartyNames} searched for a tribute to attack, but couldn't find anyone around them" );
                return;
            }

            // retrieve other party members
            var fighters = GetParty( actor );
            var defenders = GetParty( otherActor );

            // create the request and simulate combat
            var request = new CombatRequest( fighters, defenders );
            var response = _combatService.Simulate( request );

            GenerateCombatDescriptions( response, fighters, defenders );
        }

        /// <summary>
        /// Creates the dynamic messages that can result from the summary of the combat experience
        /// </summary>
        /// <param name="combatResponse"></param>
        /// <param name="fighters"></param>
        /// <param name="defenders"></param>
        private void GenerateCombatDescriptions( CombatResponse combatResponse, List<IActor> fighters, List<IActor> defenders )
        {
            // manage the response 
            var concatedFighterNames = SimulationUtils.GetConcatenatedActorNames( fighters );
            var concatedDefenderNames = SimulationUtils.GetConcatenatedActorNames( defenders );

            if ( combatResponse.DefendersDied )
            {
                var deadDefenders = defenders.Where( defender => defender.IsDead() );
                var deadDefenderNames = SimulationUtils.GetConcatenatedActorNames( deadDefenders.ToList() );

                _messageCenter.AddMessage( $"{concatedFighterNames} attacked {concatedDefenderNames} and killed {deadDefenderNames}" );
                foreach ( var deadActor in deadDefenders )
                {
                    _messageCenter.AddCannonMessage( deadActor );
                }
            }
            else if ( combatResponse.Escaped )
            {
                _messageCenter.AddMessage( $"{concatedFighterNames} attacked {concatedDefenderNames}. {concatedDefenderNames} barely escaped" );
            }
        }

        /// <summary>
        /// Request to move an actor and party in the simulation
        /// </summary>
        /// <param name="actor"></param>
        private void MovementRequest( IActor actor )
        {
            // retrieve other party members
            var party = GetParty( actor );

            MovementResponse response = _movementService.Move( new MovementRequest( party, _simulation.GetSimulationSnapshot() ) );

            var partyNames = SimulationUtils.GetConcatenatedActorNames( party );

            _messageCenter.AddMessage( $"{partyNames} moved from {response.PastLocation} to {response.NewLocation}" );
        }

        /// <summary>
        /// Decide if there will be an event for today
        /// </summary>
        /// <returns>the number of actions the event took, if there was one</returns>
        private int DecideEvents()
        {
            if (ShouldSuddenDeathEventTrigger())
            {
                return _eventService.HandleEventCreation(EventName.SuddenDeath);
            }

            if (_simulation.Day == 1)
            {
                return _eventService.HandleEventCreation(EventName.Cornucopia);
            }

            if (_simulation.Day % 3 == 0)
            {
                return _eventService.HandleEventCreation(EventName.Burn);
            }

            return 0;
        }

        /// <summary>
        /// Get the party of an actor
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        private List<IActor> GetParty( IActor actor )
        {
            _partyFinder.PartyToFind = actor.PartyId;
            var party = _simulation.GetAliveActors( _partyFinder.FindParty, actor ) ?? new List<IActor>();
            party.Add( actor );
            return party;
        }

        /// <summary>
        /// Get a random actor using the current actor. 
        /// </summary>
        /// <param name="actor"></param>
        /// <returns>Returns null if there are no actors in the area, otherwise a randome actor in the area</returns>
        private IActor? GetRandomActor( IActor actor )
        {
            if ( actor.IsInParty() )
            {
                _partyFinder.PartyToAvoid = actor.PartyId;
                return GetRandomActorInArea( actor.Location, actor, _partyFinder.AvoidParty );
            }
            return GetRandomActorInArea( actor.Location, actor );
        }

        private IActor? GetRandomActorInArea( Coord origin, IActor? toIgnore = null, Predicate<IActor>? predicate = null )
        {
            return _simulation.GetRandomActorInArea( origin, toIgnore, predicate );
        }

        private bool ShouldSuddenDeathEventTrigger()
        {
           if (_simulation.Width <= 1 && _simulation.Height <= 1)
           {
              return true;
           }
           var aliveActors = _simulation.GetActors( actor => actor.Health >= 1 );
           if ( aliveActors.Count == 2 )
           {
              return true;
           }

           var actorsInParty = GetParty( aliveActors.First() );
           if ( aliveActors.Count == actorsInParty.Count )
           { 
              return true;
           }

           return false;
        }

        public bool IsSimulationFinished()
        {
            if ( _simulation.GetActors( actor => actor.Health >= 1 ).Count() == 1 )
            {
                return true;
            }
            return false;
        }

        public void ClearMessageCenter()
        {
            _messageCenter.ClearCannonMessages();
            _messageCenter.ClearMessages();
        }

        public List<string> GetMessages()
        {
            return _messageCenter.GetMessages();
        }

        public List<string> GetCannonMessages()
        {
            return _messageCenter.GetCannonMessages();
        }

        public int GetCannonShotAmounts()
        {
            return _messageCenter.GetCannonMessagesCount();
        }

        public IActor GetWinner()
        {
           return _simulation.GetActors( actor => actor.Health >= 1 ).First();
        }
    }
}
