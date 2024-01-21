using HungerGamesSimulator.MessageCenter;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HungerGamesSimulator.Data
{
    public class CornucopiaEvent : Event
    {
        private const int ConfidenceDC = 4;

        private const int GrabWeaponDC = 8;

        private readonly IReadOnlyList<IActor> _actorsInEvent;

        private readonly CombatService _combatService;

        private readonly GameStringBuilder _gameStringBuilder;

        public override int ActionsTook { get; }

        public CornucopiaEvent(Simulation simulation, IMessageCenter messageCenter, MemoryService memoryService ) : base(simulation, messageCenter)
        {
            var participatingActors = simulation.GetAliveActors();
            Debug.Assert(participatingActors != null, "Cannot have no alive actors in cornucopia event");
            _actorsInEvent = participatingActors;
            ActionsTook = simulation.ActionsPerDay;
            _combatService = new CombatService( memoryService );
            _gameStringBuilder = simulation.GameStringFactory.CreateStringBuilder();
        }

        public override IReadOnlyList<IActor> Run()
        {
            // keeps track of tributes who continue after each round
            List<IActor> actorsThatAreFighting = new();

            // determine which tributes run to cornucopia or away
            foreach (var actor in _actorsInEvent)
            {
                if (actor.Dexerity + SimulationUtils.RollD20() >= GrabWeaponDC && actor.Wisdom + SimulationUtils.RollD20() >= ConfidenceDC)
                {
                    actorsThatAreFighting.Add(actor);
                }
                else
                {
                    _gameStringBuilder.QueueInformation( new ContextType[] { ContextType.CornucopiaRunAway } , actor );
                }
            }

            _messageCenter.AddMessage(_gameStringBuilder.ToString());

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
                    actor.Weapon = GetWeaponFromLoot(actor);
                    _gameStringBuilder.QueueInformation(new ContextType[] { ContextType.CornucopiaLuck }, actor);
                }


                if (!actor.IsDead() && actor.Wisdom + SimulationUtils.RollD20() >= ConfidenceDC)
                {
                    actorsParticipatingInBloodBath.Add(actor);
                }

                actorsWent.Add(actor);
                _messageCenter.AddMessage(_gameStringBuilder.ToString());
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

                var otherActor = EventUtils.GetRandomActor(actorsParticipatingInBloodBath, actor);
                if (otherActor == null)
                {
                    break;
                }

                fighters.Add(actor);
                defenders.Add(otherActor);

                _combatService.Simulate(new CombatRequest(fighters, defenders), _gameStringBuilder, _messageCenter);

                fighters.Clear();
                defenders.Clear();

                _messageCenter.AddMessage(_gameStringBuilder.ToString());
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
            if (actorsPossible != null && actorsPossible.Count != 0)
            {
                randomActor = actorsPossible[Random.Shared.Next(actorsPossible.Count)];
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
            inAdvantage.Weapon = GetWeaponFromLoot(inAdvantage);
            _gameStringBuilder.QueueInformation(new ContextType[] { ContextType.CornucopiaOutpace }, new object[] { new CornucopiaTribute(inAdvantage, true), new CornucopiaTribute(inDisadvantage, false) });

            if (inAdvantage.Wisdom + SimulationUtils.RollD20() < ConfidenceDC)
            {
                _gameStringBuilder.QueueInformation(new ContextType[] { ContextType.CornucopiaLuck }, inDisadvantage);
                inDisadvantage.Weapon = GetWeaponFromLoot(inDisadvantage);
                return;
            }

            _combatService.Simulate(new CombatRequest( new List<IActor>() { inAdvantage } , new List<IActor>() { inDisadvantage } ), _gameStringBuilder, _messageCenter);
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

    public class CornucopiaTribute : Tribute
    {
        public bool HasAdvantage { get; set; }

        public CornucopiaTribute( IActor tribute, bool advantage )
        {
            this.ActorId = tribute.ActorId;
            this.Name = tribute.Name;
            this.ArmourClass = tribute.ArmourClass;
            this.Weapon = tribute.Weapon;
            this.Charisma = tribute.Charisma;
            this.Dexerity = tribute.Dexerity;
            this.Wisdom = tribute.Wisdom;
            this.Strength = tribute.Strength;

            HasAdvantage = advantage;
        }
    }
}
