namespace HungerGamesSimulator.Data
{
    public static class EventUtils
    {
        public static IActor? GetRandomActor(IReadOnlyList<IActor> actors, IActor? toIgnore = null)
        {
            var aliveActors = actors.Where(actor => actor.Health > 0).Where(actor => actor != toIgnore).ToList();
            return  aliveActors != null && aliveActors.Count > 0 ? aliveActors[Random.Shared.Next(aliveActors.Count)] : null;
        }
    }
}
