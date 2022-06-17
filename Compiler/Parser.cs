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
        Lexer _lexer;
        Token _currentToken;
        Token _peekToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
        }

        // Return true if the current token matches.
        public bool CheckToken(TokenType tokenKind)
        {
            return (tokenKind == _currentToken.TokenKind);
        }

        // Return true if the next token matches.
        public bool CheckPeek(TokenType tokenKind)
        {
            return (tokenKind == _peekToken.TokenKind);
        }

        // Try to match current Token. If not, error. Advances the current token.
        public void Match(TokenType tokenKind)
        {
            if (!CheckToken(tokenKind))
                Abort("Expected " + tokenKind.ToString() + ", got " + _currentToken.TokenKind.ToString());
            NextToken();
        }

        public void NextToken()
        {
            _currentToken = _peekToken;
            _peekToken = _lexer.GetToken();
            // No need to worry about passing the EOF, lexer handles that.
        }

        public void Abort(string message)
        {
            throw new Exception(message);
        }
    }
}
