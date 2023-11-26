using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace HungerGamesSimulator.Data
{
    public abstract class Event
    {
        /// <summary>
        /// The number of actions the event takes from the tributes, this shouldn't be greater than the amount of actions per day in the simulation
        /// </summary>
        public abstract int ActionsTook { get; }

        protected Simulation _simulation;

        protected IMessageCenter _messageCenter;

        protected Event( Simulation simulation, IMessageCenter messageCenter )
        {
            _simulation = simulation;
            _messageCenter = messageCenter;
        }

        public abstract IReadOnlyList<IActor> Run();
    }
}
