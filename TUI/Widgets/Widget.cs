using System.Data;

namespace cscharp_quiz_gabel.TUI.Widgets
{
    public interface IWidgetManager
    {
        void AddWidget(Widget widget);
        void RemoveWidget(Widget? widget);
        List<Widget> GetAllWidgets();
        Widget? FindWidget(string? id);

    }

    public class Widget(int x, int y, int width, int height, IWidgetManager manager, string? id = null)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;


        public string? ID = id;

        public bool dirty = true;

        public List<(int x, int y, int width, int height)> UpdatedRegions { get; protected set; } = new List<(int, int, int, int)>();

        protected List<Func<int, int, (int, int)>> positionRules = new List<Func<int, int, (int, int)>>();

        public IWidgetManager Manager { get; set; } = manager;

        protected void AddUpdatedRegion(int x, int y, int width, int height)
        {
            UpdatedRegions.Add((x, y, width, height));
        }

        public void ClearUpdatedRegions()
        {
            UpdatedRegions.Clear();
        }


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

        protected void ApplyPositionRules(int terminalWidth, int terminalHeight)
        {
            foreach (var rule in positionRules)
            {
                var (newX, newY) = rule(terminalWidth, terminalHeight);
                if (newX != -1) X = newX;
                if (newY != -1) Y = newY;
            }
        }

        public virtual void setupInput()
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

        public bool IDEquals(string? id)
        {
            return id == ID;
        }

        public virtual void destroy()
        {

        }
    }
}