using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Ramsey.Utilities;
using UnityEditor;
using UnityEngine;

namespace Ramsey.Graph
{
    internal static class PathFinderAlgorithms
    {
        public const int NodeCount = 14;

        internal static Task Time(Func<Task> task, string name)
        {
            return Task.Run(async delegate 
            {
                var sw = new Stopwatch();
                sw.Start();

                await task();

                sw.Stop();

                UnityEngine.Debug.Log(name + " took " +  sw.ElapsedMilliseconds / 1000.0f + " seconds to complete its test!");
            });
        }

        internal static Task Perform<TAlgo>() where TAlgo : IIncrementalPathFinder, new()
            => Time(TestAlgo<TAlgo>, typeof(TAlgo).Name);


        // TODO: this was fully mutilated in order to test the job path finder!
        internal static async Task TestAlgo<TAlgo>() where TAlgo : IIncrementalPathFinder, new()
        {
            var graph = GraphManager.UsingAlgorithm<TAlgo>();

            Edge e = null;
            
            for(var i = 0; i < NodeCount; i++) graph.CreateNode();
            for(var i = 0; i < NodeCount; i++)
            {
                for(var j = i + 1; j < NodeCount; j++)
                {
                    if(e != null) graph.graph.PaintEdge(e, 0);

                    e = graph.CreateEdge(
                        graph.NodeFromID(i), 
                        graph.NodeFromID(j)
                    );
                }
            }

            graph.PaintEdge(e, 0);
            await graph.AwaitPathTask();
        }

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        internal static void Test() 
        {
            Task.Run(async () => 
            {
                for(var i = 0; i < 1; i++)
                {
                    UnityEngine.Debug.Log("Starting new round of tests!");

                    await Task.WhenAll
                    ( 
                        // Perform<DefaultPathFinder>(), //,
                        Perform<Experimental.JobPathFinder>()
                    );
                }
            });
        }
    }
}