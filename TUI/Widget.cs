namespace cscharp_quiz_gabel.TUI
{
    class Widget(int x, int y, int width, int height)
    {
        // origin is top left corner of the terminal and goes down and right
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;

        public bool dirty = true;

        protected bool XCentered { get; set; } = false;
        protected bool YCentered { get; set; } = false;

        private void CenterX(int terminalWidth)
        {
            X = (terminalWidth - Width) / 2;
            dirty = true;
        }

        private void CenterY(int terminalHeight)
        {
            Y = (terminalHeight - Height) / 2;
            dirty = true;
        }

        public void Center(bool centerX, bool centerY)
        {
            XCentered = centerX;
            YCentered = centerY;
            dirty = true;
        }

        public bool Update(char[,] screenBuffer)
        {
            if (XCentered)
            {
                CenterX(screenBuffer.GetLength(0));
            }
            if (YCentered)
            {
                CenterY(screenBuffer.GetLength(1));
            }

            return Draw(screenBuffer);
        }

        protected virtual bool Draw(char[,] screenBuffer)
        {
            dirty = false;
            return true;
        }
    }
}