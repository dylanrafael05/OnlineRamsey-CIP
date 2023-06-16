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
    public interface IPlayer
    {
        bool IsDeterministic { get; }
        bool IsAutomated { get; }

        Task<IMove> GetMove(GameState gameState);

        void Reset();
    }

    public abstract class Builder : IPlayer
    {
        public virtual bool IsDeterministic => true;
        public virtual bool IsAutomated => true;

        public Builder() { }

        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<BuilderMove> GetMove(GameState gameState);
        public abstract void Reset();
    }

    public abstract class Painter : IPlayer
    {
        public virtual bool IsDeterministic => true;
        public virtual bool IsAutomated => true;

        public Painter() { }

        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<PainterMove> GetMove(GameState gameState);
        public abstract void Reset();
    }
}