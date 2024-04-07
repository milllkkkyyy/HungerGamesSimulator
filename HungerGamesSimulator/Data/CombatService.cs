using HungerGamesSimulator.MessageCenter;

namespace HungerGamesSimulator.Data
{
    public class CombatService
    {
        private readonly MemoryService _memoryService;

        public CombatService( MemoryService memoryService )
        { 
            _memoryService = memoryService;
        }

        public event EventHandler<CombatEndedEventArgs>? CombatEnded;
        
        private static bool AreAllTributesDead( List<IActor> tributes )
        {
            int deadTributes = 0;
            foreach ( var tribute in tributes )
            {
                if ( tribute.IsDead() )
                {
                    deadTributes += 1;
                }
            }
            return deadTributes == tributes.Count;
        }

        private bool HandleAttack( IActor fighter, IActor defender )
        {
            if ( fighter.SimulateHit( defender ) )
            {
                defender.TakeDamage( Weapon.RollWeaponDamage( fighter.Weapon ) );
                return true;
            }

            return false;
        }

        private bool DefenderEscaped( IActor fighter, IActor defender )
        {
            return fighter.SimulateEscape( defender );
        }

        public void Simulate( CombatRequest request, GameStringBuilder builder, IMessageCenter messageCenter )
        {
            // retrieve data needed for combat
            int fightersCount = request.Fighters.Count;
            int defendersCount = request.Defenders.Count;
            var fightersArchive = request.Fighters.ToList();
            var defendersArchive = request.Defenders.ToList();

            if ( fightersCount == 0 || defendersCount == 0 )
            {
                throw new ArgumentException( "You can not have no defenders or no fighters" );
            }

            int maxActors = fightersCount < defendersCount ? defendersCount : fightersCount;

            // begin combat loop
            bool escaped = false;
            while ( !escaped && !AreAllTributesDead( request.fighters ) && !AreAllTributesDead( request.defenders ) )
            {
                // attack tributes based on 1 -> 1 combat
                for ( int i = 0; i < maxActors; i++ )
                {
                    // combat could keep going, but defenders all escaped
                    if ( request.Defenders.Count <= 0 )
                    {
                        break;
                    }

                    IActor fighter = request.Fighters.Dequeue();
                    IActor defender = request.Defenders.Dequeue();

                    HandleAttack(fighter, defender);

                    if (defender.IsDead())
                    {
                        builder.QueueInformation(new ContextType[] { ContextType.Combat, ContextType.Death }, new BuilderObject( fighter ), new BuilderObject( defender ) );
                        messageCenter.AddCannonMessage(defender);
                    }
                    else
                    {
                        _memoryService.AddActorMemory(fighter.ActorId, defender.ActorId, MemoryType.Bad);
                        _memoryService.AddActorMemory(defender.ActorId, fighter.ActorId, MemoryType.Bad);

                        if ( !DefenderEscaped(fighter, defender) )
                        {
                            request.Defenders.Enqueue(defender);
                        }
                    }

                    request.Fighters.Enqueue( fighter );
                }


                // reshuffle combat line up for added variety
                CombatUtils.Shuffle( request.Fighters );
                CombatUtils.Shuffle( request.Defenders );

                // check if all the defenders escaped
                escaped = request.Defenders.Count <= 0;
            }

            // build combat string
            messageCenter.AddMessage( builder.ToString() );

            // signal event
            var eventArgs = new CombatEndedEventArgs { Fighters = fightersArchive, Defenders = defendersArchive };
            CombatEnded?.Invoke( this, eventArgs );
        }
    }

    public record CombatRequest( List<IActor> fighters, List<IActor> defenders )
    {
        public Queue<IActor> Fighters { get; } = new Queue<IActor>( fighters );
        public Queue<IActor> Defenders { get; } = new Queue<IActor>( defenders );
    }

    public class CombatEndedEventArgs : EventArgs
    {
        public List<IActor> Fighters { get; set;  }
        public List<IActor> Defenders { get; set; }
    }
}
