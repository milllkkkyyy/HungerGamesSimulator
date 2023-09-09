using HungerGamesSimulator.Data.Metadata;

namespace HungerGamesSimulator.Data;

public class Game
{
    private int width;

    private int height;
    
    private List<IActor> _actors;

    public int Day { get; private set; } = 1;

    private IMessageCenter messageCenter;

    public Game( int x , int y, List<IMetaData> tributeData, IMessageCenter messageCenter)
    {
        width = x;
        height = y;


        _actors = new List<IActor>();
        foreach (var data in tributeData )
        {
            IActor actor = new Tribute( data, new Coord( width / 2, height / 2 ) );
            _actors.Add( actor );
        }

        this.messageCenter = messageCenter;
    }
    
    public void Simulate()
    {
        messageCenter.ClearMessages();

        messageCenter.AddMessage( $"Day: {Day}" );

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
