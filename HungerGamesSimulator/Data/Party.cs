using System.Collections.ObjectModel;
using System.IO;

namespace HungerGamesSimulator.Data
{
    public class Party : List<IActor>
    {

        public Party Dead 
        {
            get
            {
                return new Party( this.Where(actor => actor.IsDead()) );
            }
        }

        public Party Alive
        {
            get
            {
                return new Party(this.Where(actor => !actor.IsDead()));
            }
        }

        public Party( IEnumerable<IActor> actor ) : base( actor ) 
        {
        }

        public override string ToString()
        {
            if (Count < 0)
            {
                return string.Empty;
            }

            return this.Count > 1 ? string.Join(", ", this.GetRange(0, Count-1)) + " and " + this.Last().Name : this.First().Name;
        }
    }
}
