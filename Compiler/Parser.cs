using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    // The parser keeps track of current token and checks if the code matches the grammar.
    internal class Parser
    {
        public Parser(Lexer lexer)
        {

        }

        // Return true if the token matches.
        public void CheckToken(TokenType tokenType)
        {

        }

        // Return true if the next token matches.
        public void CheckPeek(TokenType tokenType)
        {

        }

        // Try to match current Token. If not, error. Advances the current token.
        public void Match(TokenType tokenType)
        {

        }

        public void Abort(string message)
        {
            throw new Exception(message);
        }
    }
}
