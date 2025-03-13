namespace DeadlineVisualizer
{
    public class MilestoneBuffer
    {
        private readonly Queue<Milestone> _milestoneQueue = new();

        public void Enqueue(Milestone milestone)
        {
            _milestoneQueue.Enqueue(milestone);
        }

        public Milestone Dequeue()
        {
            return _milestoneQueue.Dequeue();
        }

        public bool HasData() => _milestoneQueue.Count > 0;
    }
}
