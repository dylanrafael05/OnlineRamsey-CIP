using Ramsey.Graph;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ramsey.Gameplayer
{
    public class BuilderStrategyCap : Builder
    {
        //Tree of substrategies for every
        //possible move that loops back on itself
        //each substrategy is different logic so instead of looking at the board with clean eyes everytime
        //the builder knows which move it made which means that the branch can only go into 2 possible directions

        public override Task<BuilderMove> GetMove(GameState gameState)
        {

            throw new System.NotImplementedException();

        }
    }
}
