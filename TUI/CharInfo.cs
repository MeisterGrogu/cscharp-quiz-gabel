namespace cscharp_quiz_gabel.TUI
{
    public struct CharInfo
    {
        public char Content { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public CharInfo()
        {
            Content = ' ';
            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;
        }

        public CharInfo(char content, ConsoleColor fg = ConsoleColor.White, ConsoleColor bg = ConsoleColor.Black)
        {
            Content = content;
            ForegroundColor = fg;
            BackgroundColor = bg;
        }
    }
}