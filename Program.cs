using cscharp_quiz_gabel.TUI;

class Program
{
    public static void Main()
    {
        string widthString;
        string heightString;
        TUIApp app = new TUIApp(80, 24, "Quiz Game");

        widthString = app.getWidth().ToString();
        heightString = app.getHeight().ToString();

        TextWidget textWidget = new TextWidget(20, 10, 25, 1, "WELCOME TO THE QUIZ GAME!");
        textWidget.AddPositionRule(textWidget.CenterX);
        textWidget.AddPositionRule((terminalWidth, terminalHeight) => (-1, terminalHeight / 2 - 2));
        app.AddWidget(textWidget);

        Button startButton = new Button(0, 0, 5, 1, "START", () =>
        {
            textWidget.SetContent("The quiz has started!");

        });
        startButton.AddPositionRule(startButton.Center);
        app.AddWidget(startButton);

        while (true)
        {
            app.Update();
        }
    }
}