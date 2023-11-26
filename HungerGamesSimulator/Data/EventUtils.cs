namespace HungerGamesSimulator.Data
{
    public static class EventUtils
    {
        public static IActor GetRandomActor(IReadOnlyList<IActor> actors, IActor? toIgnore = null)
        {
            var aliveActors = actors.Where(actor => actor.Health > 0).Where(actor => actor != toIgnore).ToList();
            var otherActor = aliveActors[Random.Shared.Next(aliveActors.Count)];
            return otherActor;
        }
    }
}
