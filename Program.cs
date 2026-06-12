using cscharp_quiz_gabel.TUI;
using cscharp_quiz_gabel.TUI.Widgets;

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

        Button startButton = new Button(0, 0, 9, 1, "START", () =>
        {
            textWidget.SetContent("The quiz has started!");

        });
        startButton.AddPositionRule(startButton.Center);
        app.AddWidget(startButton);

        Outlinewidget outlinewidget = new Outlinewidget(-1, -1, 50, 10);
        outlinewidget.AddPositionRule(outlinewidget.Center);
        app.AddWidget(outlinewidget);

        while (true)
        {
            app.Update();
        }
    }
}