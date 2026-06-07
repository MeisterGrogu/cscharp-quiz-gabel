namespace cscharp_quiz_gabel.TUI
{
    class TUIApp
    {
        // Fixed dimensions for the TUI
        private const int FIXED_WIDTH = 120;
        private const int FIXED_HEIGHT = 40;

        private int terminalWidth;
        private int terminalHeight;

        private bool dirty;
        private bool terminalTooSmall = false;

        internal char[,] screenBuffer;

        private List<Widget> widgets = new List<Widget>();

        public TUIApp(int initialWidth, int initialHeight)
        {
            terminalWidth = initialWidth;
            terminalHeight = initialHeight;
            screenBuffer = new char[FIXED_WIDTH, FIXED_HEIGHT];
            dirty = true;
        }

        private void updateTerminalSize(int newWidth, int newHeight)
        {
            terminalWidth = newWidth;
            terminalHeight = newHeight;
            dirty = true;

            // Check if terminal is now too small
            if (newWidth < FIXED_WIDTH || newHeight < FIXED_HEIGHT)
            {
                terminalTooSmall = true;
            }
            else
            {
                terminalTooSmall = false;
            }
        }

        public bool detectResize()
        {
            var currentSize = Terminal.GetTerminalSize();
            return currentSize.width != terminalWidth || currentSize.height != terminalHeight;
        }

        private static void clearScreen()
        {
            Console.Clear();
        }

        private void drawBuffer()
        {
            if (terminalTooSmall)
            {
                clearScreen();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: Terminal is too small!\n");
                Console.Write($"Required: {FIXED_WIDTH}x{FIXED_HEIGHT}\n");
                Console.Write($"Current:  {terminalWidth}x{terminalHeight}\n");
                Console.ResetColor();
                dirty = false;
                return;
            }

            int offsetX = Math.Max(0, (terminalWidth - FIXED_WIDTH) / 2);
            int offsetY = Math.Max(0, (terminalHeight - FIXED_HEIGHT) / 2);

            string output = "";

            for (int y = 0; y < offsetY; y++)
            {
                output += "\n";
            }

            for (int y = 0; y < FIXED_HEIGHT; y++)
            {
                output += new string(' ', offsetX);

                for (int x = 0; x < FIXED_WIDTH; x++)
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
            return FIXED_WIDTH;
        }

        public int getHeight()
        {
            return FIXED_HEIGHT;
        }

        public void Update()
        {
            if (detectResize())
            {
                var newSize = Terminal.GetTerminalSize();
                updateTerminalSize(newSize.width, newSize.height);
            }

            // Always create a fresh screen buffer with fixed dimensions
            char[,] newScreenBuffer = new char[FIXED_WIDTH, FIXED_HEIGHT];

            if (!terminalTooSmall)
            {
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