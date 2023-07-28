using Ramsey.Graph;
using Ramsey.Board;
using Ramsey.Gameplayer;
using Cysharp.Threading.Tasks;

using static Ramsey.Gameplayer.BuilderUtils;
using System.Collections.Generic;
using System.Linq;
using Ramsey.Utilities;
using Ramsey.Utilities.UI;

namespace Ramsey.Gameplayer
{
    [NonDeterministicStrategy]
    public class RandomBuilder : Builder.Synchronous
    {
        //Will randomly choose between 3 substrategies: placing a pendant, placing an internal edge (between 2 nodes that have neighbors), and placing an isolated edge.
        float pendantProb;
        float internalProb;
        float isolatedProb;

        public RandomBuilder(float pendantW, float internalW, float isolatedW)
        { float W = pendantW + internalW + isolatedW; this.pendantProb = pendantW / W; this.internalProb = internalW / W; this.isolatedProb = isolatedW / W; }

        //Create a pendant move
        BuilderMove RandomPendant(GameState state)
        { var n = RandomNode(null, state); return Extend(ref n, state); }

        //Create an internal edge move
        BuilderMove RandomInternal(GameState state)
        {
            var n1 = RandomNode(state);
            var n2 = RandomNode(n1, state);
            return new(n1, n2);
        }

        //Create an isolated edge move
        BuilderMove RandomIsolated(GameState state)
            => new(state.CreateNode(), state.CreateNode());

        public override BuilderMove GetMove(GameState state)
        {
            //Generate a random number and choose the substrategy based on the number generated and probability parameters
            float r = UnityEngine.Random.Range(0f, 1f);
            if (r <= pendantProb)
                return RandomPendant(state);
            if (r <= pendantProb + internalProb)
                return RandomInternal(state);
            return RandomIsolated(state);
        }

        public override void Reset() { }

        static RandomBuilder()
            => StrategyInitializer.RegisterFor<RandomBuilder>(
                p => new((float)p[0], (float)p[1], (float)p[2]),
                new TextParameter { Name = "Pendant Weight", Verifier = new IInputVerifier.Float(0), DefaultValue = "0.5" },
                new TextParameter { Name = "Internal Weight", Verifier = new IInputVerifier.Float(0), DefaultValue = "0.4" },
                new TextParameter { Name = "Isolated Weight", Verifier = new IInputVerifier.Float(0), DefaultValue = "0.1" }
        );

        public override string GetStrategyName(bool compact)
            => compact ? $"Random({pendantProb}; {internalProb}; {isolatedProb})"
                       : $"Random(Pendant Prob = {pendantProb}; Internal Prob = {internalProb}; Isolated Prob = {isolatedProb})";
    }
}


