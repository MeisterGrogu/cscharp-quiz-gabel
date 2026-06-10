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

        // Tracks the region that was updated in the last draw
        public (int x, int y, int width, int height)? UpdatedRegion { get; protected set; } = null;

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

        private void ApplyPositionRules(int terminalWidth, int terminalHeight)
        {
            foreach (var rule in positionRules.ToList())
            {
                var (newX, newY) = rule(terminalWidth, terminalHeight);
                if (newX != -1) X = newX;
                if (newY != -1) Y = newY;
                positionRules.Remove(rule);
            }
        }

        public virtual void processInput()
        {

        }

        public bool Update(CharInfo[,] screenBuffer)
        {
            ApplyPositionRules(screenBuffer.GetLength(0), screenBuffer.GetLength(1));
            return Draw(screenBuffer);
        }

        protected virtual bool Draw(CharInfo[,] screenBuffer)
        {
            dirty = false;
            return true;
        }
    }
}