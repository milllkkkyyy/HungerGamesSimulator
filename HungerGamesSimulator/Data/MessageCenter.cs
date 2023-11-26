using System.Diagnostics;

namespace HungerGamesSimulator.Data;

public class MessageCenter : IMessageCenter
{
    private List<string> _messages = new List<string>();
    private List<string> _cannonMessages = new List<string>();

    private readonly Dictionary<string, IReadOnlyList<string>> _messageTemplates;

    public MessageCenter( Dictionary<string, IReadOnlyList<string>> templates  )
    {
        _messageTemplates = templates;
    }

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

    public void AddMessage(IActor actor, string templateKey, IReadOnlyList<IActor>? otherActor = null)
    {
        Debug.Assert(_messageTemplates.TryGetValue(templateKey, out var messages), $"{templateKey} is not a valid key for a message center template");
        var message = messages[Random.Shared.Next(messages.Count())];
        var formattedMessage = string.Format(message, actor, otherActor);
        _messages.Add(formattedMessage);
    }

    public void AddMessage(IReadOnlyList<IActor> actorsInParty, string templateKey, IReadOnlyList<IActor>? otherActorsInParty = null)
    {
        Debug.Assert(_messageTemplates.TryGetValue(templateKey, out var messages), $"{templateKey} is not a valid key for a message center template");
        var message = messages[Random.Shared.Next(messages.Count())];
        var partyNames = SimulationUtils.GetConcatenatedActorNames(actorsInParty);
        var otherPartyNames = SimulationUtils.GetConcatenatedActorNames(otherActorsInParty == null ? new List<IActor>() : otherActorsInParty);
        var formattedMessage = string.Format(message, partyNames, otherPartyNames);
        _messages.Add(formattedMessage);
    }
}