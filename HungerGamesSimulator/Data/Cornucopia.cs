namespace HungerGamesSimulator.Data;

public class Cornucopia : IEvent
{ 
    public List<IActor> Simulate(List<IActor> actors)
    {
        foreach (var actor in actors)
        {
            MessageCenter.AddMessage( $"{actor.Name} participated in the cornucopia" );
        }

        return actors;
    }
}