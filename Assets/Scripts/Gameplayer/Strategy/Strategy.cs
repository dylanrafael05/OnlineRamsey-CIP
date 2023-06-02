using Ramsey.Board;
using Ramsey.Graph;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Ramsey.Gameplayer
{
    public interface IPlayer
    {
        public float Delay { get; }

        Task<IMove> GetMove(GameState gameState);
    }

    public abstract class Builder : IPlayer
    {
        public float Delay { get; private set; } = 1f;
        public Builder() { }
        public Builder(float Delay) => this.Delay = Delay;

        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<BuilderMove> GetMove(GameState gameState);
    }

    public abstract class Painter : IPlayer
    {
        public float Delay { get; private set; } = 1f;
        public Painter() { }
        public Painter(float Delay) => this.Delay = Delay;

        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<PainterMove> GetMove(GameState gameState);
    }
}