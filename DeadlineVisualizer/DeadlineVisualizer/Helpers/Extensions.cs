namespace DeadlineVisualizer
{
    internal static class Extensions
    {
        public static void ClearGrid(this Grid grid)
        {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            grid.Children.Clear();
        }
    }
}
