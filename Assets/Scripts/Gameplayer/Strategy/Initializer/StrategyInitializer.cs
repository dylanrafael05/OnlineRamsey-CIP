using System;
using System.Collections.Generic;
using System.Linq;
using Ramsey.Gameplayer;

using static Unity.Mathematics.math;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Ramsey.Utilities;
using Ramsey.Utilities.UI;

namespace Ramsey.Gameplayer
{
    public sealed class StrategyInitializer<T> : IStrategyInitializer<T> where T : IPlayer
    {
        public IReadOnlyList<TextParameter> Parameters { get; }
        public Func<object[], T> Construct { get; }
        
        public string Name { get; set; }

        public bool IsDeterministic { get; } = Player.IsDeterminstic(typeof(T));
        public bool IsAutomated { get; } = Player.IsAutomated(typeof(T));

        public StrategyInitializer(Func<object[], T> construct, params TextParameter[] parameters)
        {
            Parameters = parameters;
            Construct = construct;

            Name = string.Join(
                ' ', 
                Regex.Replace(typeof(T).Name, @"[A-Z][a-z0-9]+", m => m.Value + " ")
                    .TrimEnd()
                    .Split(' ')
                    .SkipLast(1)
            );
        }

        public T Initialize(object[] objects)
            => Construct(objects);
    }
    
    public static class StrategyInitializer
    {
        private static readonly List<IStrategyInitializer<Builder>> builderInits = new();
        private static readonly List<IStrategyInitializer<Painter>> painterInits = new();

        public static IReadOnlyList<IStrategyInitializer<Builder>> BuilderInitializers => builderInits; 
        public static IReadOnlyList<IStrategyInitializer<Painter>> PainterInitializers => painterInits; 

        public static void RegisterFor<T>() where T : IPlayer, new()
        {
            if(Player.IsBuilder<T>())
            {
                builderInits.Add(new StrategyInitializer<Builder>(p => new T() as Builder));
            }
            else 
            {
                painterInits.Add(new StrategyInitializer<Painter>(p => new T() as Painter));
            }
        }
        public static void RegisterFor<T>(Func<T> construct) where T : IPlayer
        {
            if(Player.IsBuilder<T>())
            {
                builderInits.Add(new StrategyInitializer<Builder>(p => construct() as Builder));
            }
            else 
            {
                painterInits.Add(new StrategyInitializer<Painter>(p => construct() as Painter));
            }
        }
        public static void RegisterFor<T>(Func<object[], T> construct, params TextParameter[] parameters) where T : IPlayer
        {
            if(Player.IsBuilder<T>())
            {
                builderInits.Add(new StrategyInitializer<Builder>(p => construct(p) as Builder, parameters));
            }
            else 
            {
                painterInits.Add(new StrategyInitializer<Painter>(p => construct(p) as Painter, parameters));
            }
        }
    }
}
