using System.Text.Json.Serialization;

namespace HungerGamesSimulator.MessageCenter
{
    public enum ContextType
    {
        Combat,
        Flavor,
        Death,
        Cannon,
        PartySearchFail,
        CombatSearchFail,
        CornucopiaRunAway,
        CornucopiaOutpace,
        CornucopiaLuck,
        BurnMapSuicide,
        BurnMapFail,
        BurnMapSucceed,
        BurnMapIgnore,
        SuddenDeathWinner,
        PartyJoin,
        PartyLeave,
        PartyJoinFail,
        Move
    }
}
