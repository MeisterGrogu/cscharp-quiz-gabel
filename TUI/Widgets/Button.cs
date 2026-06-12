using System.Runtime.CompilerServices;

namespace cscharp_quiz_gabel.TUI.Widgets
{
    class Button : Widget
    {
        public string Text { get; set; }

        public Button(int x, int y, string text, Action action) : this(x, y, 0, 1, text, action) { }

        public Button(int x, int y, int width, int height, string text, Action action) : base(x, y, width, height)
        {
            Text = text;
            this.action = action;

            if (Width == 0)
            {
                Width = Text.Length + 4; // "[ " + text + " ]"
            }
        }
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor HoveredForegroundColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor HoveredBackgroundColor { get; set; } = ConsoleColor.White;

        private Action action;
        private bool handlerRegistered = false;

        private bool hovered = false;

        public override void setupInput()
        {
            if (!handlerRegistered)
            {
                Mouse.AddMouseClickHandler(ButtonMouseClickHandler);
                Mouse.AddMouseMoveHandler(ButtonMouseMoveHandler);
                handlerRegistered = true;
            }
        }

        protected override bool Draw(CharInfo[,] buffer)
        {
            string buttonText = $"[ {Text} ]";
            if (Width < 4)
            {
                Console.WriteLine("Width too small!");
                dirty = false;
                return false;
            }
            if (buttonText.Length > Width) buttonText = buttonText.Substring(0, Width - 2) + " ]";

            if (Y < 0 || Y >= buffer.GetLength(1))
            {
                Console.WriteLine("Button is out of bounds and cannot be drawn, please increase the buffer size");
                dirty = false;
                return false;
            }

            for (int i = 0; i < 2 && X + i < buffer.GetLength(0); i++)
            {
                buffer[X + i, Y] = new CharInfo(buttonText[i], ConsoleColor.White, ConsoleColor.Black);
            }

            ConsoleColor fg = hovered ? HoveredForegroundColor : ForegroundColor;
            ConsoleColor bg = hovered ? HoveredBackgroundColor : BackgroundColor;

            for (int i = 2; i < buttonText.Length - 2 && X + i < buffer.GetLength(0); i++)
            {
                buffer[X + i, Y] = new CharInfo(buttonText[i], fg, bg);
            }

            for (int i = buttonText.Length - 2; i < buttonText.Length && X + i < buffer.GetLength(0); i++)
            {
                buffer[X + i, Y] = new CharInfo(buttonText[i], ConsoleColor.White, ConsoleColor.Black);
            }

            AddUpdatedRegion(X, Y, Width, Height);
            dirty = false;
            return true;
        }

        private void ButtonMouseMoveHandler(int mouseX, int mouseY)
        {
            if (mouseX >= X && mouseX < X + Width && mouseY >= Y && mouseY < Y + Height)
            {
                if (!hovered)
                {
                    hovered = true;
                    dirty = true;
                }
            }
            else
            {
                if (hovered)
                {
                    hovered = false;
                    dirty = true;
                }
            }
        }

        private void ButtonMouseClickHandler(int clickX, int clickY, int button)
        {
            if (clickX >= X && clickX < X + Width && clickY >= Y && clickY < Y + Height && button == 1)
            {
                action();
            }
        }
    }
}