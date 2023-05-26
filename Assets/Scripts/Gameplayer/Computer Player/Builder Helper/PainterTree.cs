using Ramsey.Board;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Ramsey.Utilities;
using System.Linq;

namespace Ramsey.Gameplayer
{
    public class PainterTree //only 4 const loop lengths.. which a lot of strats dont have so should make a thing to deal with that
    {

        List<int> movesLoop;
        int maxLoop;
        bool loop;

        public PainterTree(int maxLoop, Func<GameState, BuilderMove>[] subStrategies, bool loop = true)
        {
            this.maxLoop = maxLoop;
            movesLoop = new();
            this.loop = loop;

            this.subStrategies = new Func<GameState, BuilderMove>[(int) Enumerable.Range(1, maxLoop).Select(math.ceilpow2).Sum()];
            subStrategies.CopyTo(this.subStrategies, 0);
        }

        public BuilderMove? Run(GameState gameState)
        {
            if (movesLoop.Count == maxLoop) if (loop) movesLoop.Clear(); else return null;
            movesLoop.Add(gameState.NewestPaint);

            int bfore = (int)Enumerable.Range(1, movesLoop.Count - 1).Select(math.ceilpow2).Sum();
            return (subStrategies[bfore + movesLoop.ToDecimal(2)] ?? subStrategies[bfore + movesLoop.Select(k => 1 - k).ToDecimal(2)]).Invoke(gameState);
        }

        Func<GameState, BuilderMove>[] subStrategies;

    }
}
