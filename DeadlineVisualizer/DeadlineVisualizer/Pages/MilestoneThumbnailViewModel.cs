using System.Windows.Input;

namespace DeadlineVisualizer
{
    public class MilestoneThumbnailViewModel : NotifyBase
    {
        private readonly MilestoneBuffer _milestoneBuffer;
        private Milestone _milestone;
        public ICommand RemoveCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public Milestone Milestone
        {
            get { return _milestone; }
            set { _milestone = value; NotifyPropertyChanged(); }
        }

        public event EventHandler<Milestone> MilestoneChangeRequested;

        public MilestoneThumbnailViewModel(Milestone milestone, MilestoneBuffer milestoneBuffer)
        {
            _milestoneBuffer = milestoneBuffer;
            Milestone = milestone;
            RemoveCommand = new Command(Remove);
            EditCommand = new Command(Edit);
        }

        private void Remove(object o)
        {
            var milestone = ((MilestoneThumbnailViewModel)o).Milestone;
            _milestoneBuffer.MarkMilestoneForDelete(milestone);
        }

        private void Edit(object o)
        {
            var milestone = ((MilestoneThumbnailViewModel)o).Milestone;
            MilestoneChangeRequested?.Invoke(this, milestone);
        }
    }
}
