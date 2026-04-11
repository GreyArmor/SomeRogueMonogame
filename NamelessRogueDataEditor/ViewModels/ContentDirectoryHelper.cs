using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogueDataEditor.ViewModels
{
    public static class ContentDirectoryHelper
    {
        public static string contentDirectoryPath;

        static ContentDirectoryHelper()
        {
            string workingDirectory = Environment.CurrentDirectory;
#if DEBUG
            string projectDirectory = Directory.GetParent(Directory.GetParent(workingDirectory).Parent.Parent.FullName).FullName + "\\NamelessRogue";
            contentDirectoryPath = projectDirectory + "\\Content\\";
#else
            contentDirectoryPath = workingDirectory + "\\Content\\";
#endif
        }
    }
}