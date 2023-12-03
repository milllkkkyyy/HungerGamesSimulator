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

    /// <summary>
    /// Deprecated
    /// </summary>
    /// <param name="message"></param>
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

    private string GetRandomMessageFromTemplate( string templateKey )
    {
        Debug.Assert(_messageTemplates.TryGetValue(templateKey, out var messages), $"{templateKey} is not a valid key for a message center template");
        return messages[Random.Shared.Next(messages.Count())];
    }

    private void AppendFormattedMessage(string message, object[] args)
    {
        var formattedMessage = string.Format(message, args );
        AddMessage(formattedMessage);
    }
    
    public void AddCombatMessage(string templateKey, IActor actor, IActor other)
    {
        var message = GetRandomMessageFromTemplate(templateKey);

        object[] args = new object[4];

        args[0] = actor.Name;
        args[1] = other.Name;
        args[2] = actor.Weapon;
        args[3] = other.Weapon;

        AppendFormattedMessage(message, args);
    }

    public void AddPartyMessage(string templateKey, List<IActor> party, List<IActor> otherParty)
    {
        var message = GetRandomMessageFromTemplate(templateKey);

        object[] args = new object[2];

        args[0] = SimulationUtils.GetConcatenatedActorNames( party );
        args[1] = SimulationUtils.GetConcatenatedActorNames( otherParty );

        AppendFormattedMessage(message, args);
    }

    public void AddMovingMessage(string templateKey, List<IActor> party, Coord oldLocation, Coord newLocation )
    {
        var message = GetRandomMessageFromTemplate(templateKey);

        object[] args = new object[1];

        args[0] = SimulationUtils.GetConcatenatedActorNames(party);
        args[1] = oldLocation;
        args[2] = newLocation;

        AppendFormattedMessage(message, args);
    }

    public void AddPersonalMessage(string templateKey, IActor actor)
    {
        var message = GetRandomMessageFromTemplate(templateKey);

        object[] args = new object[4];

        args[0] = actor.Name;
        args[1] = actor.Weapon;
        args[2] = actor.Location;

        AppendFormattedMessage(message, args);
    }
}