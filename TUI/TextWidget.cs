namespace cscharp_quiz_gabel.TUI
{
    class TextWidget(int x, int y, int width, int height, string content) : Widget(x, y, width, height)
    {
        public string Content { get; set; } = content;
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

            dirty = false;
            return true;
        }
    }
}