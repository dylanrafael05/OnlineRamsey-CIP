using Ramsey.Board;
using Ramsey.Graph;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Ramsey.Gameplayer
{
    public interface IBuilder
    {
        Task<BuilderMove> GetMove(GameState gameState);
    }

    public interface IPainter
    {
        Task<PainterMove> GetMove(GameState gameState);
    }
}