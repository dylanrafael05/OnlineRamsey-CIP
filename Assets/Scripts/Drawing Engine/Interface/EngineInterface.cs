public class EngineInterface
{

    //
    EngineDrawer drawer;

    public EngineInterface(EngineDrawer drawer) => this.drawer = drawer;

    //
    public void Draw()
    {

        //thikn baout when the buffers should be updated and how that should interact with graph computations
        //but not updaetd for now

        drawer.Draw();

    }

}