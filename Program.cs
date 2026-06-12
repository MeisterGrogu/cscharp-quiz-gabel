using cscharp_quiz_gabel.TUI;
using cscharp_quiz_gabel.TUI.Widgets;

class Program
{
    public static void Main()
    {
        string widthString;
        string heightString;
        TUIApp app = new TUIApp(80, 24, "Quiz Game");
        Button otherButton;
        Button startButton;

        widthString = app.getWidth().ToString();
        heightString = app.getHeight().ToString();

        Outlinewidget outlinewidget = new Outlinewidget(-1, -1, 50, 10, app);
        outlinewidget.AddPositionRule(outlinewidget.Center);
        app.AddWidget(outlinewidget);

        TextWidget textWidget = new TextWidget(20, 10, 0, 1, app, "WELCHE ANTWORT IST RICHTIG?");
        textWidget.AddPositionRule(textWidget.CenterX);
        textWidget.AddPositionRule((terminalWidth, terminalHeight) => (-1, terminalHeight / 2 - 2));
        app.AddWidget(textWidget);

        startButton = new Button(0, 0, 0, 1, app, "diese", (widgetManager, self) =>
        {
            textWidget.SetContent("Richtig");
            widgetManager.RemoveWidget(self);
            widgetManager.RemoveWidget(widgetManager.FindWidget("falsch"));
        }, "richtig");
        startButton.AddPositionRule((terminalWidth, terminalHeight) => (terminalWidth / 2 - 17, terminalHeight / 2 + 3));
        app.AddWidget(startButton);

        otherButton = new Button(0, 0, 0, 1, app, "diese (FALSCH)", (widgetManager, self) =>
        {
            textWidget.SetContent("Falsch");
            widgetManager.RemoveWidget(self);
            widgetManager.RemoveWidget(widgetManager.FindWidget("richtig"));
        }, "falsch");
        otherButton.AddPositionRule((terminalWidth, terminalHeight) => (terminalWidth / 2 - 2, terminalHeight / 2 + 3));
        app.AddWidget(otherButton);

        while (true)
        {
            app.Update();
        }
    }
}