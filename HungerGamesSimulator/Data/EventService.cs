using HungerGamesSimulator.MessageCenter;

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

        private readonly Simulation _simulation;
        
        public EventService(Simulation simulation, IMessageCenter messageCenter, MemoryService memoryService )
        {
            _simulation = simulation;
            _eventFactory = new EventFactory(messageCenter, memoryService, _simulation);
        }

        private int HandleEventCreation(EventName eventName)
        {
            var createdEvent = _eventFactory.CreateEvent(eventName);
            var tributesAffected = createdEvent.Run();
            var eventArgs = new OnEventCompletedArgs(tributesAffected);
            OnEventCompleted?.Invoke(this, eventArgs);
            return createdEvent.ActionsTook;
        }

        /// <summary>
        /// Decide if there will be an event for today
        /// </summary>
        /// <returns>the number of actions the event took, if there was one</returns>
        public int RunEventsIfNeeded()
        {
            if (ShouldSuddenDeathEventTrigger())
            {
                return HandleEventCreation(EventName.SuddenDeath);
            }

            if (_simulation.Day == 1)
            {
                return HandleEventCreation(EventName.Cornucopia);
            }

            if (_simulation.Day % 3 == 0)
            {
                return HandleEventCreation(EventName.Burn);
            }

            return 0;
        }


        private bool ShouldSuddenDeathEventTrigger()
        {
            if (_simulation.Width <= 1 && _simulation.Height <= 1)
            {
                return true;
            }
            var aliveActors = _simulation.GetActors(actor => actor.Health >= 1);
            if (aliveActors.Count == 2)
            {
                return true;
            }

            var actorsInParty = _simulation.GetParty(aliveActors.First());
            if (aliveActors.Count == actorsInParty.Count)
            {
                return true;
            }

            return false;
        }
    }
}
