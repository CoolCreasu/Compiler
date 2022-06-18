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
            Console.WriteLine("Please put in the path to your code. (e.g. C:/code/hello.teeny)");
            var path = Console.ReadLine();
            Console.WriteLine("Please put in the path to your output. (e.g. C:/code/output.c)");
            var outputPath = Console.ReadLine();

            if (path == null || outputPath == null)
            {
                Console.WriteLine("No valid path given.");
            }
            else
            {
                var _code = File.ReadAllText(path);

                // Initialize the lexer, emitter and parser.
                Lexer _lexer = new Lexer(_code);
                Emitter _emitter = new Emitter(outputPath);
                Parser _parser = new Parser(_lexer, _emitter);

                _parser.Program();  // Start the parser
                _emitter.WriteFile(); // Write the output to a file.
                Console.WriteLine("Compiling complete");
            }
        }
    }
}