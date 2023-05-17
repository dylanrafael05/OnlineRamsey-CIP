using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Ramsey.Core
{

    public class GameState
    {

        public static GameState CreateGameState(IGraphView graph, Path[] bestPaths) //or a strategy could request the best paths to be calculated if they havent for the current iteration
            => new GameState(); //then it'd like have a ref to an obj that can make it for it

        IGraphView graph;
        Path[] bestPaths;

        public int LongestPath(int type)
            => bestPaths[type].Length;

    }
    

}
