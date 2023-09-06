
namespace HungerGamesSimulator.Data;

public static class MessageCenter
{
    private static List<string> _messages = new List<string>();

    public static void AddMessage(string message)
    {
        _messages.Add(message);  
    }

    public static void ClearMessages()
    {
        _messages.Clear();
    }

    public static List<string> GetMessages()
    {
        return _messages;
    }
}