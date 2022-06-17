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
        Token? _currentToken;
        Token? _peekToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = null;
            _peekToken = null;
            NextToken();
            NextToken();    // Call this twice to initialize current and peek.
        }

        // Return true if the current token matches.
        public bool CheckToken(TokenType tokenKind)
        {
            return tokenKind == _currentToken.TokenKind;
        }

        // Return true if the next token matches.
        public bool CheckPeek(TokenType tokenKind)
        {
            return tokenKind == _peekToken.TokenKind;
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

        public void Program()
        {
            Console.WriteLine("PROGRAM");

            while (!CheckToken(TokenType.EOF))
            {
                Statement();
            }
        }

        // One of the following statements
        public void Statement()
        {
            // Check the first token to see what kind of statement it is.

            // "PRINT" (expression | string)
            if (CheckToken(TokenType.PRINT))
            {
                Console.WriteLine("STATEMENT-PRINT");
                NextToken();

                if (CheckToken(TokenType.STRING))
                {
                    // Simple string.
                    NextToken();
                }
                else
                {
                    // Expression
                    Console.WriteLine("Expression");
                }
            }
            // "IF" comparison "THEN" {statement} "ENDIF"
            else if (CheckToken(TokenType.IF))
            {
                Console.WriteLine("STATEMENT-IF");
                NextToken();
                //Comparison();

                Match(TokenType.THEN);
                NL();

                // Zero or more statements in the body
                while (!CheckToken(TokenType.ENDIF))
                {
                    Statement();
                }

                Match(TokenType.ENDIF);
            }
            // "WHILE" comparison "REPEAT" nl {statement nl} "ENDWHILE" nl
            else if (CheckToken(TokenType.WHILE))
            {
                Console.WriteLine("STATEMENT-WHILE");
                NextToken();
                //Comparison();

                Match(TokenType.REPEAT);
                NL();

                // Zero or more statements in the loop body.
                while (!CheckToken(TokenType.ENDWHILE))
                    Statement();

                Match(TokenType.ENDWHILE);
            }
            // "LABEL" ident
            else if (CheckToken(TokenType.LABEL))
            {
                Console.WriteLine("STATEMENT-LABEL");
                NextToken();
                Match(TokenType.IDENT);
            }
            // "GOTO" ident
            else if (!CheckToken(TokenType.GOTO))
            {
                Console.WriteLine("STATEMENT-GOTO");
                NextToken();
                Match(TokenType.IDENT);
            }
            // "LET" ident "=" expression
            else if (CheckToken(TokenType.LET))
            {
                Console.WriteLine("STATEMENT-LET");
                NextToken();
                Match(TokenType.IDENT);
                Match(TokenType.EQ);
                //Expression();
            }
            NL();
        }

        public void NL()
        {
            Console.WriteLine("NEWLINE");

            // Require at least one newline.
            Match(TokenType.NEWLINE);
            // But we will allow extra newlines too, of course.
            while (CheckToken(TokenType.NEWLINE))
                NextToken();
        }
    }
}
