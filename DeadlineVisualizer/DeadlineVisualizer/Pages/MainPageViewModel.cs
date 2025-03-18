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

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; NotifyPropertyChanged(); }
        }

        public ICommand AddMilestoneCommand { get; private set; }

        public MainPageViewModel(MilestoneBuffer milestoneBuffer)
        {
            _milestoneBuffer = milestoneBuffer;
            _milestoneBuffer.MilestoneDeleteRequested += MilestoneBuffer_MilestoneDeleteRequested;
            AddMilestoneCommand = new Command(AddMilestone);
            Milestones.CollectionChanged += Milestones_CollectionChanged;
        }

        private void Milestones_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }

        private void MilestoneBuffer_MilestoneDeleteRequested(object? sender, Milestone e)
        {
            if (Milestones.Any(_ => _.ID == e.ID))
            {
                var matching = Milestones.First(_ => _.ID == e.ID);
                Milestones.Remove(matching);
            }
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
                Milestones.CollectionChanged -= Milestones_CollectionChanged;
                Milestones = data;
                Milestones.CollectionChanged += Milestones_CollectionChanged;
                IsDirty = false;
            }
        }

        public void Clear()
        {
            Milestones.Clear();
            CurrentFileFullPath = string.Empty;
            IsDirty = false;
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

        private void AddMilestone()
        {
            var mileStone = new Milestone() {
                Deadline = DateTime.Today,
                WarningLevel1 = Preferences.Default.Get(Constants.warning_level_1_distance_key, Constants.warning_level_1_distance),
                WarningLevel2 = Preferences.Default.Get(Constants.warning_level_2_distance_key, Constants.warning_level_2_distance),
                WarningLevel3 = Preferences.Default.Get(Constants.warning_level_3_distance_key, Constants.warning_level_3_distance),
            };
            MilestoneChangeRequested?.Invoke(this, mileStone);
        }


        public event EventHandler<Milestone> MilestoneChangeRequested;
    }
}
