using System.Diagnostics.CodeAnalysis;

namespace HungerGamesSimulator.Data
{
    public enum MemoryType
    {
        None,
        Good,
        Bad,
        Ally,
        Threat
    }

    public class MemoryService
    {
        private class MemoryBank
        {
            private Dictionary<Guid, Stack<MemoryType>> _memory = new();

            public void AddMemory(Guid playerId, MemoryType memoryType)
            {
                if (!_memory.ContainsKey(playerId))
                {
                    _memory[playerId] = new Stack<MemoryType>();
                }

                _memory[playerId].Push(memoryType);
            }

            public bool TryGetMemory(Guid guid, [MaybeNullWhen(returnValue: false)] out IReadOnlyList<MemoryType> memories )
            {
                if (_memory.TryGetValue(guid, out var actualMemory))
                {
                    memories = actualMemory.ToList();
                    return true;
                }
                memories = null;
                return false;
            }

            public MemoryType GetLastMemeory(Guid playerId )
            {
                if (_memory.TryGetValue(playerId, out var actualMemory))
                {
                    return actualMemory.Peek();
                }
                return MemoryType.None;
            }
        }

        private Dictionary<Guid, MemoryBank> _memoryData = new();

        public void AddActorMemory(Guid playerId, Guid otherId, MemoryType memoryType )
        {
            if (!_memoryData.ContainsKey(playerId))
            {
                _memoryData[playerId] = new MemoryBank();
            }

            _memoryData[playerId].AddMemory( otherId, memoryType);
        }

        public bool TryGetMemoryData(Guid playerId, Guid otherId, [MaybeNullWhen(returnValue: false)] out IReadOnlyList<MemoryType> memories)
        {
            if ( _memoryData.TryGetValue( playerId, out var memoryBank ) && memoryBank.TryGetMemory( otherId, out var actualMemory))
            {
                memories = actualMemory;
                return true;
            }
            memories = null;
            return false;
        }

        public MemoryType GetLastMemeory(Guid playerId, Guid otherId)
        {
            if (_memoryData.TryGetValue(playerId, out var memoryBank))
            {
                return memoryBank.GetLastMemeory(otherId);
            }
            return MemoryType.None;
        }
   
    }
}
