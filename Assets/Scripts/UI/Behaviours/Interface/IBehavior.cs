namespace Ramsey.UI
{
    public interface IBehavior 
    {
        public static void SwitchTo(IBehavior behavior)
        {
            Active?.OnExit();
            behavior.OnEnter();

            Active = behavior;
        }

        public static void Cleanup()
        {
            Active?.OnExit();
        }

        static IBehavior Active { get; private set; }

        void Loop(InputData input); 

        void OnEnter();
        void OnExit();
    }
}