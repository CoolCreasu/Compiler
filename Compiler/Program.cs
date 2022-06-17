namespace Compiler
{
    internal class Program
    {
        private static string _code = "+-123 9.8654*/*/UP LET BE IS GOTO LABEL";
        private static Lexer _lexer = new Lexer(_code);


        static void Main(string[] args)
        {
            Token? token = _lexer.GetToken();
            while (token.TokenKind != TokenType.EOF)
            {
                Console.WriteLine(token.TokenKind);
                token = _lexer.GetToken();
            }
        }
    }
}