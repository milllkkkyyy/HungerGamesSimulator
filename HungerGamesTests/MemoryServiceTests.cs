using HungerGamesSimulator.Data;

namespace HungerGamesTests;

public class MemoryServiceTests
{
    private readonly Guid _player1Guid = Guid.NewGuid();
    private readonly Guid _player2Guid = Guid.NewGuid();
    private readonly Guid _player3Guid = Guid.NewGuid();

    [Fact]
    public void Test_Class_Creation()
    {
        MemoryService _ = new(); 
    }

    [Fact]
    public void Test_NoActorMemory()
    {
        MemoryService memoryService = new();
        Assert.True(memoryService.GetLastMemeory(_player1Guid, _player2Guid) == MemoryType.None);
        memoryService.AddActorMemory(_player1Guid, _player2Guid, MemoryType.Good);
        Assert.True(memoryService.GetLastMemeory(_player2Guid, _player1Guid) == MemoryType.None);
    }

    [Fact]
    public void Test_AddActorMemory_TryGetMemoryData()
    {
        MemoryService memoryService = new();
        memoryService.AddActorMemory(_player1Guid, _player2Guid, MemoryType.Good );
        Assert.True(memoryService.TryGetMemoryData(_player1Guid, _player2Guid, out var mems));
        Assert.True(mems[0] == MemoryType.Good);
    }

    [Fact]
    public void Test_AddActorMemory_GetLastMemory()
    {
        MemoryService memoryService = new();
        memoryService.AddActorMemory(_player1Guid, _player2Guid, MemoryType.Good);
        Assert.True(memoryService.GetLastMemeory(_player1Guid, _player2Guid) == MemoryType.Good);
    }

    [Fact]
    public void Test_Multiple_AddActorMemory_GetLastMemory()
    {
        MemoryService memoryService = new();
        memoryService.AddActorMemory(_player1Guid, _player2Guid, MemoryType.Good);
        memoryService.AddActorMemory(_player1Guid, _player2Guid, MemoryType.Bad);
        memoryService.AddActorMemory(_player1Guid, _player3Guid, MemoryType.Good);
        Assert.True(memoryService.GetLastMemeory(_player1Guid, _player2Guid) == MemoryType.Bad);
        Assert.True(memoryService.TryGetMemoryData(_player1Guid, _player2Guid, out var mems) && mems.Count == 2);
        Assert.True(memoryService.GetLastMemeory(_player1Guid, _player3Guid) == MemoryType.Good);
    }
}
