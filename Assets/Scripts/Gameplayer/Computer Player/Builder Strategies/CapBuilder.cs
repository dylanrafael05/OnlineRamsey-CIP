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
using UnityEngine;

public class CapBuilder : Builder.Synchronous
{
    public CapBuilder()
        => seq = new(InitialTree, LoopTree);

    readonly SequenceNavigator<BuilderMove> seq;

    public override BuilderMove GetMove(GameState state)
       => seq.Loop(state);

    public override void Reset()
    {
        t1 = 0;

        n1 = null;
        n2 = null;

        longBase = 0;
        longOther = 0;

        seq.Reset();
    }

    int t1; //Color of the base path
    Node n1 = null; //End node of base path
    Node n2 = null; //End node of other path
    
    int longBase; //Length of longest base color path
    int longOther; //Length of longest other color path

    IEnumerable<BuilderMove> InitialTree(GameState state)
    {
        //Create Initial Node
        Node n = state.CreateNode();
        Node initial = n;

        //Extend the initial node
        yield return Extend(ref n, state);

        //Store the first painter move (the base color)
        var b = state.NewestPaint;

        //While the painter keeps painting the same color, keep extending the path
        while (state.NewestPaint == b) { n1 = n; longBase++; yield return Extend(ref n, state); }

        //We now have a path of color 'b' and an edge at the end of opposite color; extend from the intersection of the edges of different colors
        yield return Extend(ref n1, state);
        t1 = state.NewestPaint;

        //Depending on what the painter just did, choose our longest red and blue paths accordingly (designated by their end nodes 'n1' and 'n2')
        //and store their lengths in longBase and longOther
        if (t1 == b) { n2 = n; longBase++; longOther = 1; }
        else { n2 = initial; longOther = longBase; longBase = 2; }
    }

    IEnumerable<BuilderMove> LoopTree(GameState state)
    {

        var oldBase = n1.Neighbors.First();
        var oldOther = n2.Neighbors.First();

        //If the 'other' path has length 0 give it a node to start with
        //if (longOther == 0) n2 = state.CreateNode();

        //Connect the ends of the two paths (we'll call this the bridge edge from now on)
        yield return new BuilderMove(n1, n2);

        //Extend the path that was just extended by painter's paint
        if (state.NewestPaint != t1)
        {
            longOther++;

            t1 = 1 - t1;

            Utils.Swap(ref n1, ref n2);
            Utils.Swap(ref oldBase, ref oldOther);
            Utils.Swap(ref longBase, ref longOther);
        }
        yield return Extend(ref n2, state);

        //If the newest paint is the same color as that just extended path, increment our longest base path length and update the end points of our paths
        Debug.Log("T1: " + t1 + " Newest Paint: " + state.NewestPaint);
        if(state.NewestPaint == t1)
        {
            longBase++;

            n1 = n2;
            n2 = oldOther;

            longOther--;
        }
        //if the newest paint isn't the same color as that just extended path, imagine the bridge edge doesn't exist (longBase--) and increment the longOther path length
        else { longBase--; longOther++; }
            
    }
    
    static CapBuilder()
        => StrategyInitializer.RegisterFor<CapBuilder>();

    public override string ToString()
            => "Cap Builder";
}
