using System.Data;

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

        protected List<Func<int, int, (int, int)>> positionRules = new List<Func<int, int, (int, int)>>();


        public (int, int) CenterX(int terminalWidth, int terminalHeight)
        {
            int newX = (terminalWidth - Width) / 2;
            dirty = true;
            return (newX, -1);
        }

        public (int, int) CenterY(int terminalWidth, int terminalHeight)
        {
            int newY = (terminalHeight - Height) / 2;
            dirty = true;
            return (-1, newY);
        }

        public (int, int) Center(int terminalWidth, int terminalHeight)
        {
            int newX = CenterX(terminalWidth, terminalHeight).Item1;
            int newY = CenterY(terminalWidth, terminalHeight).Item2;
            dirty = true;
            return (newX, newY);
        }

        public void AddPositionRule(Func<int, int, (int, int)> rule)
        {
            positionRules.Add(rule);
            dirty = true;
        }

        public bool Update(char[,] screenBuffer)
        {
            int ruledX = X;
            int ruledY = Y;
            for (int i = 0; i < positionRules.Count; i++)
            {
                int tempX, tempY;
                (tempX, tempY) = positionRules[i](screenBuffer.GetLength(0), screenBuffer.GetLength(1));
                if (tempX != -1) ruledX = tempX;
                if (tempY != -1) ruledY = tempY;
            }

            if (ruledX < 0) ruledX = X;
            if (ruledY < 0) ruledY = Y;

            return Draw(screenBuffer, ruledX, ruledY);
        }

        protected virtual bool Draw(char[,] screenBuffer, int ruledX, int ruledY)
        {
            dirty = false;
            return true;
        }
    }
}