using Ramsey.Graph;
using Ramsey.Board;
using Ramsey.Gameplayer;
using System.Threading.Tasks;

using static Ramsey.Gameplayer.BuilderUtils;
using Rand = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;

public class RandomBuilder : Builder
{
    public override bool IsDeterministic => false;

    float pendantProb;
    float internalProb;
    float isolatedProb;

    public RandomBuilder(float pendantW, float internalW, float isolatedW)
    { float W = pendantW + internalW + isolatedW; this.pendantProb = pendantW/W; this.internalProb = internalW/W; this.isolatedProb = isolatedW/W; }

    Node RandomNode(Node other, GameState state)
    { 
        IReadOnlyList<Node> nodes = other == null ? state.Nodes : state.Nodes.Where(n => !n.Neighbors.Contains(other) && other != n).ToList();
        if (nodes.Count == 0) { state.CreateNode(); return RandomNode(other, state); }
        return nodes[Rand.Range(0, nodes.Count)];
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

    public override Task<BuilderMove> GetMove(GameState state)
    {
        float r = Rand.Range(0f, 1f);
        if (r <= pendantProb)
            return Task.FromResult(RandomPendant(state));
        if (r <= pendantProb + internalProb)
            return Task.FromResult(RandomInternal(state));
        return Task.FromResult(RandomIsolated(state));
    }


}