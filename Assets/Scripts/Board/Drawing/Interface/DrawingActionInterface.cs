namespace Ramsey.Drawing
{
    public class DrawingActionInterface
    {

        //
        readonly Drawer drawer;

        internal DrawingActionInterface(Drawer drawer) => this.drawer = drawer;

        //
        public void Draw()
        {
            drawer.Draw();
        }

        public void Cleanup()
            => drawer.Cleanup();

    }
}