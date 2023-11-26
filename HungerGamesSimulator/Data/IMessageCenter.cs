namespace HungerGamesSimulator.Data
{
    public interface IMessageCenter
    {
        public void AddMessage( string message );
        public void AddMessage( IActor actor, string key, IReadOnlyList<IActor>? otherActor = null );
        public void AddMessage(IReadOnlyList<IActor> actorsInParty, string key, IReadOnlyList<IActor>? otherActorsInParty = null );
        public void AddCannonMessage( IActor actor );
        public void ClearCannonMessages();
        public void ClearMessages();
        public List<string> GetMessages();
        public List<string> GetCannonMessages();
    }
}