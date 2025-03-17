namespace DeadlineVisualizer
{
    public class MilestoneBuffer
    {
        private readonly Queue<Milestone> _milestoneQueue = new();
        private Milestone _milestoneToDelete;
        public Milestone MilestoneToDelete => _milestoneToDelete;

        public event EventHandler<Milestone> MilestoneDeleteRequested;

        public void Enqueue(Milestone milestone)
        {
            _milestoneQueue.Enqueue(milestone);
        }

        public Milestone Dequeue()
        {
            return _milestoneQueue.Dequeue();
        }

        public bool HasData() => _milestoneQueue.Count > 0;

        public void MarkMilestoneForDelete(Milestone milestone)
        {
            _milestoneToDelete = milestone;
            MilestoneDeleteRequested?.Invoke(this, milestone);
        }
    }
}
