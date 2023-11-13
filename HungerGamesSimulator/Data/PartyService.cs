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

        public PartyService( CombatService combatService )
        {
            _combatService = combatService;
            _manager = new PartyManager();

            _combatService.CombatEnded += CombatService_CombatEnded;
        }

        private void CombatService_CombatEnded( object? sender, CombatEndedEventArgs e )
        {
            var deadFighters = e.Fighters.Count( actor => actor.Health < 1 );
            if ( deadFighters == e.Fighters.Count() - 1 )
            {
                _manager.DisbandParty( e.Fighters );
            }

            var deadDefenders = e.Defenders.Count( actor => actor.Health < 1 );
            if ( deadDefenders == e.Defenders.Count() - 1 )
            {
                _manager.DisbandParty( e.Defenders );
            }
        }

        public PartyResponse HandlePartyRequest( PartyRequest request )
        {
            string message = FailMessage;

            switch ( request.PartyRequestType )
            {
                case PartyRequestType.Join:
                if ( request.OtherActorsParty != null )
                {
                    message = HandleJoinParty( request.Actor, request.ActorsParty, request.OtherActorsParty );
                }
                break;
                case PartyRequestType.Leave:
                message = HandleLeaveParty( request.Actor, request.ActorsParty );
                break;
            }

            return new PartyResponse( message );
        }

        private string HandleJoinParty( IActor actor, List<IActor> actorsParty, List<IActor> otherActorsParty )
        {
            IActor otherActor = otherActorsParty.First();

            if ( actor.IsInParty() && otherActor.IsInParty() )
            {
                _manager.MergeParties( actorsParty, otherActorsParty );

                foreach ( var partyMember in actorsParty.Concat( otherActorsParty ) )
                {
                    partyMember.Location = actor.Location;
                }
                return $"{SimulationUtils.GetConcatenatedActorNames( otherActorsParty )} merged parties with {SimulationUtils.GetConcatenatedActorNames( actorsParty )}";
            }
            else if ( actor.IsInParty() && !otherActor.IsInParty() )
            {
                _manager.JoinParty( actor, actor.PartyId );
                otherActorsParty.First().Location = actor.Location;
                return $"{otherActorsParty.First().Name} joined a party with {SimulationUtils.GetConcatenatedActorNames( actorsParty )}";
            }
            else if ( !actor.IsInParty() && otherActor.IsInParty() )
            {
                _manager.JoinParty( actor, otherActor.PartyId );
                actor.Location = otherActor.Location;
                return $"{actorsParty.First().Name} joined a party with {SimulationUtils.GetConcatenatedActorNames( otherActorsParty )}";
            }
            else
            {
                _manager.CreateParty( actor, otherActor );
                otherActor.Location = actor.Location;
                return $"{actorsParty.First().Name} created a party with {otherActorsParty.First().Name}";
            }
        }


        /// <summary>
        /// Leave the current party the actor is a part of
        /// </summary>
        /// <param name="actor"></param>
        private string HandleLeaveParty( IActor actor, List<IActor> actorsParty )
        {
            // dispand any parties that are have less than two people
            if ( actorsParty.Count <= 2 )
            {
                _manager.DisbandParty( actorsParty );
                return $"the party with {SimulationUtils.GetConcatenatedActorNames( actorsParty )} has been dispanded";
            }
            else
            {
                // otherwise, just leave the party
                _manager.LeaveParty( actor );
                actorsParty.Remove( actor );
                return $"{actor.Name} left {SimulationUtils.GetConcatenatedActorNames( actorsParty )} party";
            }
        }

        public void Dispose()
        {
            _combatService.CombatEnded -= CombatService_CombatEnded;
        }
    }

    public enum PartyRequestType
    {
        Join,
        Leave
    }

    public record PartyRequest( PartyRequestType partyRequestType, IActor actor, List<IActor> actorsParty, List<IActor> otherActorsParty = null )
    {
        public PartyRequestType PartyRequestType { get; set; } = partyRequestType;

        public IActor Actor { get; set; } = actor;

        public List<IActor> ActorsParty { get; set; } = actorsParty;

        public List<IActor>? OtherActorsParty { get; set; } = otherActorsParty;
    }

    public record PartyResponse( string message )
    {
        public string Message { get; set; } = message;
    }
}
