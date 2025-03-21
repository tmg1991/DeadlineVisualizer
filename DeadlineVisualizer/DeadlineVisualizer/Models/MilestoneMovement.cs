namespace DeadlineVisualizer
{
    public class MilestoneMovement
    {
        public Milestone Milestone { get; init; }
        public int Movement { get; init; }

        public MilestoneMovement(Milestone milestone, int movement)
        {
            Milestone = milestone;
            Movement = movement;
        }
    }
}
