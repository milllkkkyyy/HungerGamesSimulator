using HungerGamesSimulator.Data.Metadata;
using System.ComponentModel.DataAnnotations;

namespace HungerGamesSimulator.Data;

public class Tribute : IActor
{
    public Tribute( IMetaData data, Coord location )
    {
        Name = data.Name;
        Location = location;
        Velocity = new Coord(1, 1);
    }

    public string? Name { get; set; }
    public Coord Velocity { get; set; }
    public Coord Location { get; set; }
    private void RandomizeDirection()
    {
        var newDirection = Velocity;
        newDirection.X = Random.Shared.Next(0, 2) != 0 ? Velocity.X * -1 : Velocity.X;
        newDirection.Y = Random.Shared.Next(0, 2) != 0 ? Velocity.Y * -1 : Velocity.Y;
        Velocity = newDirection;
    }
    
    # region Implements IActor Functions
    
    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Act( Game world )
    {
        if ( world == null)
        {
            return;
        }

        RandomizeDirection();
        var oldLocation = world.Move( this );
    }

    public bool IsDead()
    {
        return false;
    }

    #endregion

}