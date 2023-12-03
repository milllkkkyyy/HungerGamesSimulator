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
        public void AddCombatMessage(string templateKey, IActor actor, IActor other);
        public void AddPartyMessage(string templateKey, List<IActor> party, List<IActor> otherParty);
        public void AddMovingMessage(string templateKey, List<IActor> party, Coord oldLocation, Coord newLocation);
        public void AddPersonalMessage(string templateKey, IActor actor);
    }
}