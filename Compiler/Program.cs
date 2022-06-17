namespace Compiler
{
    internal class Program
    {
        private static string _code = "PRINT \"hello\"\n";
        private static Lexer _lexer = new Lexer(_code);
        private static Parser _parser = new Parser(_lexer);


        static void Main(string[] args)
        {
            /*
            Token? token = _lexer.GetToken();
            while (token.TokenKind != TokenType.EOF)
            {
                Console.WriteLine(token.TokenKind);
                token = _lexer.GetToken();
            }
            */
            _parser.Program();
            Console.WriteLine("Parsing complete");
        }
    }
}