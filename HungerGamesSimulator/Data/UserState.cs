namespace HungerGamesSimulator.Data
{
    public class UserState
    {
        public SimulationState CurrentState { get; set; } = SimulationState.Empty;

        public SimulationService? ActiveSimulation { get; set; }

        public IMessageCenter? MessageCenter { get; set; }

        public List<IActor> Tributes { get; } = new List<IActor>() { new Tribute("Katniss Everdeen"), new Tribute( "Peeta Mellark" ), new Tribute("Thresh"), new Tribute("Rue"), new Tribute("Foxface"), new Tribute("Cato") , new Tribute("Clove") } ;

        public bool Reset { get; set; } = false;
    }
}
