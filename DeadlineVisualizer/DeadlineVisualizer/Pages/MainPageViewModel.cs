using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DeadlineVisualizer
{
    public class MainPageViewModel : NotifyBase
    {
        private ObservableCollection<Milestone> _milestones = new();
        private readonly MilestoneBuffer _milestoneBuffer;

        public ObservableCollection<Milestone> Milestones
        {
            get { return _milestones; }
            set { _milestones = value; NotifyPropertyChanged(); }
        }

        private string _currentFileName;
        public string CurrentFileName
        {
            get { return _currentFileName; }
            private set { _currentFileName = value; NotifyPropertyChanged(); }
        }

        private string _currentFileFullPath;
        public string CurrentFileFullPath
        {
            get { return _currentFileFullPath; }
            set 
            {
                _currentFileFullPath = value;
                CurrentFileName = Path.GetFileName(value);
                NotifyPropertyChanged();
            }
        }

        public ICommand NewFileCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand SaveFileCommand { get; private set; }
        public ICommand AddMilestoneCommand { get; private set; }

        public MainPageViewModel(MilestoneBuffer milestoneBuffer)
        {
            _milestoneBuffer = milestoneBuffer;
            NewFileCommand = new Command(NewFile);
            OpenFileCommand = new Command(OpenFile);
            SaveFileCommand = new Command(SaveFile);
            AddMilestoneCommand = new Command(AddMilestone);
        }

        public void OnNavigatedTo()
        {
            if (_milestoneBuffer.HasData())
            {
                var milestone = _milestoneBuffer.Dequeue();
                UpdateCollection(milestone);
            }
        }

        public Stream GetSerializedMilestones()
        {
            var options = new JsonSerializerOptions { WriteIndented = true, };
            string json = JsonSerializer.Serialize<ObservableCollection<Milestone>>(Milestones, options);
            var stream = new MemoryStream(Encoding.Default.GetBytes(json));
            return stream;
        }

        public async Task LoadFromFileAsync(string filepath)
        {
            var jsonString = await File.ReadAllTextAsync(filepath);
            using var stream = new MemoryStream(Encoding.Default.GetBytes(jsonString));
            var data = await JsonSerializer.DeserializeAsync<ObservableCollection<Milestone>>(stream);
            if (data != null)
            {
                Milestones = data;
            }
        }

        public void Clear()
        {
            Milestones.Clear();
            CurrentFileFullPath = string.Empty;
        }

        private void UpdateCollection(Milestone milestone)
        {
            int index = -1;
            if(Milestones.Any(_ => _.ID == milestone.ID))
            {
                var matching = Milestones.First(_ => _.ID == milestone.ID);
                index = Milestones.IndexOf(matching);
                Milestones.Remove(matching);
            }

            if (index < 0)
            {
                Milestones.Add(milestone);
            }
            else 
            {
                Milestones.Insert(index, milestone); 
            }

        }

        private void NewFile() { throw new NotImplementedException(); }
        private void OpenFile() { throw new NotImplementedException(); }
        private void SaveFile() { throw new NotImplementedException();
        
        }
        private void AddMilestone()
        {
            var mileStone = new Milestone() { Deadline = DateTime.Today};
            MilestoneChangeRequested?.Invoke(this, mileStone);
        }


        public event EventHandler<Milestone> MilestoneChangeRequested;
    }
}
