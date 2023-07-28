using Ramsey.Board;
using Ramsey.Utilities;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ramsey.Gameplayer
{
    //Will choose a random color to paint the edge
    [NonDeterministicStrategy]
    public class RandomPainter : Painter.Synchronous
    {
        public override PainterMove GetMove(GameState gameState)
        {
            return new(gameState.NewestEdge, UnityEngine.Random.Range(0, 2));
        }
        
        public override void Reset() {}

        static RandomPainter()
            => Utils.ForLength(2, (i) => StrategyInitializer.RegisterFor<RandomPainter>()); //Duplicate to look nice - if you don't care, just change from 2 to 1

        public override string GetStrategyName(bool compact)
            => "Random";
    }
}
