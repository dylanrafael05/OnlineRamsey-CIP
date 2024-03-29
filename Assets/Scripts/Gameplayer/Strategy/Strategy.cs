using Ramsey.Board;
using Ramsey.Graph;
using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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

        UniTask<IMove> GetMoveAsync(GameState gameState, CancellationToss cancel = null);

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

        async UniTask<IMove> IPlayer.GetMoveAsync(GameState gameState, CancellationToss cancel)
        {
            return await GetMoveAsync(gameState);
        }

        public abstract UniTask<BuilderMove> GetMoveAsync(GameState gameState, CancellationToss cancel = null);
        public abstract void Reset();

        public abstract string GetStrategyName(bool compact);

        public abstract class Synchronous : Builder
        {
            public sealed override UniTask<BuilderMove> GetMoveAsync(GameState gameState, CancellationToss cancel)
                => UniTask.FromResult(GetMove(gameState));
            
            public abstract BuilderMove GetMove(GameState gameState);
        }

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

        async UniTask<IMove> IPlayer.GetMoveAsync(GameState gameState, CancellationToss cancel)
        {
            return await GetMoveAsync(gameState, cancel);
        }

        public abstract UniTask<PainterMove> GetMoveAsync(GameState gameState, CancellationToss cancel = null);
        public abstract void Reset();

        public abstract string GetStrategyName(bool compact);

        public abstract class Synchronous : Painter
        {
            public sealed override UniTask<PainterMove> GetMoveAsync(GameState gameState, CancellationToss cancel)
                => UniTask.FromResult(GetMove(gameState));
            
            public abstract PainterMove GetMove(GameState gameState);
        }
    }
}