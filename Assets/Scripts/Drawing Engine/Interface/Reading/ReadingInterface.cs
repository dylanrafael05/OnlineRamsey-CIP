public class ReadingInterface
{

    //
    readonly EngineDrawer drawer;

    public ReadingInterface(EngineDrawer drawer) => this.drawer = drawer;

    //
    public void Draw()
    {
        drawer.Draw();
    }

    public void Cleanup()
        => drawer.Cleanup();

}