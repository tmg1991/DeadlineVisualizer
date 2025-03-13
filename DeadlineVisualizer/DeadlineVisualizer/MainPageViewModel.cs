using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DeadlineVisualizer
{
    public class MainPageViewModel : NotifyBase
    {
        private ObservableCollection<Milestone> _milestones;
        private readonly MilestoneBuffer _milestoneBuffer;

        public ObservableCollection<Milestone> Milestones
        {
            get { return _milestones; }
            set { _milestones = value; NotifyPropertyChanged(); }
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
                //TODO update to-be-collection based on GUID
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
