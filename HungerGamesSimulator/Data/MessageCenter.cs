namespace HungerGamesSimulator.Data;

public class MessageCenter : IMessageCenter
{
    private List<string> _messages = new List<string>();

    public void AddMessage( string message )
    {
        _messages.Add( message );
    }

    public void ClearMessages()
    {
        _messages.Clear();
    }

    public List<string> GetMessages()
    {
        return _messages;
    }
}