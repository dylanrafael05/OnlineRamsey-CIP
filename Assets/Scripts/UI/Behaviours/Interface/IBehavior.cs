using System.Collections;
using Ramsey.Utilities;

namespace Ramsey.UI
{
    public interface IBehavior 
    {
        public static void SwitchTo(IBehavior behavior)
        {
            IEnumerator Coro() 
            {
                if(Active != null)
                {
                    yield return Transition.HideScreen();
                    Active.OnExit();
                }

                behavior.OnEnter();
                Active = behavior;
                
                yield return Transition.ShowScreen();
            }
            
            Coroutines.Start(Coro);
        }

        public static void Cleanup()
        {
            Active?.OnCleanup();
        }

        static IBehavior Active { get; private set; }

        void Loop(InputData input); 

        void OnEnter();
        void OnExit();
        void OnCleanup();
    }
}