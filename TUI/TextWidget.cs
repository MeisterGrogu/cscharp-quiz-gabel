namespace cscharp_quiz_gabel.TUI
{
    class TextWidget(int x, int y, int width, int height, string content) : Widget(x, y, width, height)
    {
        public string Content { get; set; } = content;

        public void SetContent(string newContent)
        {
            Content = newContent;
            dirty = true;
        }

        protected override bool Draw(char[,] screenBuffer, int ruledX, int ruledY)
        {
            for (int i = 0; i < Content.Length && i < Width * Height; i++)
            {
                int screenX = ruledX + (i % Width);
                int screenY = ruledY + (i / Width);
                if (screenX >= 0 && screenX < screenBuffer.GetLength(0) && screenY >= 0 && screenY < screenBuffer.GetLength(1))
                {
                    screenBuffer[screenX, screenY] = Content[i];
                }
                else
                {
                    return false;
                }
            }

            dirty = false;
            return true;
        }
    }
}