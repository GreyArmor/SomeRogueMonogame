using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class BaseEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        string headerName = "";

        protected List<string> editorFiles;
        [ObservableProperty]
        public  List<string> editorFileNames;
        public string editorPath;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType">Example string: "*.nrbf" for nameless rogue buff file "</param>
        /// <param name="filesDirectory">Example: "Buffs" will work with "NamelessRogue\\Content\\GameObjects\\Buffs</param>
        public BaseEditorViewModel(string fileType, string filesDirectory)
        {
            editorPath = System.IO.Path.Combine(ContentDirectoryHelper.contentDirectoryPath, "GameObjects", filesDirectory);
            editorFiles = Directory.EnumerateFiles(editorPath, fileType, SearchOption.AllDirectories).ToList();
            editorFileNames = editorFiles.Select(x => Path.GetFileName(x)).ToList();
        }
    }
}
