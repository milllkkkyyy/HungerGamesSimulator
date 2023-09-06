using System.Numerics;

namespace HungerGamesSimulator.Data;

public class World
{
    private int width;

    private int height;
    
    private List<IActor> _actors;

    private IEvent? _event;

    public int Day { get; private set; } = 1;

    public static readonly Random Random = new Random();

    public World( int x , int y, List<IActor> actors)
    {
        width = x;
        height = y;
        _actors = actors;

        foreach (var actor in _actors)
        {
            actor.Location = new Coord(width / 2, height / 2);
        }
    }
    
    public void Simulate()
    {
        MessageCenter.ClearMessages();
        
        MessageCenter.AddMessage( $"Day: {Day}" );

        if (Day == 1)
        {
            _event = new Cornucopia();
        }

        if (_event == null)
        {
            foreach (var actor in _actors)
            {
                actor.Act();
            }
        }
        else
        {
            _actors = _event.Simulate(_actors);
            _event = null;
        }

        Day++;
    }

    public Coord Move(Tribute tribute)
    {
        var oldLocation = tribute.Location;
        
        var wishLocation = new Coord(tribute.Location.X + tribute.Velocity.X, tribute.Location.Y + tribute.Velocity.Y);
        wishLocation.X = Math.Clamp(wishLocation.X, 0, width);
        wishLocation.Y = Math.Clamp(wishLocation.Y, 0, height);
        tribute.Location = wishLocation;
        
        return oldLocation;
    }
}
