using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace NamelessRogueDataEditor.ViewModels
{
    public abstract partial class BaseEditorViewModel<ObjectDataType> : ObservableObject
    {
        [ObservableProperty]
        string headerName = "";

        [ObservableProperty]
        public string id;

        [ObservableProperty]
        public string iconPath;

        protected List<string> editorFiles;
        [ObservableProperty]
        public  List<string> editorFileNames;
        [ObservableProperty]
        public int selectedFileIndex;
        public string editorPath;

        public string FileType { get; private set; }
        public string FilesDirectory { get; private set; }

        [ObservableProperty]
        bool canSave = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType">Example string: "*.nrbf" for nameless rogue buff file "</param>
        /// <param name="filesDirectory">Example: "Buffs" will work with "NamelessRogue\\Content\\GameObjects\\Buffs</param>
        public BaseEditorViewModel(string fileType, string filesDirectory)
        {
            ReloadFiles(fileType, filesDirectory);  
        }

        protected void ReloadFiles(string fileType, string filesDirectory)
        {
            editorPath = Path.Combine(ContentDirectoryHelper.contentDirectoryPath, "GameObjects", filesDirectory);
            editorFiles = Directory.EnumerateFiles(editorPath, fileType, SearchOption.AllDirectories).ToList();
            EditorFileNames = editorFiles.Select(x => Path.GetFileName(x)).ToList();
            FileType = fileType;
            FilesDirectory = filesDirectory;
        }

        protected string _selectFile(string extensions = "*.png;*.jpg", string title = "Select a file")
        {
            var result = string.Empty;
            OpenFileDialog fileDialog = new OpenFileDialog();
            var dir = ContentDirectoryHelper.contentDirectoryPath;
            Directory.CreateDirectory(dir);
            fileDialog.InitialDirectory = dir;
            fileDialog.Filter = "Files|"+ extensions;
            fileDialog.Title = title;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(fileDialog.FileName))
                {
                    result = fileDialog.FileName;
                }
            }

            fileDialog = null;
            return result;
        }


        protected abstract ObjectDataType FillDataForSave();
        protected abstract void FillDataFromSave(ObjectDataType data);

        [RelayCommand]
        public virtual void Save()
        {
            ObjectDataType objecToSave = FillDataForSave();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ObjectDataType));
            var dir = System.IO.Path.Combine(ContentDirectoryHelper.contentDirectoryPath, "GameObjects", FilesDirectory);
            System.IO.Directory.CreateDirectory(dir);
            var path = System.IO.Path.Combine(dir, $"{Id + Path.GetExtension(this.FileType)}");
            using var stream = System.IO.File.Create(path);
            serializer.Serialize(stream, objecToSave);
            System.Windows.MessageBox.Show($"Saved to {path}");
            ReloadFiles(FileType, FilesDirectory);
        }

        [RelayCommand]
        public virtual void Load()
        {
            if (!editorFiles.Any())
            {
                return;
            }

            var selectedFile = editorFiles[SelectedFileIndex];
            if (File.Exists(selectedFile))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ObjectDataType));
                using (FileStream fs = new(selectedFile, FileMode.Open))
                {
                    try
                    {
                        ObjectDataType data = (ObjectDataType)serializer.Deserialize(fs);
                        FillDataFromSave(data);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Error loading file: {ex.Message}");
                    }
                }
            }
        }

        [RelayCommand]
        public virtual void Delete()
        {
            if (!editorFiles.Any())
            {
                return;
            }
            var selectedFile = editorFiles[SelectedFileIndex];
            if (File.Exists(selectedFile))
            {
                File.Delete(selectedFile);
                ReloadFiles(FileType, FilesDirectory);
                if (!editorFiles.Any() && SelectedFileIndex>0)
                {   
                    SelectedFileIndex--;
                }   

            }
        }
    }
}
