using System;
using SimpleFileBrowser;
using static SimpleFileBrowser.FileBrowser;

namespace Ramsey.Utilities
{

    //https://github.com/yasirkula/UnitySimpleFileBrowser
    public static class FileUtils
    {

        public static void PickFileToOperate(Action<string> action)
            => ShowSaveDialog((string[] paths) => action(paths[0]), () => { }, PickMode.Files);

    }

}