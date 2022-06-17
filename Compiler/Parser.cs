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

            // Since some newlines are required in our grammar, need to skip the excess.
            while (CheckToken(TokenType.NEWLINE))
            {
                NextToken();
            }

            // Parse all the statements in the program.
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
                    Expression();
                }
            }
            // "IF" comparison "THEN" {statement} "ENDIF"
            else if (CheckToken(TokenType.IF))
            {
                Console.WriteLine("STATEMENT-IF");
                NextToken();
                Comparison();

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
                Comparison();

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
            else if (CheckToken(TokenType.GOTO))
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
                Expression();
            }
            // "INPUT" ident
            else if (CheckToken(TokenType.INPUT))
            {
                Console.WriteLine("STATEMENT-INPUT");
                NextToken();
                Match(TokenType.IDENT);
            }
            // This is not a valid statement. Error!
            else
            {
                Abort("Invalid statement at " + _currentToken.TokenText + " (" + _currentToken.TokenKind.ToString() + ")");
            }
            // Newline.
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

        public void Comparison()
        {
            Console.WriteLine("COMPARISON");

            Expression();
            // Must be at least one comparison operator and another expression.
            if (IsComparisonOperator())
            {
                NextToken();
                Expression();
            }
            else
            {
                Abort("Expected comparison operator at: " + _currentToken.TokenText);
            }
            // Can have 0 or more comparison operator and expressions
            while (IsComparisonOperator())
            {
                NextToken();
                Expression();
            }

        }

        /// <summary>
        /// Returns true if the current operator is a comparison operator.
        /// </summary>
        /// <returns></returns>
        public bool IsComparisonOperator()
        {
            return CheckToken(TokenType.GT) || CheckToken(TokenType.GTEQ) || CheckToken(TokenType.LT) || CheckToken(TokenType.LTEQ) || CheckToken(TokenType.EQEQ) || CheckToken(TokenType.NOTEQ);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Expression()
        {
            Console.WriteLine("Expression");

            Term();
            // Can have 0 or more +/- and expressions.
            while (CheckToken(TokenType.PLUS) || CheckToken(TokenType.MINUS))
            {
                NextToken();
                Term();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Term()
        {
            Console.WriteLine("TERM");

            Unary();
            // Can have 0 or more *// and expressions.
            while (CheckToken(TokenType.ASTERISK) || CheckToken(TokenType.SLASH))
            {
                NextToken();
                Unary();
            }
        }

        public void Unary()
        {
            Console.WriteLine("UNARY");

            // Optional unary +/-
            if (CheckToken(TokenType.PLUS) || CheckToken(TokenType.MINUS))
            {
                NextToken();
            }
            Primary();
        }

        public void Primary()
        {

        }
    }
}
