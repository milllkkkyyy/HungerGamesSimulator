namespace HungerGamesSimulator.Data
{
    public class EventFactory
    {
        private readonly IMessageCenter _messageCenter;
        private readonly Simulation _simulation;
        public EventFactory(IMessageCenter messageCenter, Simulation simulation ) 
        {
            _messageCenter = messageCenter; 
            _simulation = simulation;   
        }

        public Event CreateEvent(EventName eventName )
        {
            return eventName switch
            {
                EventName.Burn => new BurnMapEvent(_simulation, _messageCenter),
                EventName.SuddenDeath => new SuddenDeathEvent(_simulation, _messageCenter),
                EventName.Cornucopia => new CornucopiaEvent( _simulation, _messageCenter ),
                _ => throw new NotImplementedException($"{eventName} is not implemented for tribute events"),
            };
        }
    }
}
