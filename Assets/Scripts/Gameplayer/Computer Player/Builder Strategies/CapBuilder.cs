using Ramsey.Board;
using Ramsey.Gameplayer;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Ramsey.Graph;
using System.Linq;
using System.Threading.Tasks;
using Ramsey.Utilities;

using static Ramsey.Gameplayer.BuilderUtils;

public class CapBuilder : Builder
{
    public CapBuilder(GameState state)
        => seq = new(InitialTree(state), LoopTree(state));

    SequenceNavigator<BuilderMove> seq;

    public override Task<BuilderMove> GetMove(GameState state)
       => Task.FromResult(seq.Loop());

    public override void Reset()
    {
        t1 = 0;

        n1 = null;
        n2 = null;

        longBase = 0;
        longOther = 0;

        seq.Reset();
    }

    int t1;
    Node n1 = null;
    Node n2 = null;
    
    int longBase;
    int longOther;

    IEnumerable<BuilderMove> InitialTree(GameState state)
    {
        Node n = state.CreateNode();
        Node initial = n;

        yield return Extend(ref n, state);

        var b = state.NewestPaint;

        while (state.NewestPaint == b) { n1 = n; longBase++; yield return Extend(ref n, state); }

        yield return Extend(ref n1, state);
        t1 = state.NewestPaint;

        if (t1 == b) { n2 = n; longBase++; longOther = 1; }
        else { n2 = initial; longOther = longBase; longBase = 2; }
    }

    IEnumerable<BuilderMove> LoopTree(GameState state) // 5/14 strategy needs an init to make a longer path
    {
        var oldOther = n2.Neighbors.First();

        if (longOther == 0) n2 = state.CreateNode();

        yield return new BuilderMove(n1, n2);

        if (state.NewestPaint != t1)
        {
            longOther++;

            t1 = 1 - t1;
            Utils.Swap(ref n1, ref n2);

            Utils.Swap(ref longBase, ref longOther);
        }

        yield return Extend(ref n2, state);
        if(state.NewestPaint == t1)
        {
            longBase++;

            n1 = n2;
            n2 = oldOther;

            longOther--;
        }
        else { longBase--; longOther++; }
            
    }

}
