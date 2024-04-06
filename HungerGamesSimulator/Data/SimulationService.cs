using HungerGamesSimulator.MessageCenter;

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
        private MemoryService _memoryService;

        public SimulationService(Simulation simulation, IMessageCenter messageCenter )
        {
            _simulation = simulation;
            _memoryService = new MemoryService();
            _combatService = new CombatService(_memoryService);
            _movementService = new MovementService();
            _partyFinder = new PartyFinder();
            _messageCenter = messageCenter;
            _eventService = new EventService(_simulation, _messageCenter, _memoryService);
            _partyService = new PartyService(_combatService, _eventService, _memoryService);
        }

        /// <summary>
        /// Acts a single day in the hunger games
        /// </summary>
        public void Act()
        {
            var aliveActors = _simulation.GetAliveActors();
            if (aliveActors == null || !aliveActors.Any())
            {
                _messageCenter.AddMessage("There are not any alive actors to act with.");
                return;
            }

            _messageCenter.AddMessage($"<h2>Day {_simulation.Day}</h2>");

            // Number of actions tributes can take reduces if there are events
            int eventActions = _eventService.RunEventsIfNeeded();
            int actionsToTake = _simulation.ActionsPerDay - eventActions;

            // Debug information
            if (actionsToTake <= 0)
            {
                DisplayDebugInformation(aliveActors);
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

                DisplayDebugInformation(aliveActors);

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
        private void Act(IActor actor)
        {
            var state = actor.GetNextAction(_simulation.GetSimulationSnapshot());
            switch (state)
            {
                case ActorAction.Attacking:
                    CombatRequest(actor);
                    break;
                case ActorAction.Moving:
                    MovementRequest(actor);
                    break;
                case ActorAction.JoinParty:
                    PartyRequest(actor, PartyRequestType.Join);
                    break;
                case ActorAction.LeaveParty:
                    PartyRequest(actor, PartyRequestType.Leave);
                    break;
                case ActorAction.Dead:
                    break;
                default:
                    _messageCenter.AddMessage($"There is no way for the simulation to handle state ({state}) for {actor.Name}");
                    break;
            }
        }

        /// <summary>
        /// Leave the current party the actor is a part of
        /// </summary>
        /// <param name="actor"></param>
        private void PartyRequest(IActor actor, PartyRequestType partyRequestType)
        {
            var builder = _simulation.GameStringFactory.CreateStringBuilder();

            switch (partyRequestType)
            {
                case PartyRequestType.Join:
                    {
                        IActor? otherActor = GetRandomActor(actor);
                        if (otherActor == null)
                        {
                            // joining a party cannot be processed without a tribute
                            builder.QueueInformation(new ContextType[] { ContextType.PartySearchFail },  new BuilderObject( actor.IsInParty() ? GetParty(actor) : actor  ) );
                            _messageCenter.AddMessage(builder.ToString());
                            break;
                        }

                        if (_memoryService.GetLastMemeory(actor.ActorId, otherActor.ActorId) == MemoryType.Bad)
                        {
                            ProcessCombat(actor, otherActor, builder);
                            break;
                        }

                        var actorsParty = GetParty(actor);
                        var otherActorsParty = GetParty(otherActor);
                        var request = new JoinPartyRequest(actor, otherActor, actorsParty, otherActorsParty, builder);
                        _partyService.JoinParty(request);
                        break;
                    }
                case PartyRequestType.Leave:
                    {
                        var actorsParty = GetParty(actor);
                        var request = new LeavePartyRequest(actor, actorsParty, builder);
                        _partyService.LeaveParty(request);
                        break;
                    }
            }

            _messageCenter.AddMessage(builder.ToString());
        }

        /// <summary>
        /// Initializes the data to request combat 
        /// </summary>
        /// <param name="actor"></param>
        private void CombatRequest(IActor actor)
        {
            var builder = _simulation.GameStringFactory.CreateStringBuilder();
            IActor? otherActor = GetRandomActor(actor);
            if (otherActor == null)
            {
                // combat cannot be processed without a tribute
                builder.QueueInformation(new ContextType[] { ContextType.CombatSearchFail }, new BuilderObject(actor.IsInParty() ? GetParty(actor) : actor));
                _messageCenter.AddMessage(builder.ToString());
                return;
            }

            builder.QueueInformation(new ContextType[] { ContextType.Combat, ContextType.Flavor }, new BuilderObject( actor.IsInParty() ? GetParty(actor) : actor, ContextType.Attacker) , new BuilderObject( otherActor.IsInParty() ? GetParty(otherActor) : otherActor, ContextType.Defender) );

            ProcessCombat( actor, otherActor, builder );
        }

        private void ProcessCombat(IActor actor, IActor otherActor, GameStringBuilder builder )
        {
            // retrieve other party members
            var fighters = GetParty(actor);
            var defenders = GetParty(otherActor);

            // create the request and simulate combat
            var request = new CombatRequest(fighters, defenders);
            _combatService.Simulate(request, builder, _messageCenter);
        }

        /// <summary>
        /// Request to move an actor and party in the simulation
        /// </summary>
        /// <param name="actor"></param>
        private void MovementRequest(IActor actor)
        {
            var builder = _simulation.GameStringFactory.CreateStringBuilder();
            builder.QueueInformation(new ContextType[] { ContextType.Move }, new BuilderObject( actor.IsInParty() ? GetParty(actor) : actor ) );
            _messageCenter.AddMessage(builder.ToString());

            var party = GetParty(actor);
            _movementService.Simulate(new MovementRequest(party, _simulation.GetSimulationSnapshot()));
        }

        /// <summary>
        /// Get the party of an actor
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        private Party GetParty(IActor actor)
        {
            return _simulation.GetParty(actor);
        }

        /// <summary>
        /// Get a random actor, ignoring the given actor and its party. 
        /// </summary>
        /// <param name="toIgnore"></param>
        /// <returns>Returns null if there are no actors in the area, otherwise a randome actor in the area</returns>
        private IActor? GetRandomActor(IActor toIgnore)
        {
            if (toIgnore.IsInParty())
            {
                _partyFinder.PartyToAvoid = toIgnore.PartyId;
                return GetRandomActorInArea(toIgnore.Location, toIgnore, _partyFinder.AvoidParty);
            }

            return GetRandomActorInArea(toIgnore.Location, toIgnore);
        }

        private IActor? GetRandomActorInArea(Coord origin, IActor? toIgnore = null, Predicate<IActor>? predicate = null)
        {
            return _simulation.GetRandomActorInArea(origin, toIgnore, predicate);
        }

        private void DisplayDebugInformation(IReadOnlyList<IActor> actors)
        {
            // diagnostic data for debugging
            System.Diagnostics.Debug.WriteLine($"Day {_simulation.Day}");
            foreach (var actor in actors)
            {
                System.Diagnostics.Debug.WriteLine($"Actor with name: {actor.Name} has party: {actor.PartyId} and health: {actor.Health} with location: {actor.Location}");
            }
        }

        public bool IsSimulationFinished()
        {
            if (_simulation.GetActors(actor => actor.Health >= 1).Count() == 1)
            {
                return true;
            }

            return false;
        }

        public IActor GetWinner()
        {
            return _simulation.GetActors(actor => actor.Health >= 1).First();
        }
    }
}
