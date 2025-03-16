using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DeadlineVisualizer
{
    public class MilestoneThumbnailViewModel : NotifyBase
    {
        private Milestone _milestone;
        public ICommand EditCommand { get; private set; }
        public Milestone Milestone
        {
            get { return _milestone; }
            set { _milestone = value; NotifyPropertyChanged(); }
        }

        public event EventHandler<Milestone> MilestoneChangeRequested;

        public MilestoneThumbnailViewModel(Milestone milestone)
        {
            Milestone = milestone;
            EditCommand = new Command(Edit);
        }

        private void Edit(object o)
        {
            var milestone = ((MilestoneThumbnailViewModel)o).Milestone;
            MilestoneChangeRequested?.Invoke(this, milestone);
        }
    }
}
