namespace cscharp_quiz_gabel.TUI
{
    class TextWidget : Widget
    {
        public string Content { get; set; }

        public TextWidget(int x, int y, string content) : this(x, y, 0, 1, content) { }

        public TextWidget(int x, int y, int width, int height, string content) : base(x, y, width, height)
        {
            Content = content;

            // Auto-calculate width if not provided (width == 0)
            if (Width == 0)
            {
                Width = Content.Length;
            }
        }
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        public void SetContent(string newContent)
        {
            Content = newContent;
            dirty = true;
        }

        protected override bool Draw(CharInfo[,] screenBuffer)
        {
            for (int i = 0; i < Content.Length && i < Width * Height; i++)
            {
                int screenX = X + (i % Width);
                int screenY = Y + (i / Width);
                if (screenX >= 0 && screenX < screenBuffer.GetLength(0) && screenY >= 0 && screenY < screenBuffer.GetLength(1))
                {
                    screenBuffer[screenX, screenY] = new CharInfo(Content[i], ForegroundColor, BackgroundColor);
                }
                else
                {
                    Console.WriteLine("TextWidget is out of bounds and cannot be drawn, please increase the buffer size");
                    return false;
                }
            }

            // Track the region that was updated
            int updatedHeight = (Content.Length + Width - 1) / Width; // Ceiling division
            UpdatedRegion = (X, Y, Width, updatedHeight);
            dirty = false;
            return true;
        }
    }
}