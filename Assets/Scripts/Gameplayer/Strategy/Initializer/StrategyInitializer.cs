using System;
using System.Collections.Generic;
using System.Linq;
using Ramsey.Gameplayer;

using static Unity.Mathematics.math;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Ramsey.Utilities;
using Ramsey.Utilities.UI;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ramsey.Gameplayer
{
    public sealed class StrategyInitializer<T> : IStrategyInitializer<T> where T : IPlayer
    {
        public IReadOnlyList<TextParameter> Parameters { get; }
        public Func<object[], T> Construct { get; }
        
        public string Name { get; set; }

        public bool IsDeterministic { get; } = Player.IsDeterminstic(typeof(T));
        public bool IsAutomated { get; } = Player.IsAutomated(typeof(T));

        public StrategyInitializer(Type constype, Func<object[], T> construct, params TextParameter[] parameters)
        {
            Parameters = parameters;
            Construct = construct;

            Name = string.Join(
                ' ', 
                Regex.Replace(constype.Name, @"[A-Z][a-z0-9]+", m => m.Value + " ")
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

        /// <summary>
        /// Register a strategy initializer with no parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterFor<T>() where T : IPlayer, new()
        {
            if(Player.IsBuilder<T>())
            {
                builderInits.Add(new StrategyInitializer<Builder>(typeof(T), p => new T() as Builder));
            }
            else 
            {
                painterInits.Add(new StrategyInitializer<Painter>(typeof(T), p => new T() as Painter));
            }
        }
        /// <summary>
        /// Register a strategy initializer with a function that creates the strategy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="construct"></param>
        public static void RegisterFor<T>(Func<T> construct) where T : IPlayer
        {
            if(Player.IsBuilder<T>())
            {
                builderInits.Add(new StrategyInitializer<Builder>(typeof(T), p => construct() as Builder));
            }
            else 
            {
                painterInits.Add(new StrategyInitializer<Painter>(typeof(T), p => construct() as Painter));
            }
        }
        /// <summary>
        /// Register a strategy initializer with a function that takes in an array of objects.  
        /// The array of "objects" is actually filled with the types that you set in the "parameters" parameter.
        /// For example, if you have two ints in your "parameters" parameter, you'll cast the first and second element of object array to int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="construct"></param>
        /// <param name="parameters"></param>
        public static void RegisterFor<T>(Func<object[], T> construct, params TextParameter[] parameters) where T : IPlayer
        {
            if(Player.IsBuilder<T>())
            {
                builderInits.Add(new StrategyInitializer<Builder>(typeof(T), p => construct(p) as Builder, parameters));
            }
            else 
            {
                painterInits.Add(new StrategyInitializer<Painter>(typeof(T), p => construct(p) as Painter, parameters));
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void RunStaticConstructors()
        {
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(var type in assembly.GetTypes().Where(typeof(IPlayer).IsAssignableFrom))
                {
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
            }
        }
    }
}
