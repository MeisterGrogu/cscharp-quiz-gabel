using System.Runtime.InteropServices;

namespace cscharp_quiz_gabel.TUI
{
    class TUIApp
    {
        // Fixed dimensions for the TUI
        private const int FIXED_WIDTH = 120;
        private const int FIXED_HEIGHT = 10;

        private int terminalWidth;
        private int terminalHeight;

        private bool dirty;
        private bool terminalTooSmall = false;

        internal CharInfo[,] screenBuffer;

        private List<Widget> widgets = new List<Widget>();

        private const bool debugShowMouse = false;
        private int lastMouseX = -1;
        private int lastMouseY = -1;

        public TUIApp(int initialWidth, int initialHeight, string title = "TUI App")
        {
            terminalWidth = initialWidth;
            terminalHeight = initialHeight;
            screenBuffer = new CharInfo[FIXED_WIDTH, FIXED_HEIGHT];
            dirty = true;
            Mouse.EnableMouseInput();
            Console.Title = title;
        }

        private void updateTerminalSize(int newWidth, int newHeight)
        {
            terminalWidth = newWidth;
            terminalHeight = newHeight;
            dirty = true;

            if (newWidth < FIXED_WIDTH || newHeight < FIXED_HEIGHT)
            {
                terminalTooSmall = true;
            }
            else
            {
                terminalTooSmall = false;
            }

            int offsetX = Math.Max(0, (terminalWidth - FIXED_WIDTH) / 2);
            int offsetY = Math.Max(0, (terminalHeight - FIXED_HEIGHT) / 2);
            Mouse.SetMouseOffset(offsetX, offsetY);
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

            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < offsetY; y++)
            {
                Console.WriteLine();
            }

            for (int y = 0; y < FIXED_HEIGHT; y++)
            {
                // Print left offset
                Console.Write(new string(' ', offsetX));

                for (int x = 0; x < FIXED_WIDTH; x++)
                {
                    CharInfo charInfo = screenBuffer[x, y];
                    Console.ForegroundColor = charInfo.ForegroundColor;
                    Console.BackgroundColor = charInfo.BackgroundColor;

                    if (charInfo.Content == '\0')
                    {
                        Console.Write(' ');
                    }
                    else
                    {
                        Console.Write(charInfo.Content);
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }

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

            Mouse.ProcessMouseInput();

            CharInfo[,] newScreenBuffer = new CharInfo[FIXED_WIDTH, FIXED_HEIGHT];

            if (!terminalTooSmall)
            {
                foreach (var widget in widgets)
                {
                    if (widget.dirty || dirty)
                    {
                        dirty = true;
                        widget.processInput();
                        if (!widget.Update(newScreenBuffer))
                        {
                            dirty = false;
                        }
                    }
                }
            }

            if (debugShowMouse)
            {
                int mouseX = Mouse.GetMouseX();
                int mouseY = Mouse.GetMouseY();

                if (mouseX != lastMouseX || mouseY != lastMouseY)
                {
                    lastMouseX = mouseX;
                    lastMouseY = mouseY;
                    dirty = true;
                }

                if (mouseX >= 0 && mouseX + 2 < FIXED_WIDTH && mouseY >= 0 && mouseY < FIXED_HEIGHT)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (mouseX + i < FIXED_WIDTH)
                        {
                            newScreenBuffer[mouseX + i, mouseY] = new CharInfo('@');
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