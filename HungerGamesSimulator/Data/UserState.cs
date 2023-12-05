namespace HungerGamesSimulator.Data
{
    public class UserState
    {
        public SimulationState CurrentState { get; set; } = SimulationState.Empty;

        public SimulationService? ActiveSimulation { get; set; }

        public List<IActor> Tributes { get; } = new List<IActor>();

        public bool Reset { get; set; } = false;
    }
}
