using HungerGamesSimulator.MessageCenter;
using System.Diagnostics;

namespace HungerGamesSimulator.Data
{
    /// <summary>
    /// Event that simulates suddent death. All actors will attack each other.
    /// </summary>
    public class SuddenDeathEvent : Event
    {
        private readonly IReadOnlyList<IActor> _actorsInEvent;

        private bool _eventFinished = false;
        public override int ActionsTook { get; }

        private readonly GameStringBuilder _gameStringBuilder;

        private readonly CombatService _combatService;

        public SuddenDeathEvent(Simulation simulation, IMessageCenter messageCenter, MemoryService memoryService) : base (simulation, messageCenter )
        {
            var participatingActors = simulation.GetAliveActors();
            Debug.Assert(participatingActors != null, "Cannot have no alive actors in sudden dead event");
            this._actorsInEvent = participatingActors;
            ActionsTook = _simulation.ActionsPerDay;
            _gameStringBuilder = simulation.GameStringFactory.CreateStringBuilder();
            _combatService = new CombatService( memoryService );
        }

        public override IReadOnlyList<IActor> Run()
        {
            _messageCenter.AddMessage($"Sudden Death Event");
            List<IActor> actorsFighting = _actorsInEvent.ToList();
            while (!_eventFinished)
            {
                foreach (var actor in actorsFighting)
                {
                    if (actor.Health < 1)
                    {
                        continue;
                    }

                    var otherActor = EventUtils.GetRandomActor(_actorsInEvent, actor);
                    if (otherActor == null)
                    {
                        break;
                    }

                    _combatService.Simulate(new CombatRequest(new List<IActor> { actor }, new List<IActor> { otherActor }), _gameStringBuilder, _messageCenter);
                }

                UpdateEventFinished();
                CombatUtils.Shuffle(actorsFighting);

            }

            _gameStringBuilder.QueueInformation(new ContextType[] { ContextType.SuddenDeathWinner } , new BuilderObject( GetWinner() ));

            return _actorsInEvent;
        }

        private IActor GetWinner()
        {
            return _actorsInEvent.First(actor => !actor.IsDead());
        }

        private void UpdateEventFinished()
        {
            _eventFinished = _actorsInEvent.Count(actor => !actor.IsDead()) == 1;
        }
    }
}
