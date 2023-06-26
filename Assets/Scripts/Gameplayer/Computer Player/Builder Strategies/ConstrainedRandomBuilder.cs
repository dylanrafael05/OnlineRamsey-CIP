using Ramsey.Board;
using Ramsey.Gameplayer;
using System.Collections.Generic;
using Ramsey.Graph;
using System.Threading.Tasks;
using Ramsey.Utilities;

[NonDeterministicStrategy]
public class ConstrainedRandomBuilder : Builder 
{
    private readonly int target;
    private readonly Dictionary<int, Node> nodes;
    private readonly List<(int, int)> availablePairs;

    public override void Reset()
    {
        nodes.Clear();
        availablePairs.Clear();

        for(int i = 0; i < target; i++)
        {
            for(int j = i; j < target; j++)
            {
                availablePairs.Add((i, j));
            }
        }
    }

    public ConstrainedRandomBuilder(int target)
    {
        this.target = target;

        availablePairs = new();
        nodes = new();

        Reset();
    }

    private Node GetNode(GameState state, int x) 
    {
        if(!nodes.ContainsKey(x))
        {
            nodes[x] = state.CreateNode();
        }

        return nodes[x];
    }

    public override BuilderMove GetMove(GameState state)
    {
        if(availablePairs.Count == 0)
            throw new GraphTooComplexException(state.Nodes.Count, $"{nameof(ConstrainedRandomBuilder)} of target {target} cannot expand graph any further!");

        var i = ThreadSafeRandom.Range(0, availablePairs.Count);

        var (a, b) = availablePairs[i];
        availablePairs.RemoveAt(i);

        return new BuilderMove(GetNode(state, a), GetNode(state, b));
    }
}