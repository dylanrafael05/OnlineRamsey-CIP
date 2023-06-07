using Ramsey.Board;
using Ramsey.Graph;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Ramsey.Gameplayer
{
    public interface IPlayer
    {
        public bool CanDelay { get; }

        Task<IMove> GetMove(GameState gameState);
    }

    public abstract class Builder : IPlayer
    {
        public virtual bool CanDelay => true;
        public Builder() { }

        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<BuilderMove> GetMove(GameState gameState);
    }

    public abstract class Painter : IPlayer
    {
        public virtual bool CanDelay => true;
        public Painter() { }

        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<PainterMove> GetMove(GameState gameState);
    }
}