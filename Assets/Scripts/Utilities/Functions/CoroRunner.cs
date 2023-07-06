using UnityEngine;

namespace Ramsey.Utilities
{
    /// <summary>
    /// A simple mono behaviour used as a coroutine runner.
    /// </summary>
    internal class CoroRunner : MonoBehaviour
    {
        private static CoroRunner ins = null;
        public static CoroRunner Instance 
        {
            get 
            {
                if(ins is null)
                {
                    var go = new GameObject("CoroRunner");
                    ins = go.AddComponent<CoroRunner>();
                }

                return ins;
            }
        }
    }
}