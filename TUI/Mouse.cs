using System.Runtime.InteropServices;

namespace cscharp_quiz_gabel.TUI
{
    class Mouse
    {
        private const int STD_INPUT_HANDLE = -10;
        private const uint ENABLE_MOUSE_INPUT = 0x0010;
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool PeekConsoleInput(
            IntPtr hConsoleInput,
            [Out] INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleInput(
            IntPtr hConsoleInput,
            [Out] INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsRead);

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT_RECORD
        {
            [FieldOffset(0)] public ushort EventType;
            [FieldOffset(4)] public MOUSE_EVENT_RECORD MouseEvent;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSE_EVENT_RECORD
        {
            public COORD MousePosition;
            public uint ButtonState;
            public uint ControlKeyState;
            public uint EventFlags;
        }

        private const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001;
        private const uint RIGHTMOST_BUTTON_PRESSED = 0x0002;

        private static bool mouseEnabled = false;

        private static List<Action<int, int, int>> mouseClickHandlers = new List<Action<int, int, int>>();

        private static List<Action<int, int>> mouseMoveHandlers = new List<Action<int, int>>();

        private static IntPtr consoleHandle;
        private static int offsetX = 0;
        private static int offsetY = 0;
        private static int mouseX = 0;
        private static int mouseY = 0;
        private static int lastMouseX = -1;
        private static int lastMouseY = -1;

        public static void SetMouseOffset(int x, int y)
        {
            offsetX = x;
            offsetY = y;
        }

        public static int GetMouseOffsetX() => offsetX;
        public static int GetMouseOffsetY() => offsetY;
        public static int GetMouseX() => mouseX;
        public static int GetMouseY() => mouseY;

        public static void AddMouseClickHandler(Action<int, int, int> handler)
        {
            mouseClickHandlers.Add(handler);
        }

        public static void defaultMouseClickHandler(int x, int y, int button)
        {
            // button: 1 = left click, 2 = right click
        }

        private static void InvokeAllMouseClickHandlers(int x, int y, int button)
        {
            foreach (var handler in mouseClickHandlers)
            {
                handler(x, y, button);
            }
        }

        public static void AddMouseMoveHandler(Action<int, int> handler)
        {
            mouseMoveHandlers.Add(handler);
        }

        public static void defaultMouseMoveHandler(int x, int y)
        {
            // button: 1 = left click, 2 = right click
        }

        private static void InvokeAllMouseMoveHandlers(int x, int y)
        {
            foreach (var handler in mouseMoveHandlers)
            {
                handler(x, y);
            }
        }

        public static void ProcessMouseInput()
        {
            if (!mouseEnabled) return;

            INPUT_RECORD[] buffer = new INPUT_RECORD[128];
            if (PeekConsoleInput(consoleHandle, buffer, 128, out uint eventsAvailable) && eventsAvailable > 0)
            {
                if (ReadConsoleInput(consoleHandle, buffer, Math.Min((uint)128, eventsAvailable), out uint eventsRead))
                {
                    for (int i = 0; i < eventsRead; i++)
                    {
                        if (buffer[i].EventType == 0x0002)
                        {
                            MOUSE_EVENT_RECORD mouseRecord = buffer[i].MouseEvent;

                            int newX = mouseRecord.MousePosition.X;
                            int newY = mouseRecord.MousePosition.Y;

                            int adjustedX = newX - offsetX;
                            int adjustedY = newY - offsetY;

                            mouseX = adjustedX;
                            mouseY = adjustedY;

                            if (mouseRecord.EventFlags == 0)
                            {
                                if ((mouseRecord.ButtonState & FROM_LEFT_1ST_BUTTON_PRESSED) != 0)
                                {
                                    InvokeAllMouseClickHandlers(adjustedX, adjustedY, 1);
                                }
                                else if ((mouseRecord.ButtonState & RIGHTMOST_BUTTON_PRESSED) != 0)
                                {
                                    InvokeAllMouseClickHandlers(adjustedX, adjustedY, 2);
                                }
                            }

                            if (mouseRecord.EventFlags == 0x0001)
                            {
                                if (adjustedX != lastMouseX || adjustedY != lastMouseY)
                                {
                                    InvokeAllMouseMoveHandlers(adjustedX, adjustedY);
                                    lastMouseX = adjustedX;
                                    lastMouseY = adjustedY;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void EnableMouseInput()
        {
            consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
            if (GetConsoleMode(consoleHandle, out uint consoleMode))
            {
                consoleMode |= ENABLE_MOUSE_INPUT;
                consoleMode &= ~ENABLE_QUICK_EDIT_MODE;
                consoleMode |= ENABLE_EXTENDED_FLAGS;
                SetConsoleMode(consoleHandle, consoleMode);
                Console.CursorVisible = false;
            }
            else
            {
                Console.WriteLine("Failed to get console mode.");
            }

            mouseEnabled = true;
        }
    }
}