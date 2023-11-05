namespace HungerGamesSimulator.Data
{
    public interface IMessageCenter
    {
        public void AddMessage( string message );
        public void AddCannonMessage( IActor actor );
        public void ClearCannonMessages();
        public void ClearMessages();
        public List<string> GetMessages();
        public List<string> GetCannonMessages();
    }
}