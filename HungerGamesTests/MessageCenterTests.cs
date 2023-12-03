using HungerGamesSimulator.Data;

namespace HungerGamesTests;

public class MessageCenterTests
{
    private readonly string message = $"test";
    
    [Fact] public void Test_Class_Creation()
    {
        var messageCenter = new MessageCenter( new Dictionary<string, IReadOnlyList<string>>() );
        Assert.NotNull(messageCenter);
    }

    [Fact]
    public void Test_Add_Message()
    {
        var messageCenter = new MessageCenter( new Dictionary<string, IReadOnlyList<string>>() );
        messageCenter.AddMessage( message );
        Assert.Contains(message, messageCenter.GetMessages());
    }

    [Fact]
    public void Test_Clear_Message()
    {
        var messageCenter = new MessageCenter(new Dictionary<string, IReadOnlyList<string>>());
        messageCenter.AddMessage( message );
        messageCenter.ClearMessages();
        Assert.Empty(messageCenter.GetMessages());
    }
}