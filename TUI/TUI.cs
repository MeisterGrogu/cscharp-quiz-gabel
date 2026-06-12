using cscharp_quiz_gabel.TUI.Widgets;

namespace cscharp_quiz_gabel.TUI
{
    public class TUIApp : IWidgetManager
    {
        private const int FIXED_WIDTH = 120;
        private const int FIXED_HEIGHT = 10;

        private int terminalWidth;
        private int terminalHeight;

        private bool dirty;
        private bool terminalTooSmall = false;
        private bool terminalWasToSmall = false;

        internal CharInfo[,] screenBuffer;

        private List<Widget> widgets = new List<Widget>();

        private List<(int x, int y, int width, int height)> dirtyRegions = new List<(int, int, int, int)>();

        private int lastMouseX = -1;
        private int lastMouseY = -1;

        public TUIApp(int initialWidth, int initialHeight, string title = "TUI App")
        {
            terminalWidth = initialWidth;
            terminalHeight = initialHeight;
            screenBuffer = new CharInfo[FIXED_WIDTH, FIXED_HEIGHT];

            for (int y = 0; y < FIXED_HEIGHT; y++)
            {
                for (int x = 0; x < FIXED_WIDTH; x++)
                {
                    screenBuffer[x, y] = new CharInfo();
                }
            }

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
                terminalWasToSmall = false;
            }
            else
            {
                terminalTooSmall = false;
                terminalWasToSmall = true;
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

        private void drawRegions()
        {
            if (dirtyRegions.Count == 0) return;

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

            foreach (var region in dirtyRegions)
            {
                for (int y = region.y; y < region.y + region.height && y < FIXED_HEIGHT; y++)
                {
                    Console.SetCursorPosition(offsetX + region.x, offsetY + y);

                    for (int x = region.x; x < region.x + region.width && x < FIXED_WIDTH; x++)
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
                }
            }

            dirtyRegions.Clear();
            dirty = false;
        }

        public void AddWidget(Widget widget)
        {
            widget.Manager = this;
            widgets.Add(widget);
            dirty = true;
        }

        public void RemoveWidget(Widget? widget)
        {
            if (widget != null)
            {
                widget.destroy();
                widgets.Remove(widget);
                dirty = true;
            }
        }

        public List<Widget> GetAllWidgets()
        {
            return widgets.ToList();
        }

        public Widget? FindWidget(string? id)
        {
            return widgets.FirstOrDefault((widget) => widget.IDEquals(id));
        }

        public void ClearWidgets()
        {
            foreach (Widget widget in widgets.ToList())
            {
                widgets.Remove(widget);
            }
        }

        public int getWidth()
        {
            return FIXED_WIDTH;
        }

        public int getHeight()
        {
            return FIXED_HEIGHT;
        }

        private void clearRegion(int x, int y, int width, int height)
        {
            for (int i = 0; i < width && x + i < FIXED_WIDTH; i++)
            {
                for (int j = 0; j < height && y + j < FIXED_HEIGHT; j++)
                {
                    screenBuffer[x + i, y + j] = new CharInfo();
                }
            }
        }

        public void Update()
        {
            if (detectResize())
            {
                var newSize = Terminal.GetTerminalSize();
                updateTerminalSize(newSize.width, newSize.height);
                dirty = true;
            }

            Mouse.ProcessMouseInput();

            if (!terminalTooSmall)
            {
                if (dirty)
                {
                    clearRegion(0, 0, FIXED_WIDTH, FIXED_HEIGHT);
                }

                foreach (var widget in widgets)
                {
                    if (widget.dirty || dirty || terminalWasToSmall)
                    {
                        if (!dirty && widget.X >= 0 && widget.Y >= 0)
                        {
                            clearRegion(widget.X, widget.Y, widget.Width, widget.Height);
                        }

                        widget.setupInput();
                        if (widget.Update(screenBuffer))
                        {
                            if (widget.UpdatedRegions.Count > 0)
                            {
                                dirtyRegions.AddRange(widget.UpdatedRegions);
                                widget.ClearUpdatedRegions();
                            }
                            else
                            {
                                dirty = true;
                            }
                        }
                    }
                }

                terminalWasToSmall = false;
            }

#if DEBUG
#else
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
                        screenBuffer[mouseX + i, mouseY] = new CharInfo('@');
                    }
                }
                dirtyRegions.Add((mouseX, mouseY, 3, 1));
            }
#endif

            if (dirty)
            {
                clearScreen();
                drawBuffer();
            }
            else if (dirtyRegions.Count > 0)
            {
                drawRegions();
            }
        }
    }
}