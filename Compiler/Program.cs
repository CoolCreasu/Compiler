namespace Compiler
{
    internal class Program
    {
        //private static string _code = "PRINT \"hello\"\n";
        //private static Lexer _lexer;
        //private static Emitter _emitter;
        //private static Parser _parser;


        static void Main(string[] args)
        {
            //var path = Console.ReadLine();
            var path = "C:/Unity Project/basic.txt";
            var _code = File.ReadAllText(path);

            // Initialize the lexer, emitter and parser.
            Lexer _lexer = new Lexer(_code);
            Emitter _emitter = new Emitter("C:/Unity Project/output.c");
            Parser _parser = new Parser(_lexer, _emitter);

            _parser.Program();  // Start the parser
            _emitter.WriteFile(); // Write the output to a file.
            Console.WriteLine("Compiling complete");
        }
    }
}