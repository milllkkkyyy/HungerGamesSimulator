using HungerGamesSimulator.MessageCenter;

namespace HungerGamesSimulator.Data
{
    public class EventFactory
    {
        private readonly IMessageCenter _messageCenter;
        private readonly Simulation _simulation;
        private readonly MemoryService _memoryService;

        public EventFactory(IMessageCenter messageCenter, MemoryService memoryService ,Simulation simulation ) 
        {
            _messageCenter = messageCenter; 
            _simulation = simulation;
            _memoryService = memoryService;
        }

        public Event CreateEvent(EventName eventName )
        {
            return eventName switch
            {
                EventName.Burn => new BurnMapEvent(_simulation, _messageCenter),
                EventName.SuddenDeath => new SuddenDeathEvent(_simulation, _messageCenter, _memoryService),
                EventName.Cornucopia => new CornucopiaEvent( _simulation, _messageCenter, _memoryService),
                _ => throw new NotImplementedException($"{eventName} is not implemented for tribute events"),
            };
        }
    }
}
