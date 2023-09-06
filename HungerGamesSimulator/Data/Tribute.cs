using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace HungerGamesSimulator.Data;

public class Tribute : IActor
{
    public Tribute(string name)
    {
        Name = name;
        Velocity = new Coord(1, 1);
    }

    [Required] 
    public string? Name { get; set; }
    public Coord Velocity { get; set; }
    public Coord Location { get; set; }
    public World? World { get; set; }
    private void RandomizeDirection()
    {
        var newDirection = Velocity;
        newDirection.X = World.Random.Next(0, 2) != 0 ? Velocity.X * -1 : Velocity.X;
        newDirection.Y = World.Random.Next(0, 2) != 0 ? Velocity.Y * -1 : Velocity.Y;
        Velocity = newDirection;
    }
    
    # region Implements IActor Functions
    
    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Act()
    {
        if (World == null)
        {
            return;
        }

        RandomizeDirection();
        var oldLocation = World.Move( this );
        MessageCenter.AddMessage( $"{Name} moved from {oldLocation} to {Location}" );
    }

    public bool IsDead()
    {
        return false;
    }

    #endregion

}