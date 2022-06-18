namespace Compiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Teeny Tiny Compiler 1.0");
            Console.WriteLine("This is a C# version of a compiler from a python tutorial made by Austin Z. Henley");
            Console.WriteLine("The website to the tutorial: https://austinhenley.com/blog/teenytinycompiler1.html");
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