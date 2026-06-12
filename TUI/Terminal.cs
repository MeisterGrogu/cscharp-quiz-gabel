namespace cscharp_quiz_gabel.TUI
{
    class Terminal
    {
        public static (int width, int height) GetTerminalSize()
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            return (width, height);
        }
    }
}