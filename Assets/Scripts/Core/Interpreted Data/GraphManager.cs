using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ramsey.Core
{
    
    public class GraphManager
    {

        //
        public IGraphView Graph => graph;

        //
        Graph graph;
        IncrementalPathFinder pathFinder;
        GameState gameState;

        //
        void ChangedGraph() { }

        //implement methods for changing graph or just give outside access to Graph so they can change from there
    }

}
