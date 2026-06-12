using System;

namespace cscharp_quiz_gabel.TUI.Widgets
{
    class TextWidget : Widget
    {
        public string Content { get; private set; }

        private bool needsCleanup = false;
        private int oldX;
        private int oldY;
        private int oldWidth;
        private int oldHeight;

        public TextWidget(int x, int y, string content) : this(x, y, 0, 1, content) { }

        public TextWidget(int x, int y, int width, int height, string content) : base(x, y, width, height)
        {
            Content = content;

            if (Width == 0)
            {
                Width = Content.Length;
            }
        }

        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        public void SetContent(string newContent, int width = 0, int height = 0)
        {
            oldX = X;
            oldY = Y;
            oldWidth = Width;
            oldHeight = Height;
            needsCleanup = true;

            Content = newContent;

            if (width != 0)
            {
                Width = width;
            }
            else
            {
                Width = Content.Length;
            }

            if (height != 0)
            {
                Height = height;
            }

            dirty = true;
        }

        protected override bool Draw(CharInfo[,] screenBuffer)
        {
            if (needsCleanup)
            {
                for (int dx = 0; dx < oldWidth; dx++)
                {
                    for (int dy = 0; dy < oldHeight; dy++)
                    {
                        int pX = oldX + dx;
                        int pY = oldY + dy;
                        if (pX >= 0 && pX < screenBuffer.GetLength(0) && pY >= 0 && pY < screenBuffer.GetLength(1))
                        {
                            screenBuffer[pX, pY] = new CharInfo();
                        }
                    }
                }

                ApplyPositionRules(screenBuffer.GetLength(0), screenBuffer.GetLength(1));

                AddUpdatedRegion(oldX, oldY, oldWidth, oldHeight);
                needsCleanup = false;
            }

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

            AddUpdatedRegion(X, Y, Width, Height);
            dirty = false;
            return true;
        }
    }
}