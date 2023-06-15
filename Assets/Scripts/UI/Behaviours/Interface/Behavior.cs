namespace Ramsey.UI
{
    public abstract class Behavior : IBehavior
    {
        public abstract void Loop(InputData input);

        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        public virtual void OnCleanup() {}
    }
}