namespace DeadlineVisualizer
{
    public class MilestoneBuffer
    {
        private readonly Queue<Milestone> _milestoneQueue = new();
        
        public event EventHandler<Milestone> MilestoneDeleteRequested;
        public event EventHandler<MilestoneMovement> MilestoneMovementRequested;

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
            MilestoneDeleteRequested?.Invoke(this, milestone);
        }

        public void RequestMilestoneMovement(MilestoneMovement movement)
        {
            MilestoneMovementRequested?.Invoke(this, movement);
        }
    }
}
