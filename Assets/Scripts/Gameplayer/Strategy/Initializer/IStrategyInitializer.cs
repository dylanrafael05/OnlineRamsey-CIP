using System.Collections.Generic;
using Unity.Mathematics;
using Ramsey.Gameplayer;
using Ramsey.Utilities.UI;

namespace Ramsey.Gameplayer
{
    public interface IStrategyInitializer<out T> where T : IPlayer
    {
        IReadOnlyList<TextParameter> Parameters { get; }

        T Initialize(object[] parameters);

        string Name { get; }

        bool IsDeterministic { get; }
        bool IsAutomated { get; }
    }
}
