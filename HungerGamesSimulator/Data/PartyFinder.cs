namespace HungerGamesSimulator.Data
{
    public class PartyFinder
    {
        public Guid PartyToFind { get; set; } = Guid.Empty;

        public Guid PartyToAvoid { get; set; } = Guid.Empty;

        public Predicate<IActor> FindParty
        {
            get => IsSameParty;
        }

        public Predicate<IActor> AvoidParty
        {
            get => IsAvoidingParty;
        }


        private bool IsSameParty( IActor actor )
        {
            return actor.IsInParty() && actor.PartyId == PartyToFind;
        }

        private bool IsAvoidingParty( IActor actor )
        {
            return actor.PartyId != PartyToAvoid;
        }
    }
}
