using HungerGamesSimulator.MessageCenter;
using System.Diagnostics;

namespace HungerGamesSimulator.Data
{
    public class BurnMapEvent : Event
    {
        private const int _burnOffDC = 10;

        private const int _suicideDC = 1;

        private const int _burnDamage = 999999;
        public override int ActionsTook { get; }

        private readonly GameStringBuilder _gameStringBuilder;


        public BurnMapEvent( Simulation simulation, IMessageCenter messageCenter ) : base(simulation, messageCenter)
        {
            ActionsTook = _simulation.ActionsPerDay;
            _gameStringBuilder = _simulation.GameStringFactory.CreateStringBuilder();
        }

        public override IReadOnlyList<IActor> Run()
        {
            _simulation.Height -= 1;
            _simulation.Width -= 1;

            _messageCenter.AddMessage($"The map shrinks to {_simulation.Height} by {_simulation.Width}");


            bool ranFromFire;
            bool killedThemSelves;
            var actorsAffected = _simulation.GetAliveActors();
            Debug.Assert(actorsAffected != null, "Cannot have no alive actors in sudden dead event");

            foreach (var actor in actorsAffected )
            {
                if ( actor.Location.X > _simulation.Width || actor.Location.Y > _simulation.Height )
                {
                    ranFromFire = SimulationUtils.RollD20() + actor.Dexerity >= _burnOffDC;
                    killedThemSelves = SimulationUtils.RollD20() + actor.Wisdom <= _suicideDC;

                    if ( !ranFromFire || killedThemSelves)
                    {
                        actor.TakeDamage(_burnDamage);

                        if (killedThemSelves)
                        {
                            _gameStringBuilder.QueueInformation( new ContextType[] { ContextType.BurnMapSuicide }, new BuilderObject(actor) );
                        }
                        else
                        {
                            _gameStringBuilder.QueueInformation( new ContextType[] { ContextType.BurnMapFail }, new BuilderObject(actor));
                        }

                        _messageCenter.AddCannonMessage(actor);
                        continue;
                    }

                    if (actor.Location.X > _simulation.Width && actor.Location.Y > _simulation.Height)
                    {
                        actor.Location = new Coord(_simulation.Width, _simulation.Height);
                    }
                    else if (actor.Location.X > _simulation.Width && actor.Location.Y < _simulation.Height)
                    {
                        actor.Location = new Coord(_simulation.Width, actor.Location.Y);
                    }
                    else
                    {
                        actor.Location = new Coord(actor.Location.X, _simulation.Height);
                    }

                    _gameStringBuilder.QueueInformation(new ContextType[] { ContextType.BurnMapSucceed }, new BuilderObject(actor));
                }
                else
                {
                    _gameStringBuilder.QueueInformation(new ContextType[] { ContextType.BurnMapIgnore }, new BuilderObject(actor));
                }

                _messageCenter.AddMessage( _gameStringBuilder.ToString() );
            }

            return actorsAffected;
        }
    }
}
