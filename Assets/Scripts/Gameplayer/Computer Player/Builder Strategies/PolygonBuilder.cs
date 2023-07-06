using Ramsey.Board;
using Ramsey.Graph;
using Ramsey.Utilities;
using Ramsey.Utilities.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static Ramsey.Gameplayer.BuilderUtils;

namespace Ramsey.Gameplayer
{
    [NonDeterministicStrategy]
    public class PolygonBuilder : Builder
    {
        public PolygonBuilder(int sideCount)
        {
            this.sideCount = sideCount;
            sequenceNavigator = new(LoopTree);
        }

        SequenceNavigator<BuilderMove> sequenceNavigator;
        int sideCount;

        Node startNode;

        public override BuilderMove GetMove(GameState gameState)
            => sequenceNavigator.Loop(gameState);

        IEnumerable<BuilderMove> LoopTree(GameState state)
        {

            startNode ??= state.CreateNode();
            var returnNode = startNode;
            var currentNode = startNode;

            int nextStartNode = ThreadSafeRandom.Range(0, sideCount);
            for (int i = 0; i < sideCount-1; i++)
            {
                if (i == nextStartNode) startNode = currentNode;
                yield return Extend(ref currentNode, state);
            }

            if (nextStartNode == sideCount - 1) startNode = currentNode;
            yield return new(currentNode, returnNode);

        }

        public override void Reset()
        {
            startNode = null;
            sequenceNavigator.Reset();
        }

        static PolygonBuilder()
            => StrategyInitializer.RegisterFor<PolygonBuilder>(
                p => new((int)p[0]), 
                new TextParameter 
                { 
                    Name = "Side Count", 
                    Verifier = new IInputVerifier.Integer(3), 
                    DefaultValue = "8"
                }
            );
    }
}
