namespace cscharp_quiz_gabel.TUI.Widgets
{
    class Outlinewidget : Widget
    {
        public Outlinewidget(int x, int y, int width, int height) : base(x, y, width, height)
        {

        }

        protected override bool Draw(CharInfo[,] screenBuffer)
        {
            if (Width > screenBuffer.GetLength(0) || screenBuffer.GetLength(0) < 1 || Height > screenBuffer.GetLength(1) || screenBuffer.GetLength(1) < 1 || Width < 2 || Height < 2) { return false; }

            for (int i = X; i < X + Width && i < screenBuffer.GetLength(0); i++)
            {
                if (Y >= 0 && Y < screenBuffer.GetLength(1))
                {
                    screenBuffer[i, Y] = new CharInfo('-');
                }
                if (Y + Height - 1 >= 0 && Y + Height - 1 < screenBuffer.GetLength(1))
                {
                    screenBuffer[i, Y + Height - 1] = new CharInfo('-');
                }
            }

            for (int j = Y; j < Y + Height && j < screenBuffer.GetLength(1); j++)
            {
                if (X >= 0 && X < screenBuffer.GetLength(0))
                {
                    screenBuffer[X, j] = new CharInfo('|');
                }
                if (X + Width - 1 >= 0 && X + Width - 1 < screenBuffer.GetLength(0))
                {
                    screenBuffer[X + Width - 1, j] = new CharInfo('|');
                }
            }

            ClearUpdatedRegions();

            AddUpdatedRegion(X, Y, Width, 1);

            AddUpdatedRegion(X, Y + Height - 1, Width, 1);

            if (Height > 2)
            {
                AddUpdatedRegion(X, Y + 1, 1, Height - 2);
            }

            if (Height > 2)
            {
                AddUpdatedRegion(X + Width - 1, Y + 1, 1, Height - 2);
            }

            dirty = false;
            return true;
        }

    }
}