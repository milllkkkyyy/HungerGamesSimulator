namespace HungerGamesSimulator.Data;

public class GameService
{
    private List<IActor> _tributes = new List<IActor>();
    
    public void AddTribute( IActor tribute )
    {
        _tributes.Add( tribute );
    }

    public List<IActor> GetTributes()
    {
        return _tributes;
    }

    public Task RemoveTribute( IActor tribute )
    {
        _tributes.Remove(tribute);

        return Task.CompletedTask;
    }

    public void ClearTributes()
    {
        _tributes.Clear();
    }
}