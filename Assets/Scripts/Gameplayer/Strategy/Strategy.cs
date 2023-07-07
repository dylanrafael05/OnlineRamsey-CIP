using Ramsey.Board;
using Ramsey.Graph;
using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Ramsey.Gameplayer
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NonDeterministicStrategyAttribute : Attribute
    {}

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NonAutomatedStrategyAttribute : Attribute
    {}
    
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UnsupportedInHeadlessAttribute : Attribute
    {}

    public interface IPlayer
    {
        bool IsDeterministic { get; }
        bool IsAutomated { get; }
        bool IsSupportedInHeadless { get; }

        IMove GetMove(GameState gameState);

        void Reset();
    }

    public static class Player 
    {
        public static bool IsDeterminstic(Type type)
            => !Attribute.IsDefined(type, typeof(NonDeterministicStrategyAttribute)) || IsAutomated(type);
        public static bool IsDeterminstic<T>() where T : IPlayer
            => IsDeterminstic(typeof(T));
        
        public static bool IsAutomated(Type type)
            => !Attribute.IsDefined(type, typeof(NonAutomatedStrategyAttribute));
        public static bool IsAutomated<T>() where T : IPlayer
            => IsAutomated(typeof(T));
        
        public static bool IsSupportedInHeadless(Type type)
            => !Attribute.IsDefined(type, typeof(UnsupportedInHeadlessAttribute));
        public static bool IsSupportedInHeadless<T>() where T : IPlayer
            => IsSupportedInHeadless(typeof(T));

        public static bool IsBuilder(Type type)
            => typeof(Builder).IsAssignableFrom(type);
        public static bool IsBuilder<T>()
            => IsBuilder(typeof(T));
        
        public static bool IsPainter(Type type)
            => typeof(Painter).IsAssignableFrom(type);
        public static bool IsPainter<T>()
            => IsBuilder(typeof(T));
    }

    public abstract class Builder : IPlayer
    {
        public bool IsDeterministic { get; }
        public bool IsAutomated { get; }
        public bool IsSupportedInHeadless { get; }

        public Builder() 
        {
            IsDeterministic = Player.IsDeterminstic(GetType());
            IsAutomated = Player.IsAutomated(GetType());
            IsSupportedInHeadless = Player.IsSupportedInHeadless(GetType());
        }

        IMove IPlayer.GetMove(GameState gameState)
        {
            return GetMove(gameState);
        }

        public abstract BuilderMove GetMove(GameState gameState);
        public abstract void Reset();

        public abstract override string ToString();
    }

    public abstract class Painter : IPlayer
    {
        public bool IsDeterministic { get; }
        public bool IsAutomated { get; }
        public bool IsSupportedInHeadless { get; }

        public Painter() 
        {
            IsDeterministic = Player.IsDeterminstic(GetType());
            IsAutomated = Player.IsAutomated(GetType());
            IsSupportedInHeadless = Player.IsSupportedInHeadless(GetType());
        }

        IMove IPlayer.GetMove(GameState gameState)
        {
            return GetMove(gameState);
        }

        public abstract PainterMove GetMove(GameState gameState);
        public abstract void Reset();

        public abstract override string ToString();
    }
}