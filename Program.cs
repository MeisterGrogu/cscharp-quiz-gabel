using cscharp_quiz_gabel.TUI;

class Program
{
    public static void Main()
    {
        string widthString;
        string heightString;
        TUIApp app = new TUIApp(80, 24);

        widthString = app.getWidth().ToString();
        heightString = app.getHeight().ToString();

        TextWidget textWidget = new TextWidget(20, 10, 25, 1, "WELCOME TO THE QUIZ GAME!");
        textWidget.AddPositionRule(textWidget.CenterX);
        textWidget.AddPositionRule((terminalWidth, terminalHeight) => (-1, terminalHeight / 2 - 2));
        TextWidget textWidget2 = new TextWidget(20, 12, 22, 1, "PRESS ANY KEY TO START");
        textWidget2.AddPositionRule(textWidget2.Center);
        app.AddWidget(textWidget);
        app.AddWidget(textWidget2);
        while (true)
        {
            app.Update();
        }
    }
}