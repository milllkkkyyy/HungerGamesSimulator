using HungerGamesSimulator.MessageCenter;
using System.Collections.Immutable;

namespace HungerGamesSimulator.Data
{
    /// <summary>
    /// Creates and manages tributes parties
    /// </summary>
    public class PartyService : IDisposable
    {
        private PartyManager _manager;
        private CombatService _combatService;
        private EventService _eventService;
        private MemoryService _memoryService;

        public PartyService(CombatService combatService, EventService eventService, MemoryService memoryService)
        {
            _combatService = combatService;
            _eventService = eventService;
            _memoryService = memoryService;

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

        /// <summary>
        /// Joins a party together depending on their situation
        /// </summary>
        /// <param name="request"></param>
        public void JoinParty( JoinPartyRequest request )
        {
            request.Builder.QueueInformation(new ContextType[] { ContextType.PartyJoin }, request.Actor.IsInParty() ? request.ActorsParty : request.Actor, !request.OtherActor.IsInParty() ? request.OtherActor : request.OtherActorsParty);

            if (request.Actor.IsInParty() && request.OtherActor.IsInParty())
            {
                _manager.MergeParties(request.ActorsParty, request.OtherActorsParty);
            }
            else if (request.Actor.IsInParty() && !request.OtherActor.IsInParty())
            {
                _manager.JoinParty(request.OtherActor, request.Actor.PartyId);
            }
            else if (!request.Actor.IsInParty() && request.OtherActor.IsInParty())
            {
                _manager.JoinParty(request.Actor, request.OtherActor.PartyId);
            }
            else
            {
                _manager.CreateParty(request.Actor, request.OtherActor);
            }

            // add new memory and set location
            var allPartyMembers = request.ActorsParty.Concat(request.OtherActorsParty).ToImmutableList();
            foreach (var partyMember in allPartyMembers)
            {
                partyMember.Location = request.Actor.Location;

                var partyMemebersIds = allPartyMembers.Where(actor => actor.ActorId != partyMember.ActorId)
                    .Select(actor => actor.ActorId); 
                _memoryService.AddActorMemory(partyMember.ActorId, partyMemebersIds , MemoryType.Good);
            }
        }


        /// <summary>
        /// Leave the current party the actor is a part of
        /// </summary>
        /// <param name="actor"></param>
        public void LeaveParty(LeavePartyRequest request)
        {
            // add new memory and set location
            foreach (var partyMember in request.ActorsParty)
            {
                partyMember.Location = request.Actor.Location;

                var partyMemebersIds = request.ActorsParty.Where(actor => actor.ActorId != partyMember.ActorId)
                    .Select(actor => actor.ActorId);
                _memoryService.AddActorMemory(partyMember.ActorId, partyMemebersIds, MemoryType.Bad);
            }

            _manager.LeaveParty(request.Actor);

            request.ActorsParty.Remove(request.Actor);

            request.Builder.QueueInformation(new ContextType[] { ContextType.PartyLeave }, new object[] { request.Actor, request.ActorsParty });

            // dispand any parties that are have less than two people
            if (request.ActorsParty.Count < 2)
            {
                _manager.DisbandParty(request.ActorsParty);
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

    public record LeavePartyRequest(IActor Actor, Party ActorsParty, GameStringBuilder Builder )
    {
        public IActor Actor { get; } = Actor;

        public Party ActorsParty { get; } = ActorsParty;

        public GameStringBuilder Builder { get; } = Builder;
    }

    public record JoinPartyRequest(IActor Actor, IActor OtherActor, Party ActorsParty, Party OtherActorsParty, GameStringBuilder Builder)
    {
        public IActor Actor { get; } = Actor;

        public IActor OtherActor { get; } = OtherActor;

        public Party ActorsParty { get; } = ActorsParty;

        public Party OtherActorsParty { get; } = OtherActorsParty;
        public GameStringBuilder Builder { get; } = Builder;
    }
}
