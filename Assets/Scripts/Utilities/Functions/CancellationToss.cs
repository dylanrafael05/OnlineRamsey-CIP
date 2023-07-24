namespace Ramsey.Utilities
{
    public class CancellationToss
    {
        private bool shouldCancel;

        public void Retract() => shouldCancel = false;
        public void Request() => shouldCancel = true;
        public bool IsRequested => shouldCancel;
    }
}