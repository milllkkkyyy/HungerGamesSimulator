namespace HungerGamesSimulator.Data
{
    /// <summary>
    /// Creates and manages tributes parties
    /// </summary>
    public class PartyService
    {
        private readonly string failMessage = "Party service cannot handle this party request type";

        private PartyManager manager;

        public PartyService()
        {
            manager = new PartyManager();
        }

        public PartyResponse HandlePartyRequest( PartyRequest request )
        {
            string message = failMessage;

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
                manager.MergeParties( actorsParty, otherActorsParty );

                foreach ( var partyMember in actorsParty.Concat( otherActorsParty ) )
                {
                    partyMember.Location = actor.Location;
                }
                return $"{SimulationUtils.GetConcatenatedActorNames( otherActorsParty )} merged parties with {SimulationUtils.GetConcatenatedActorNames( actorsParty )}";
            }
            else if ( actor.IsInParty() && !otherActor.IsInParty() )
            {
                manager.JoinParty( actor, actor.PartyId );
                otherActorsParty.First().Location = actor.Location;
                return $"{otherActorsParty.First().Name} joined a party with {SimulationUtils.GetConcatenatedActorNames( actorsParty )}";
            }
            else if ( !actor.IsInParty() && otherActor.IsInParty() )
            {
                manager.JoinParty( actor, otherActor.PartyId );
                actor.Location = otherActor.Location;
                return $"{actorsParty.First().Name} joined a party with {SimulationUtils.GetConcatenatedActorNames( otherActorsParty )}";
            }
            else
            {
                manager.CreateParty( actor, otherActor );
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
                manager.DisbandParty( actorsParty );
                return $"the party with {SimulationUtils.GetConcatenatedActorNames( actorsParty )} has been dispanded";
            }
            else
            {
                // otherwise, just leave the party
                manager.LeaveParty( actor );
                actorsParty.Remove( actor );
                return $"{actor.Name} left {SimulationUtils.GetConcatenatedActorNames( actorsParty )} party";
            }
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
