using Ramsey.Board;
using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;

using static Ramsey.Gameplayer.BuilderUtils;

namespace Ramsey.Gameplayer
{

    public class AntiBuilder : Builder.Synchronous
    {

        //Will try to lose the game in an interesting way; most of the time, it'll extend the node located in the middle of the longest path
        //This is basically the Random Builder but with horrible choices, feel free to make the hardcoded probabilities parameters just like we did in the Random Builder
        public override BuilderMove GetMove(GameState state)
        {
            var mid = state.MaxPath != null ? state.MaxPath.Middle : state.CreateNode();

            var r = UnityEngine.Random.Range(0f, 1f);
            if (r < 0.42f)
                return Extend(mid, state);
            else if (r < 0.62f)
                return RandomPair(state);
            else
                return new BuilderMove(mid, RandomNode(mid, state));
        }

        public override void Reset() { }

        static AntiBuilder()
            => StrategyInitializer.RegisterFor<AntiBuilder>();

        public override string GetStrategyName(bool compact)
            => "Anti";
    }

}