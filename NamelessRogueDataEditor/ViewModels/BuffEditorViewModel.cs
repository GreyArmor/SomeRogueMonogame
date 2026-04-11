using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NamelessRogue.Engine.Generation.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class BuffEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        private BuffTemplateData? buff;

        public BuffEditorViewModel()
        {
            buff = new BuffTemplateData
            {
                Name = "New Buff",
                Description = "A new buff.",
                Duration = 0,
            };
        }

        [RelayCommand]
        public void Save()
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(BuffTemplateData));
            var dir = System.IO.Path.Combine("..", "NamelessRogue", "Content", "GameObjects", "Buffs");
            System.IO.Directory.CreateDirectory(dir);
            var path = System.IO.Path.Combine(dir, $"{Buff?.Name ?? "Buff"}.xml");
            using var stream = System.IO.File.Create(path);
            serializer.Serialize(stream, Buff);
            System.Windows.MessageBox.Show($"Saved to {path}");
        }

        [RelayCommand]
        public void SelectIcon()
        {
            string file = "";
            // Displays an OpenFileDialog so the user can select a file.  
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Files|*.txt;*.out";
            openFileDialog1.Title = "Select a File";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a file was selected, open it.  
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog1.FileName;
                //file = openFileDialog1.OpenFile().ToString();
                //openFileDialog1.Dispose();
            }
            openFileDialog1 = null;
            Console.WriteLine("File path is: " + file);
        }
    }
}
