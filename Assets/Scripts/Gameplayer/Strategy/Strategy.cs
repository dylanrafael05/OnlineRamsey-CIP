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

    public interface IPlayer
    {
        bool IsDeterministic { get; }
        bool IsAutomated { get; }

        Task<IMove> GetMove(GameState gameState);

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
    }

    public abstract class Builder : IPlayer
    {
        public bool IsDeterministic { get; }
        public bool IsAutomated { get; }

        public Builder() 
        {
            IsDeterministic = Player.IsDeterminstic(GetType());
            IsAutomated = Player.IsAutomated(GetType());
        }

        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<BuilderMove> GetMove(GameState gameState);
        public abstract void Reset();
    }

    public abstract class Painter : IPlayer
    {
        public bool IsDeterministic { get; }
        public bool IsAutomated { get; }

        public Painter() 
        {
            IsDeterministic = Player.IsDeterminstic(GetType());
            IsAutomated = Player.IsAutomated(GetType());
        }

        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<PainterMove> GetMove(GameState gameState);
        public abstract void Reset();
    }
}