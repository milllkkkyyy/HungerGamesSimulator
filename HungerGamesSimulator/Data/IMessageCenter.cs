namespace HungerGamesSimulator.Data
{
    public interface IMessageCenter
    {
        void AddMessage( string message );
        void ClearMessages();
        List<string> GetMessages();
    }
}