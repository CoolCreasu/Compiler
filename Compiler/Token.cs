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
            // If the string matches a Name in the TokenType enum and if the value >= 100 && value < 200
            if (Enum.TryParse(tokenText, true, out TokenType type))
            {
                var enumName = Enum.Parse<TokenType>(tokenText);
                int value = (int)enumName;
                if (value >= 100 && value < 200)
                {
                    return enumName;
                }
            }
            // Else
            return null;
        }
    }
}
