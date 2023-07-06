using Ramsey.Graph;
using Ramsey.Board;
using Ramsey.Gameplayer;
using System.Threading.Tasks;

using static Ramsey.Gameplayer.BuilderUtils;
using System.Collections.Generic;
using System.Linq;
using Ramsey.Utilities;
using Ramsey.Utilities.UI;

namespace Ramsey.Gameplayer
{
    [NonDeterministicStrategy]
    public class RandomBuilder : Builder
    {
        float pendantProb;
        float internalProb;
        float isolatedProb;

        public RandomBuilder(float pendantW, float internalW, float isolatedW)
        { float W = pendantW + internalW + isolatedW; this.pendantProb = pendantW/W; this.internalProb = internalW/W; this.isolatedProb = isolatedW/W; }

        Node RandomNode(Node other, GameState state)
        { 
            IReadOnlyList<Node> nodes = other == null ? state.Nodes : state.Nodes.Where(n => !n.Neighbors.Contains(other) && other != n).ToList();
            if (nodes.Count == 0) { state.CreateNode(); return RandomNode(other, state); }
            return nodes[ThreadSafeRandom.Range(0, nodes.Count)];
        }

        Node RandomNode(GameState state) => RandomNode(null, state);

        BuilderMove RandomPendant(GameState state)
        { var n = RandomNode(null, state); return Extend(ref n, state); }

        BuilderMove RandomInternal(GameState state)
        {
            var n1 = RandomNode(state);
            var n2 = RandomNode(n1, state);
            return new(n1, n2);
        }

        BuilderMove RandomIsolated(GameState state)
            => new(state.CreateNode(), state.CreateNode());

        public override BuilderMove GetMove(GameState state)
        {
            float r = ThreadSafeRandom.Range(0f, 1f);
            if (r <= pendantProb)
                return RandomPendant(state);
            if (r <= pendantProb + internalProb)
                return RandomInternal(state);
            return RandomIsolated(state);
        }

        public override void Reset() {}

        static RandomBuilder()
            => StrategyInitializer.RegisterFor<RandomBuilder>(
                p => new((float)p[0], (float)p[1], (float)p[2]), 
                new TextParameter { Name = "Pendant Weight",  Verifier = new IInputVerifier.Float(0, 1), DefaultValue = "0.5" },
                new TextParameter { Name = "Internal Weight", Verifier = new IInputVerifier.Float(0, 1), DefaultValue = "0.4" },
                new TextParameter { Name = "Isolated Weight", Verifier = new IInputVerifier.Float(0, 1), DefaultValue = "0.1" }
            );
    }
}