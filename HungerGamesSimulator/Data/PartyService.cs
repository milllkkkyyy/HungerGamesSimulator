namespace HungerGamesSimulator.Data
{
    /// <summary>
    /// Creates and manages tributes parties
    /// </summary>
    public class PartyService : IDisposable
    {
        private readonly string FailMessage = "Party service cannot handle this party request type";

        private PartyManager _manager;
        private CombatService _combatService;
        private EventService _eventService;

        public PartyService(CombatService combatService, EventService eventService)
        {
            _combatService = combatService;
            _eventService = eventService;
            _manager = new PartyManager();

            _eventService.OnEventCompleted += EventService_OnEventCompleted;
            _combatService.CombatEnded += CombatService_CombatEnded;
        }

        private void EventService_OnEventCompleted(object? sender, OnEventCompletedArgs args)
        {
            bool TryGetParty(IActor actorToIgnore, out List<IActor> party)
            {
                party = args.ActorsAffected.Where(actor =>
                    actorToIgnore != actor
                    && actor.IsInParty()
                    && actorToIgnore.PartyId == actor.PartyId).ToList();

                if (party != null)
                {
                    party.Add(actorToIgnore);
                    return true;
                }
                return false;
            }

            HashSet<Guid> partiesWent = new();
            foreach (var actor in args.ActorsAffected)
            {
                if ( actor.IsInParty() && !partiesWent.Contains( actor.PartyId ) && TryGetParty(actor, out var party))
                {
                    partiesWent.Add(actor.PartyId);

                    var deadMembers = party.Count(actor => actor.IsDead() );
                    if (deadMembers >= party.Count - 1)
                    {
                        _manager.DisbandParty( party );
                        continue;
                    }

                    foreach ( var partyMember in party )
                    {
                        if (partyMember.IsDead())
                        {
                            _manager.LeaveParty(partyMember);  
                        }
                    }
                }
            }
        }

        private void CombatService_CombatEnded(object? sender, CombatEndedEventArgs e)
        {
            var deadFighters = e.Fighters.Count(actor => actor.Health < 1);
            if (deadFighters == e.Fighters.Count - 1)
            {
                _manager.DisbandParty(e.Fighters);
            }

            var deadDefenders = e.Defenders.Count(actor => actor.Health < 1);
            if (deadDefenders == e.Defenders.Count - 1)
            {
                _manager.DisbandParty(e.Defenders);
            }
        }

        public PartyResponse HandlePartyRequest(PartyRequest request)
        {
            string message = FailMessage;

            switch (request.PartyRequestType)
            {
                case PartyRequestType.Join:
                    if (request.OtherActorsParty != null)
                    {
                        message = HandleJoinParty(request.Actor, request.ActorsParty, request.OtherActorsParty);
                    }
                    break;
                case PartyRequestType.Leave:
                    message = HandleLeaveParty(request.Actor, request.ActorsParty);
                    break;
            }

            return new PartyResponse(message);
        }

        private string HandleJoinParty(IActor actor, List<IActor> actorsParty, List<IActor> otherActorsParty)
        {
            IActor otherActor = otherActorsParty.First();

            if (actor.IsInParty() && otherActor.IsInParty())
            {
                _manager.MergeParties(actorsParty, otherActorsParty);

                foreach (var partyMember in actorsParty.Concat(otherActorsParty))
                {
                    partyMember.Location = actor.Location;
                }
                return $"{SimulationUtils.GetConcatenatedActorNames(actorsParty)} merged parties with {SimulationUtils.GetConcatenatedActorNames(otherActorsParty)}";
            }
            else if (actor.IsInParty() && !otherActor.IsInParty())
            {
                _manager.JoinParty(otherActor, actor.PartyId);
                otherActorsParty.First().Location = actor.Location;
                return $"{SimulationUtils.GetConcatenatedActorNames(actorsParty)} conviced {otherActorsParty.First().Name} to join their party";
            }
            else if (!actor.IsInParty() && otherActor.IsInParty())
            {
                _manager.JoinParty(actor, otherActor.PartyId);
                actor.Location = otherActor.Location;
                return $"{actorsParty.First().Name} joined a party with {SimulationUtils.GetConcatenatedActorNames(otherActorsParty)}";
            }
            else
            {
                _manager.CreateParty(actor, otherActor);
                otherActor.Location = actor.Location;
                return $"{actorsParty.First().Name} created a party with {otherActorsParty.First().Name}";
            }
        }


        /// <summary>
        /// Leave the current party the actor is a part of
        /// </summary>
        /// <param name="actor"></param>
        private string HandleLeaveParty(IActor actor, List<IActor> actorsParty)
        {
            // dispand any parties that are have less than two people
            if (actorsParty.Count <= 2)
            {
                _manager.DisbandParty(actorsParty);
                return $"the party with {SimulationUtils.GetConcatenatedActorNames(actorsParty)} has been dispanded";
            }
            else
            {
                // otherwise, just leave the party
                _manager.LeaveParty(actor);
                actorsParty.Remove(actor);
                return $"{actor.Name} left {SimulationUtils.GetConcatenatedActorNames(actorsParty)} party";
            }
        }

        public void Dispose()
        {
            _eventService.OnEventCompleted -= EventService_OnEventCompleted;
            _combatService.CombatEnded -= CombatService_CombatEnded;
        }
    }

    public enum PartyRequestType
    {
        Join,
        Leave
    }

    public record PartyRequest(PartyRequestType partyRequestType, IActor actor, List<IActor> actorsParty, List<IActor> otherActorsParty = null)
    {
        public PartyRequestType PartyRequestType { get; set; } = partyRequestType;

        public IActor Actor { get; set; } = actor;

        public List<IActor> ActorsParty { get; set; } = actorsParty;

        public List<IActor>? OtherActorsParty { get; set; } = otherActorsParty;
    }

    public record PartyResponse(string message)
    {
        public string Message { get; set; } = message;
    }
}
