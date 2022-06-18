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
        Emitter _emitter;
        Token _currentToken;
        Token _peekToken;

        string[] symbols = { string.Empty };   // Variables declared so far.
        string[] labelsDeclared = { string.Empty }; // Labels declared so far.
        string[] labelsGotoed = { string.Empty };  // Labels goto'ed so far.

        public Parser(Lexer lexer, Emitter emitter)
        {
            _lexer = lexer;
            _emitter = emitter;

            //_currentToken = null;
            //_peekToken = null;
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

        /// <summary>
        /// program ::= {statement}
        /// </summary>
        public void Program()
        {
            _emitter.HeaderLine("#include <stdio.h>");
            _emitter.HeaderLine("int main(void){");

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

            // Wrap things up.
            _emitter.EmitLine("return 0;");
            _emitter.EmitLine("}");

            // Check that each label referenced in GOTO is declared.
            foreach (string label in labelsGotoed)
            {
                if (!labelsDeclared.Contains(label))
                {
                    Abort("Attempting to GOTO to undeclared variable: " + label);
                }
            }
        }

        // One of the following statements
        public void Statement()
        {
            // Check the first token to see what kind of statement it is.

            // "PRINT" (expression | string)
            if (CheckToken(TokenType.PRINT))
            {
                NextToken();

                if (CheckToken(TokenType.STRING))
                {
                    // Simple string so print it.
                    _emitter.EmitLine("printf(\"" + _currentToken.TokenText.Substring(0, _currentToken.TokenText.Length-1) + "\\n\");");
                    NextToken();
                }
                else
                {
                    // Expect an expression and print the result as a float.
                    _emitter.Emit("printf(\"%" + ".2f\\n\", (float)(");
                    Expression();
                    _emitter.EmitLine("));");
                }
            }
            // "IF" comparison "THEN" {statement} "ENDIF"
            else if (CheckToken(TokenType.IF))
            {
                NextToken();
                _emitter.Emit("if(");
                Comparison();

                Match(TokenType.THEN);
                NL();
                _emitter.Emit("){");

                // Zero or more statements in the body
                while (!CheckToken(TokenType.ENDIF))
                {
                    Statement();
                }

                Match(TokenType.ENDIF);
                _emitter.Emit("}");
            }
            // "WHILE" comparison "REPEAT" nl {statement nl} "ENDWHILE" nl
            else if (CheckToken(TokenType.WHILE))
            {
                NextToken();
                _emitter.Emit("while(");
                Comparison();

                Match(TokenType.REPEAT);
                NL();
                _emitter.EmitLine("){");

                // Zero or more statements in the loop body.
                while (!CheckToken(TokenType.ENDWHILE))
                {
                    Statement();
                }

                Match(TokenType.ENDWHILE);
                _emitter.Emit("}");
            }
            // "LABEL" ident
            else if (CheckToken(TokenType.LABEL))
            {
                NextToken();

                // Make sure the label doesn't already exist
                if (labelsDeclared.Contains(_currentToken.TokenText))
                {
                    Abort("Label already exists: " + _currentToken.TokenText);
                }
                Array.Resize(ref labelsDeclared, labelsDeclared.Length + 1);
                //labelsDeclared.Append(_currentToken.TokenText);
                labelsDeclared[labelsDeclared.Length - 1] = _currentToken.TokenText;

                _emitter.EmitLine(_currentToken.TokenText + ":");
                Match(TokenType.IDENT);
            }
            // "GOTO" ident
            else if (CheckToken(TokenType.GOTO))
            {
                NextToken();
                Array.Resize(ref labelsGotoed, labelsGotoed.Length + 1);
                //labelsGotoed.Append(_currentToken.TokenText);
                labelsGotoed[labelsGotoed.Length - 1] = _currentToken.TokenText;
                _emitter.EmitLine("goto " + _currentToken.TokenText + ";");
                Match(TokenType.IDENT);
            }
            // "LET" ident = expression
            else if (CheckToken(TokenType.LET))
            {
                NextToken();

                // Check if ident exists in symbol table. If not, declare it.
                if (!symbols.Contains(_currentToken.TokenText))
                {
                    Array.Resize(ref symbols, symbols.Length + 1);
                    //symbols.Append(_currentToken.TokenText);
                    symbols[symbols.Length - 1] = _currentToken.TokenText;
                    _emitter.HeaderLine("float " + _currentToken.TokenText + ";");
                }

                _emitter.Emit(_currentToken.TokenText + " = ");
                Match(TokenType.IDENT);
                Match(TokenType.EQ);

                Expression();
                _emitter.EmitLine(";");
            }
            // "INPUT" ident
            else if (CheckToken(TokenType.INPUT))
            {
                NextToken();

                // If variable doesn't already exist, declare it.
                if (!symbols.Contains(_currentToken.TokenText))
                {
                    Array.Resize(ref symbols, symbols.Length + 1);
                    //symbols.Append(_currentToken.TokenText);
                    symbols[symbols.Length-1] = _currentToken.TokenText;

                    _emitter.HeaderLine("float " + _currentToken.TokenText + ";");
                }

                // Emit scanf but also validate the input. If invalid, set the variable to 0 and clear the input.
                _emitter.EmitLine("if(0 == scanf(\"%" + "f\", &" + _currentToken.TokenText + ")) {");
                _emitter.EmitLine(_currentToken.TokenText + " = 0;");
                _emitter.Emit("scanf(\"%");
                _emitter.EmitLine("*s\");");
                _emitter.EmitLine("}");

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
            // Require at least one newline.
            Match(TokenType.NEWLINE);
            // But we will allow extra newlines too, of course.
            while (CheckToken(TokenType.NEWLINE))
                NextToken();
        }

        /// <summary>
        /// comparison ::= expression (("==" | "!=" | ">" | ">=" | "<" | "<=") expression)+
        /// </summary>
        public void Comparison()
        {
            Expression();
            // Must be at least one comparison operator and another expression.
            if (IsComparisonOperator())
            {
                _emitter.Emit(_currentToken.TokenText);
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
                _emitter.Emit(_currentToken.TokenText);
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
        /// expression ::= term {( "-" | "+" ) term}
        /// </summary>
        public void Expression()
        {
            Term();
            // Can have 0 or more +/- and expressions.
            while (CheckToken(TokenType.PLUS) || CheckToken(TokenType.MINUS))
            {
                _emitter.Emit(_currentToken.TokenText);
                NextToken();
                Term();
            }
        }

        /// <summary>
        /// expression ::= unary {( "/" | "*") unary}
        /// </summary>
        public void Term()
        {
            Unary();
            // Can have 0 or more *// and expressions.
            while (CheckToken(TokenType.ASTERISK) || CheckToken(TokenType.SLASH))
            {
                _emitter.Emit(_currentToken.TokenText);
                NextToken();
                Unary();
            }
        }
        /// <summary>
        /// unary ::= ["+" | "-"] primary
        /// </summary>
        public void Unary()
        {
            // Optional unary +/-
            if (CheckToken(TokenType.PLUS) || CheckToken(TokenType.MINUS))
            {
                _emitter.Emit(_currentToken.TokenText);
                NextToken();
            }
            Primary();
        }

        /// <summary>
        /// primary ::= number | ident
        /// </summary>
        public void Primary()
        {
            if (CheckToken(TokenType.NUMBER))
            {
                _emitter.Emit(_currentToken.TokenText);
                NextToken();
            }
            else if (CheckToken(TokenType.IDENT))
            {
                // Ensure the variable already exists.
                if (!symbols.Contains(_currentToken.TokenText))
                {
                    Abort("Referencing variable before assignment: " + _currentToken.TokenText);
                }
                _emitter.Emit(_currentToken.TokenText);
                NextToken();
            }
            else
            {
                // Error
                Abort("Unexpected token at " + _currentToken.TokenText);
            }
        }
    }
}