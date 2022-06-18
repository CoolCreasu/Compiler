using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class Lexer
    {
        private string _source;
        public char CurrentChar { get; private set; }
        private char _lastChar;
        private int _currentPos = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The source code</param>
        public Lexer(string source)
        {
            this._source = source + "\n"; // The source code to lex as a string. Append a newline at the end to simplify lexing/parsing the last token/statement.
            this.CurrentChar = ' '; // Current character in the string.
            this._currentPos = -1; // Current position in the string.
            NextChar();
        }

        /// <summary>
        /// Process the next character.
        /// </summary>
        public void NextChar()
        {
            _currentPos += 1;
            if (_currentPos >= _source.Length)
            {
                CurrentChar = '\0'; // EOF
            }
            else
            {
                CurrentChar = _source[_currentPos];
            }
        }

        /// <summary>
        /// Return the lookahead character.
        /// </summary>
        public char Peek()
        {
            if (_currentPos + 1 >= _source.Length)
            {
                return '\0';
            }
            else
            {
                return _source[_currentPos+1];
            }
        }

        public void Abort(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(1);
        }

        public void SkipWhitespace()
        {
            while (CurrentChar == ' ' || CurrentChar == '\t' || CurrentChar == '\r')
            {
                NextChar();
            }
        }

        public void SkipComment()
        {
            if (CurrentChar == '#')
            {
                while (CurrentChar != '\n') NextChar();
            }
        }

        /// <summary>
        /// Return the next token.
        /// </summary>
        public Token GetToken()
        {
            SkipWhitespace();
            SkipComment();
            Token? token = null;

            // Check the first character of this token to see if we can decide what it is.
            // If it is a multiple character operator (e.g. !=), number, identifier, or keyword then we will process the rest.
            if (CurrentChar == '+')
            {   // + Token
                token = new Token(CurrentChar.ToString(), TokenType.PLUS);
            }
            else if (CurrentChar == '-')
            {   // - Token
                token = new Token(CurrentChar.ToString(), TokenType.MINUS);
            }
            else if (CurrentChar == '*')
            {   // * Token
                token = new Token(CurrentChar.ToString(), TokenType.ASTERISK);
            }
            else if (CurrentChar == '/')
            {   // / Token
                token = new Token(CurrentChar.ToString(), TokenType.SLASH);
            }
            else if (CurrentChar == '\n')
            {   // NEWLINE Token
                token = new Token(CurrentChar.ToString(), TokenType.NEWLINE);
            }
            else if (CurrentChar == '\0')
            {   // EOF Token
                token = new Token(String.Empty, TokenType.EOF);
            }
            else if (CurrentChar == '=')
            {   // Check if this Token is = or ==
                if (Peek() == '=')
                {
                    _lastChar = CurrentChar;
                    NextChar();
                    token = new Token(_lastChar + CurrentChar.ToString(), TokenType.EQEQ);
                }
                else
                {
                    token = new Token(CurrentChar.ToString(), TokenType.EQ);
                }
            }
            else if (CurrentChar == '>')
            {
                // Check if this Token is > or >=
                if (Peek() == '=')
                {
                    _lastChar = CurrentChar;
                    NextChar();
                    token = new Token(_lastChar + CurrentChar.ToString(), TokenType.GTEQ);
                }
                else
                {
                    token = new Token(CurrentChar.ToString(), TokenType.GT);
                }
            }
            else if (CurrentChar == '<')
            {
                // Check if this Token is < or <=
                if (Peek() == '=')
                {
                    _lastChar = CurrentChar;
                    NextChar();
                    token = new Token(_lastChar + CurrentChar.ToString(), TokenType.LTEQ);
                }
                else
                {
                    token = new Token(CurrentChar.ToString(), TokenType.LT);
                }
            }
            else if (CurrentChar == '!')
            {   // Token = !=
                if (Peek() == '=')
                {
                    _lastChar = CurrentChar;
                    NextChar();
                    token = new Token(_lastChar.ToString() + CurrentChar.ToString(), TokenType.NOTEQ);
                }
                else
                {
                    Abort("Expected !=, got !" + Peek());
                }
            }
            else if (CurrentChar == '\"')
            {
                // Get characters between quotations.
                NextChar();
                var startPos = _currentPos;
                while (CurrentChar != '\"')
                {
                    // Don't allow special characters in string. No escape characters, newlines, tabs, or %.
                    // We will be using C's printf on this string.
                    if (CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t' || CurrentChar == '\\' || CurrentChar == '%')
                    {
                        Abort("Illigal character in string: " + CurrentChar);
                    }
                    NextChar();
                }
                var tokText = _source.Substring(startPos, _currentPos - startPos + 1); // Get the substring.
                token = new Token(tokText, TokenType.STRING);
            }
            else if (char.IsDigit(CurrentChar))
            {
                // Leading character is a digit so this must be a number.
                // Get all consecutive digits and decimal if there is one.
                var startPos = _currentPos;
                while (char.IsDigit(Peek())) 
                {
                    NextChar();
                }
                if (Peek() == '.') // Decimal!
                {
                    NextChar();

                    // Must have at least one digit after decimal.
                    if (!char.IsDigit(Peek()))
                    {
                        // Error
                        Abort("Illigal character in number: " + Peek());
                    }
                    while (char.IsDigit(Peek()))
                    {
                        NextChar();
                    }
                }
                var tokText = _source.Substring(startPos, _currentPos - startPos + 1);  // Get the substring.
                token = new Token(tokText, TokenType.NUMBER);
            }
            else if (char.IsLetter(CurrentChar))
            {
                // Leading character is a letter, so this must be an identifier or a keyword.
                // Get all consecutive alpha numeric characters.
                var startPos = _currentPos;
                while (char.IsLetterOrDigit(Peek()))
                {
                    NextChar();
                }
                var tokText = _source.Substring(startPos, _currentPos - startPos + 1);  // Get the substring.
                TokenType? keyword = (TokenType?)Token.CheckIfKeyword(tokText);
                if (keyword == null)    // Identifier
                {
                    token = new Token(tokText, TokenType.IDENT);
                }
                else    // Keyword
                {
                    token = new Token(tokText, (TokenType)keyword);
                }
            }
            else
            {
                Abort("Unknown token: " + CurrentChar);
            }

            NextChar();
            return token;
        }
    }
}
