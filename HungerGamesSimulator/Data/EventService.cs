namespace HungerGamesSimulator.Data
{
    public enum EventName
    {
        Burn,
        SuddenDeath,
        Cornucopia
    }

    public class OnEventCompletedArgs : EventArgs
    {
        public IReadOnlyList<IActor> ActorsAffected { get; }

        public OnEventCompletedArgs(IReadOnlyList<IActor> actors) 
        {
            ActorsAffected = actors;
        }
    }

    public class EventService
    {
        public event EventHandler<OnEventCompletedArgs>? OnEventCompleted;

        private readonly EventFactory _eventFactory;

        public EventService(Simulation simulation, IMessageCenter messageCenter )
        {
            _eventFactory = new EventFactory(messageCenter, simulation);
        }

        public int HandleEventCreation(EventName eventName)
        {
            var createdEvent = _eventFactory.CreateEvent(eventName);
            var tributesAffected = createdEvent.Run();
            var eventArgs = new OnEventCompletedArgs(tributesAffected);
            OnEventCompleted?.Invoke(this, eventArgs);
            return createdEvent.ActionsTook;
        }
    }
}
