
class Program
{
    public static void Main(String[] Args)
    {
        Questions questions = new Questions();
        questions.ParseQuestions(questions.Path);
    }
}