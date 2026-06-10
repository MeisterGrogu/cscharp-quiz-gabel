using System;
using System.Runtime.InteropServices;

namespace MouseCursor
{
    class Program
    {
        // --- 1. Windows API (P/Invoke) Declarations ---
        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_MOUSE_INPUT = 0x0010;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        const uint ENABLE_EXTENDED_FLAGS = 0x0080;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

        // --- 2. Memory Structs for Windows Events ---
        [StructLayout(LayoutKind.Explicit)]
        struct INPUT_RECORD
        {
            [FieldOffset(0)] public ushort EventType;
            [FieldOffset(4)] public MOUSE_EVENT_RECORD MouseEvent;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public uint dwButtonState;
            public uint dwControlKeyState;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct COORD
        {
            public short X;
            public short Y;
        }

        static void _Main(string[] args)
        {
            // --- 3. Configure the Console to Listen to the Mouse ---
            IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);

            GetConsoleMode(handle, out uint mode);
            mode |= ENABLE_MOUSE_INPUT;         // Turn on mouse input
            mode &= ~ENABLE_QUICK_EDIT_MODE;    // Turn off Quick Edit (which blocks the mouse)
            mode |= ENABLE_EXTENDED_FLAGS;
            SetConsoleMode(handle, mode);

            // --- 4. Set up the Environment ---
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("Move your mouse around the console!");
            Console.WriteLine("Press Ctrl+C to exit.");

            int lastX = 0;
            int lastY = 0;
            INPUT_RECORD[] record = new INPUT_RECORD[1];

            // --- 5. The Infinite Mouse-Tracking Loop ---
            while (true)
            {
                // Read the raw input stream from Windows
                ReadConsoleInput(handle, record, 1, out uint eventsRead);

                // Check if the event was a Mouse Event (0x0002)
                if (record[0].EventType == 0x0002)
                {
                    // Check if the mouse actually moved (0x0001)
                    if (record[0].MouseEvent.dwEventFlags == 0x0001)
                    {
                        // Get the new coordinates
                        int newX = record[0].MouseEvent.dwMousePosition.X;
                        int newY = record[0].MouseEvent.dwMousePosition.Y;

                        // Only update if the coordinates actually changed
                        if (newX != lastX || newY != lastY)
                        {
                            try
                            {
                                // Erase the old "@"
                                Console.SetCursorPosition(lastX, lastY);
                                Console.Write(" ");

                                // Draw the new "@"
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.SetCursorPosition(newX, newY);
                                Console.Write("@");
                                Console.ResetColor();

                                // Save the current position for the next loop
                                lastX = newX;
                                lastY = newY;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                // Ignore crashes if you drag the mouse outside the window too fast
                            }
                        }
                    }
                }
            }
        }
    }
}