using System;
using System.IO;
using SimpleFileBrowser;
using static SimpleFileBrowser.FileBrowser;

namespace Ramsey.Utilities
{

    //https://github.com/yasirkula/UnitySimpleFileBrowser
    public static class FileUtils
    {

        public static void PickFileToOperate(Action<string> action, string initialFileName = null, string title = "Save", string saveButtonText = "Save", params Filter[] filters)
        {
            SetFilters(false, filters);
            ShowSaveDialog((string[] paths) => action(paths[0]), () => { }, PickMode.Files, false, null, initialFileName, title, saveButtonText);
        }

    }

}