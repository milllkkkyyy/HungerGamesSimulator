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

        public SuddenDeathEvent(Simulation simulation, IMessageCenter messageCenter) : base (simulation, messageCenter )
        {
            var participatingActors = simulation.GetAliveActors();
            Debug.Assert(participatingActors != null, "Cannot have no alive actors in sudden dead event");
            this._actorsInEvent = participatingActors;
            ActionsTook = _simulation.ActionsPerDay;
        }

        public override IReadOnlyList<IActor> Run()
        {
            _messageCenter.AddMessage($"A Sudden Death Event started with {SimulationUtils.GetConcatenatedActorNames(_actorsInEvent)}");
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

                    if (actor.SimulateHit(otherActor))
                    {
                        otherActor.TakeDamage(SimulationUtils.CalculateDamage(actor));
                        _messageCenter.AddMessage($"{actor.Name} hit {otherActor.Name} with {actor.Weapon.Name}");
                    }
                    else
                    {
                        _messageCenter.AddMessage($"{actor.Name} missed their attack on {otherActor.Name}");
                    }

                    if (otherActor.Health < 1)
                    {
                        _messageCenter.AddMessage($"{actor.Name} slayed {otherActor.Name}");
                        _messageCenter.AddCannonMessage(otherActor);
                    }

                }

                UpdateEventFinished();
                CombatUtils.Shuffle(actorsFighting);

            }

            _messageCenter.AddMessage($"{GetWinner().Name} is the last one standing after the suddent death event.");

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
