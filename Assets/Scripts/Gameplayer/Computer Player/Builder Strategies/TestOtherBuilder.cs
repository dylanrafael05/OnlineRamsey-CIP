using Ramsey.Board;
using Ramsey.Gameplayer;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Ramsey.Graph;
using System.Linq;
using System.Threading.Tasks;
using Ramsey.Utilities;

public class TestOtherBuilder : Builder
{

    public TestOtherBuilder(GameState state)
        => sequenceNavigator = new(new List<IEnumerable<BuilderMove>>() { InitialTree(state), LoopTree(state) });

    SequenceNavigator<BuilderMove> sequenceNavigator;

    public override Task<BuilderMove> GetMove(GameState state)
       => Task.FromResult(sequenceNavigator.Loop());

    BuilderMove Extend(Node n1, GameState state)
    {
        var n2 = state.Board.CreateNode();
        var move = new BuilderMove(n1, n2);
        n1 = n2;
        return move;
    }

    int t1;
    Node n1 = null;
    Node n2 = null;

    IEnumerable<BuilderMove> InitialTree(GameState state)
    {
        Node n = state.Board.CreateNode();
        Node initial = n;

        yield return Extend(n, state);

        var b = state.NewestPaint;

        while (state.NewestPaint == b) { n1 = n; yield return Extend(n, state); }

        yield return Extend(n1, state);
        t1 = state.NewestPaint;

        if (state.NewestPaint == b) n2 = n;
        else n2 = initial;

    }

    IEnumerable<BuilderMove> LoopTree(GameState state) // 5/14 strategy needs an init to make a longer path
    {
        var oldOther = n2.Neighbors.First();

        yield return new BuilderMove(n1, n2);

        if (state.NewestPaint != t1)
        {
            t1 = 1 - t1;
            Node n1t = n1;
            n1 = n2;
            n2 = n1t;
        }

        yield return Extend(n2, state);
        if(state.NewestPaint == t1)
        {
            n1 = n2;
            n2 = oldOther;
        }
            
    }

}
