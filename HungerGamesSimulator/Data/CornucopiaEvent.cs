using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HungerGamesSimulator.Data
{
    public class CornucopiaEvent : Event
    {
        private const int ConfidenceDC = 10;

        private const int GrabWeaponDC = 14;

        private readonly IReadOnlyList<IActor> _actorsInEvent;

        private readonly CombatService _combatService;

        public override int ActionsTook { get; }

        public CornucopiaEvent(Simulation simulation, IMessageCenter messageCenter) : base(simulation, messageCenter)
        {
            var participatingActors = simulation.GetAliveActors();
            Debug.Assert(participatingActors != null, "Cannot have no alive actors in cornucopia event");
            _actorsInEvent = participatingActors;
            ActionsTook = simulation.ActionsPerDay;
            _combatService = new CombatService();
        }

        public override IReadOnlyList<IActor> Run()
        {
            // keeps track of tributes who continue after each round
            List<IActor> actorsThatAreFighting = new();

            // determine which tributes run to cornucopia or away
            foreach (var actor in _actorsInEvent)
            {
                if (actor.Dexerity + SimulationUtils.RollD20() > GrabWeaponDC && actor.Wisdom + SimulationUtils.RollD20() > ConfidenceDC)
                {
                    actorsThatAreFighting.Add(actor);
                }
                else
                {
                    _messageCenter.AddMessage($"{actor.Name} ran away from the Cornucopia");
                }
            }

            // keeps track of actors that want to fight after
            List<IActor> actorsParticipatingInBloodBath = new();

            // pairs actors who run towards cornucopia and simulates them running and battling
            HashSet<IActor> actorsWent = new();
            foreach ( var actor in actorsThatAreFighting )
            {
                if (!actorsWent.Add(actor))
                {
                    continue;
                }

                if (TryGetRandomActorInEvent(actorsThatAreFighting, out var otherActor, actorsWent))
                {
                    if (actor.Dexerity + SimulationUtils.RollD20() < otherActor.Dexerity + SimulationUtils.RollD20())
                    {
                        SimulateRunningToCornicopia(otherActor, actor);
                    }
                    else
                    {
                        SimulateRunningToCornicopia(actor, otherActor);
                    }

                    if (!otherActor.IsDead() && otherActor.Wisdom + SimulationUtils.RollD20() >= ConfidenceDC)
                    {
                        actorsParticipatingInBloodBath.Add(otherActor);
                    }

                    actorsWent.Add(otherActor);
                }
                else
                {
                    var weapon = GetWeaponFromLoot(actor);
                    _messageCenter.AddMessage($"{actor.Name} ran towards the cornucopia and grabbed a {weapon.Name} while everyone was distracted");
                }


                if (!actor.IsDead() && actor.Wisdom + SimulationUtils.RollD20() >= ConfidenceDC)
                {
                    actorsParticipatingInBloodBath.Add(actor);
                }
                actorsWent.Add(actor);
            }

            // simulate battleing
            var fighters = new List<IActor>();
            var defenders = new List<IActor>();
            foreach (var actor in actorsParticipatingInBloodBath)
            {
                if (actor.Health < 1)
                {
                    continue;
                }

                var otherActor = EventUtils.GetRandomActor(_actorsInEvent, actor);

                fighters.Add(actor);
                defenders.Add(otherActor);

                var response = _combatService.Simulate(new CombatRequest(fighters, defenders));
                if (response.Escaped)
                {
                    _messageCenter.AddMessage($"{actor.Name} attacked {otherActor.Name}. {otherActor.Name} was grazed in the attack");
                }
                else if (response.DefendersDied)
                {
                    _messageCenter.AddMessage($"{actor.Name} slayed {otherActor.Name}");
                    _messageCenter.AddCannonMessage(otherActor);
                }
                else
                {
                    _messageCenter.AddMessage($"{actor.Name} attacked {otherActor.Name}, but {otherActor.Name} was able to slay {actor.Name}");
                    _messageCenter.AddCannonMessage(actor);
                }

                fighters.Clear();
                defenders.Clear();
            }


            return _actorsInEvent;
        }

        /// <summary>
        /// Try to get a random actor in event
        /// </summary>
        /// <param name="actors"></param>
        /// <param name="randomActor"></param>
        /// <param name="actorsWent"></param>
        /// <returns></returns>
        private bool TryGetRandomActorInEvent( List<IActor> actors, [NotNullWhen(returnValue: true)] out IActor? randomActor, HashSet<IActor>? actorsWent = null  )
        {
            var actorsPossible = actors.Where(actor => actorsWent != null && !actorsWent.Contains(actor)).ToList();
            if (actorsPossible != null)
            {
                randomActor = actorsPossible[Random.Shared.Next(actorsPossible.Count())];
                return true;
            }

            randomActor = null;
            return false;
        }

        /// <summary>
        /// Simulates running to the cornicopia
        /// </summary>
        /// <param name="inAdvantage"></param>
        /// <param name="inDisadvantage"></param>
        private void SimulateRunningToCornicopia(IActor inAdvantage, IActor inDisadvantage)
        {
            var fighters = new List<IActor>();
            var defenders = new List<IActor>();
            inAdvantage.Weapon = GetWeaponFromLoot(inAdvantage);
            if (inAdvantage.Wisdom + SimulationUtils.RollD20() < ConfidenceDC)
            {
                _messageCenter.AddMessage($"{inDisadvantage.Name} outpaced {inAdvantage.Name} in grabbing a {inAdvantage.Weapon.Name}, but they ran away. Allowing {inDisadvantage.Name} to get a {inDisadvantage.Weapon.Name} and escape");
                inDisadvantage.Weapon = GetWeaponFromLoot(inDisadvantage);
                return;
            }

            fighters.Add(inAdvantage);
            defenders.Add(inDisadvantage);
            var response = _combatService.Simulate(new CombatRequest(fighters, defenders));
            if (response.Escaped)
            {
                _messageCenter.AddMessage($"{inAdvantage.Name} picked up a {inAdvantage.Weapon} and attacked {inDisadvantage.Name}. {inDisadvantage.Name} escaped");
            }
            else if (response.DefendersDied)
            {
                _messageCenter.AddMessage($"{inAdvantage.Name} picked up a {inAdvantage.Weapon} and slayed {inDisadvantage.Name}");
                _messageCenter.AddCannonMessage(inDisadvantage);
            }
            else
            {
                _messageCenter.AddMessage($"{inAdvantage.Name} picked up a {inAdvantage.Weapon} and attacked {inDisadvantage.Name}, but {inDisadvantage.Name} was able to slay {inAdvantage.Name}");
                inDisadvantage.Weapon = inAdvantage.Weapon;
                _messageCenter.AddCannonMessage(inAdvantage);
            }
        }

        /// <summary>
        /// Get a random weapon from the loot table in the simulations
        /// </summary>
        /// <param name="actorToIgnore"> the actor that you are giving the weapon to </param>
        /// <returns></returns>
        private Weapon GetWeaponFromLoot( IActor actorToIgnore )
        {
            return _simulation.GetRandomWeapon(actorToIgnore.Weapon);
        }
    }
}
