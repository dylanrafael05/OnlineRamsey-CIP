using Ramsey.Board;
using Ramsey.Graph;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Ramsey.Gameplayer
{
    public interface IPlayer
    {
        Task<IMove> GetMove(GameState gameState);
    }

    public abstract class Builder : IPlayer
    {
        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<BuilderMove> GetMove(GameState gameState);
    }

    public abstract class Painter : IPlayer
    {
        async Task<IMove> IPlayer.GetMove(GameState gameState)
        {
            return await GetMove(gameState);
        }

        public abstract Task<PainterMove> GetMove(GameState gameState);
    }
}