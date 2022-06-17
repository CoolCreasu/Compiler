using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class Token
    {
        public string TokenText { get; private set; }
        public TokenType TokenKind { get; private set; }
        public Token(string tokenText, TokenType tokenType)
        {
            TokenText = tokenText;
            TokenKind = tokenType;
        }

        public static TokenType? CheckIfKeyword(string tokenText)
        {
            // TODO: Check if token is keyword
            foreach (TokenType kind in Enum.GetValues(typeof(TokenType)))
            {
                Console.WriteLine(kind + " <--> " + kind.ToString());
                return TokenType.IDENT;
            }

            // Return null if it is not a keyword
            return null;
        }
    }
}
