using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NamelessRogue.Engine.Generation.Editor;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace NamelessRogueDataEditor.ViewModels
{
    public partial class DialogEditorViewModel : BaseEditorViewModel<DialogData>
    {
        private DialogData dialog;

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string response;

        [ObservableProperty]
        private ObservableCollection<DialogOption> options;

        [ObservableProperty]
        private DialogOption selectedOption;

        [ObservableProperty]
        private object selectedOptionObject;

        public DialogEditorViewModel() : base("*.nrdl", "Dialogs")
        {
            dialog = new DialogData();
            options = new ObservableCollection<DialogOption>();
            PropertyChanged += (s, e) =>
            {
                CanSave = !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Response);

                if(e.PropertyName == "SelectedOptionObject")
                {
                    SelectedOption = (DialogOption)selectedOptionObject;                    
                }
            };
        }

        protected override DialogData FillDataForSave()
        {
            dialog.Id = id;
            dialog.Response = response;
            dialog.Options = options?.ToArray() ?? Array.Empty<DialogOption>();
            return dialog;
        }

        protected override void FillDataFromSave(DialogData data)
        {
            dialog = data ?? new DialogData();
            Id = dialog.Id;
            Response = dialog.Response;
            Options = new ObservableCollection<DialogOption>(dialog.Options ?? Array.Empty<DialogOption>());
            SelectedOption = Options.FirstOrDefault();
        }

        [RelayCommand]
        private void AddOption()
        {
            var baseId = "option";
            var nextIndex = 1;
            while (Options.Any(o => string.Equals(o.Id, $"{baseId}{nextIndex}", StringComparison.OrdinalIgnoreCase)))
            {
                nextIndex++;
            }

            var newOption = new DialogOption
            {
                Id = $"{baseId}{nextIndex}",
                OptionText = "New Option",
                DialogOutcomeId = DialogOutcomeId.None,
                DialogOutcomeData = string.Empty,
                HasDialogOutcomeData = false,
                DialogData = null
            };

            Options.Add(newOption);
            SelectedOption = newOption;
        }

        [RelayCommand]
        private void RemoveOption()
        {
            if (SelectedOption == null)
            {
                return;
            }

            var toRemove = SelectedOption;
            var index = Options.IndexOf(toRemove);
            Options.Remove(toRemove);

            if (Options.Count == 0)
            {
                SelectedOption = null;
            }
            else
            {
                var newIndex = Math.Min(Math.Max(0, index - 1), Options.Count - 1);
                SelectedOption = Options[newIndex];
            }
        }
    }
}
