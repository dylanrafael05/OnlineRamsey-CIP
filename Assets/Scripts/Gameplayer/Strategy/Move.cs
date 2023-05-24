using Codice.CM.Client.Differences;
using Ramsey.Board;
using Ramsey.Graph;
using System.Threading.Tasks;

namespace Ramsey.Gameplayer
{
    public interface IMove
    {
        static bool Enable = true;
        bool MakeMove(BoardManager board);
    }

    public readonly struct BuilderMove : IMove
    {
        private readonly Node n1;
        private readonly Node n2;

        public BuilderMove(Node n1, Node n2)
        {
            this.n1 = n1;
            this.n2 = n2;
        }

        bool IsValid(BoardManager board)
            => IMove.Enable && n1 is not null && n2 is not null && board.IsValidEdge(n1, n2);

        public bool MakeMove(BoardManager board)
        {
            if (!IsValid(board))
                return false;

            board.CreateEdge(n1, n2);
            return true;
        }

    }

    public readonly struct PainterMove : IMove
    {
        private readonly Edge edge;
        private readonly int type;

        public PainterMove(Edge edge, int type)
        {
            this.edge = edge;
            this.type = type;
        }

        bool IsValid(BoardManager board)
            => IMove.Enable && this.edge.Type == Edge.NullType;

        public bool MakeMove(BoardManager board)
        {
            if (!IsValid(board))
                return false;

            board.PaintEdge(edge, type);
            return true;
        }
    }
}