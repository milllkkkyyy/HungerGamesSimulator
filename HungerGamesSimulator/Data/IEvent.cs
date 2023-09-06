namespace HungerGamesSimulator.Data;

public interface IEvent
{
    public List<IActor> Simulate(List<IActor> actors);
}