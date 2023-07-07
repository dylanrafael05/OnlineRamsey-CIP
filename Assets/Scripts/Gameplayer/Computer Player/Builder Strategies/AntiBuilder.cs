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

    public class AntiBuilder : Builder.Synchronous
    {

        //Will try to lose the game in an interesting way; most of the time, it'll extend the node located in the middle of the longest path
        public override BuilderMove GetMove(GameState state)
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.8f)
                return Extend(state.MaxPath.Middle, state);
            else
                return new BuilderMove(state.MaxPath.Middle, state.Nodes.RandomElement());
        }

        public override void Reset() { }

        static AntiBuilder()
            => StrategyInitializer.RegisterFor<AntiBuilder>();
    }

}