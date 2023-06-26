using Ramsey.Board;
using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ramsey.Gameplayer.BuilderUtils;

namespace Ramsey.Gameplayer
{

    public class AntiBuilder : Builder
    {

        public override Task<BuilderMove> GetMove(GameState state)
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.8f) //not a param since it should be whatever parameter makes it a bad builder (without making it impossible to win)
                return Task.FromResult(Extend(state.MaxPath.Middle, state));
            else
                return Task.FromResult(new BuilderMove(state.MaxPath.Middle, state.Nodes.RandomElement()));
        }

        public override void Reset() { }

    }

}