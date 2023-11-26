namespace HungerGamesSimulator.Data
{
    public class BurnMapEvent : Event
    {
        private const int _burnOffDC = 10;

        private const int _suicideDC = 1;

        private const int _burnDamage = 999999;
        public override int ActionsTook { get; }

        public BurnMapEvent( Simulation simulation, IMessageCenter messageCenter ) : base(simulation, messageCenter)
        {
            ActionsTook = _simulation.ActionsPerDay;
        }

        public override IReadOnlyList<IActor> Run()
        {
            _simulation.Height -= 1;
            _simulation.Width -= 1;

            bool ranFromFire;
            bool killedThemSelves;
            List<IActor> actorsAffected = new();

            foreach (var actor in _simulation.GetActors())
            {
                ranFromFire = SimulationUtils.RollD20() + actor.Dexerity >= _burnOffDC;
                killedThemSelves = SimulationUtils.RollD20() + actor.Wisdom <= _suicideDC;

                if ((actor.Location.X > _simulation.Width ||
                    actor.Location.Y > _simulation.Height) &&
                    ranFromFire && 
                    !killedThemSelves)
                {

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
                        actor.Location = new Coord(actor.Location.X, actor.Location.Y);
                    }

                    _messageCenter.AddMessage($"{actor.Name} successfully ran from the flames.");
                }
                else
                {
                    actor.TakeDamage(_burnDamage);

                    if (killedThemSelves)
                    {
                        _messageCenter.AddMessage($"{actor.Name} couldn't take the stress of being in the Hunger Games and killed themselves.");
                    }
                    else
                    {
                        _messageCenter.AddMessage($"{actor.Name} was eaten by the flames while running for safety.");
                    }
                }

                actorsAffected.Add(actor);
            }


            return actorsAffected;
        }
    }
}
