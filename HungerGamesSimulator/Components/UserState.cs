using HungerGamesSimulator.Data;
using Microsoft.AspNetCore.Components;

namespace HungerGamesSimulator.Components
{
  public class UserState
  {
    public SimulationState CurrentState { get; set; }

    public SimulationService ActiveSimulation { get; }

    public UserState( SimulationState simulationState, SimulationService simulationService )
    {
      CurrentState = simulationState;
      ActiveSimulation = simulationService;
    }
  }
}
