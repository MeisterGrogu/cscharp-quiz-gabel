namespace cscharp_quiz_gabel.App
{
    static class QuestionLoader
    {
        private static string LoadFromFile(string Path)
        {
            StreamReader sr = new StreamReader(Path);
            string File = sr.ReadToEnd();
            return File;
        }

        public static Question ParseQuestions(string Path)
        {
            var ToParse = LoadFromFile(Path);
            Console.WriteLine(ToParse);
            return new Question();
        }
    }
}