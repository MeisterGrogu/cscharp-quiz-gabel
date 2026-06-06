namespace cscharp_quiz_gabel.TUI
{
    class TUIApp
    {
        private int width;
        private int heigth;

        private bool dirty;

        internal char[,] screenBuffer;

        private List<Widget> widgets = new List<Widget>();

        public TUIApp(int initialWidth, int initialHeight)
        {
            width = initialWidth;
            heigth = initialHeight;
            screenBuffer = new char[width, heigth];
            dirty = true;
        }

        private void resize(int newWidth, int newHeight)
        {
            width = newWidth;
            heigth = newHeight;
            dirty = true;
            clearScreen();
        }

        public bool detectResize()
        {
            return Terminal.GetTerminalSize() != (width, heigth);
        }

        private static void clearScreen()
        {
            Console.Clear();
        }

        private void drawBuffer()
        {
            string output = "";
            for (int y = 0; y < heigth; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (screenBuffer[x, y] == '\0')
                    {
                        output += " ";
                    }
                    else
                        output += screenBuffer[x, y];
                }
                output += "\n";
            }
            Console.SetCursorPosition(0, 0);
            Console.Write(output);
            dirty = false;
        }

        public void AddWidget(Widget widget)
        {
            widgets.Add(widget);
        }

        public void RemoveWidget(Widget widget)
        {
            widgets.Remove(widget);
        }

        public int getWidth()
        {
            return width;
        }

        public int getHeight()
        {
            return heigth;
        }

        public void Update()
        {

            if (detectResize())
            {
                resize(Terminal.GetTerminalSize().width, Terminal.GetTerminalSize().height);
            }

            char[,] newScreenBuffer = new char[width, heigth];

            foreach (var widget in widgets)
            {
                if (widget.dirty || dirty)
                {
                    dirty = true;
                    if (!widget.Update(newScreenBuffer))
                    {
                        clearScreen();
                        Console.WriteLine("Widget " + widget.GetType().Name + " is out of bounds and cannot be drawn, please increase the window size");
                    }
                }
            }

            if (dirty)
            {
                screenBuffer = newScreenBuffer;
                clearScreen();
                drawBuffer();
            }
        }
    }
}