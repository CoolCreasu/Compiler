namespace Compiler
{
    internal class Program
    {
        //private static string _code = "PRINT \"hello\"\n";
        private static Lexer _lexer;
        private static Parser _parser;


        static void Main(string[] args)
        {
            //var path = Console.ReadLine();
            var path = "C:/Unity Project/basic.txt";
            var _code = System.IO.File.ReadAllText(path);

            _lexer = new Lexer(_code);
            _parser = new Parser(_lexer);
            _parser.Program();
            Console.WriteLine("Parsing complete");
        }
    }
}