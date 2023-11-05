namespace HungerGamesSimulator.Data;

public class MessageCenter : IMessageCenter
{
    private List<string> _messages = new List<string>();
    private List<string> _cannonMessages = new List<string>();

    public void AddMessage( string message )
    {
        _messages.Add( message );
    }

    public void ClearMessages()
    {
        _messages.Clear();
    }

    public void ClearCannonMessages()
    {
        _cannonMessages.Clear();
    }

    public List<string> GetMessages()
    {
        return _messages;
    }

    public List<string> GetCannonMessages()
    {
        return _cannonMessages;
    }

    public void AddCannonMessage( IActor actor )
    {
        _cannonMessages.Add( $"{actor.Name}" );
    }
}