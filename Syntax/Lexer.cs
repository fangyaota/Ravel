using Ravel.Text;
using Ravel.Values;

using System.Numerics;
using System.Text;

namespace Ravel.Syntax
{
    public class Lexer
    {
        private readonly SourceText _text;
        private int _position;

        private int badStart;
        private readonly StringBuilder badText;
        private bool lastBad;

        private readonly DiagnosticList _diagnostics;
        public Lexer(SourceText text, RavelGlobal global)
        {
            _text = text;
            _diagnostics = new(_text);
            Global = global;
            badText = new();
        }
        private char Current => Peek(0);
        private char Last => Peek(-1);
        private char Will => Peek(1);
        public IEnumerable<SyntaxToken> Lex()
        {
            SyntaxToken? token;
            do
            {
                token = NextToken();
                yield return token;
            } while (token.Kind != SyntaxKind.EndOfFile);
            _position = 0;
        }
        private char Peek(int offset)
        {
            int index = _position + offset;
            if (index >= _text.Length)
            {
                //_diagnostics.ReportPositionOutOfRange(index);
                return '\0';
            }
            else
            {
                return _text[index];
            }
        }
        public IEnumerable<Diagnostic> Diagnostics
        {
            get
            {
                return _diagnostics;
            }
        }

        public RavelGlobal Global { get; }

        private void Next()
        {
            _position++;
        }
        private bool TakeTill(out int start, out string text, Predicate<char> predicate)
        {
            start = _position;
            text = "";
            if (_position >= _text.Length)
            {
                return false;
            }
            if (!predicate(Current))
            {
                return false;
            }
            while (_position < _text.Length && predicate(Current))
            {
                Next();
            }
            text = _text[start.._position];
            return true;
        }
        private bool TakeContains(out int start, out string text, out SyntaxKind syntax)
        {
            start = _position;
            text = "";
            syntax = SyntaxKind.BadToken;
            for (int len = Global.SyntaxFacts.LongestLength; len > 0; len--)
            {
                if (_position + len > _text.Length)
                {
                    continue;
                }
                text = _text[_position..(_position + len)];
                start = _position;
                if (Global.SyntaxFacts.SyntaxDict.TryGetValue(text, out syntax))
                {
                    _position += len;
                    return true;
                }
            }
            return false;
        }
        private void TryReport()
        {
            if (lastBad)
            {
                _diagnostics.ReportInvalidLiteral(badText.ToString(), badStart);
                badText.Clear();
                lastBad = false;
            }
        }

        private int _start;
        private string? _nowText;
        public SyntaxToken NextToken()
        {
            //out of range
            if (_position >= _text.Length)
            {
                TryReport();
                return new(SyntaxKind.EndOfFile, _position, string.Empty, Global.TypePool.Unit);
            }
            //line
            if (TakeTill(out _start, out _nowText, ch => ch is '\n' or '\r'))
            {
                TryReport();
                return new(SyntaxKind.EndOfLine, _start, _nowText, Global.TypePool.Unit);
            }
            //white space, not line
            if (TakeTill(out _start, out _nowText, char.IsWhiteSpace))
            {
                TryReport();
                return new(SyntaxKind.WhiteSpace, _start, _nowText, Global.TypePool.StringType.GetRavelObject(_nowText));
            }
            //syntax
            if (TakeContains(out _start, out _nowText, out SyntaxKind syntax))
            {
                TryReport();
                return new(syntax, _start, _nowText, Global.TypePool.StringType.GetRavelObject(_nowText));
            }
            //digit
            if (TakeTill(out _start, out _nowText, char.IsDigit))
            {
                if (!BigInteger.TryParse(_nowText, out BigInteger num))
                {
                    _diagnostics.ReportInvalidLiteral(_nowText, _start);
                }
                TryReport();
                return new(SyntaxKind.Integer, _start, _nowText, Global.TypePool.IntType.GetRavelObject(num));
            }
            //string
            if (Current == '"')
            {
                Next();
                TakeTill(out _start, out _nowText, ch => ch is not ('"' or '\n' or '\r'));
                string real = '"' + _nowText;

                if (Current != '"')
                {
                    _diagnostics.ReportStringNotEnding(_start - 1, _position);
                }
                else
                {
                    Next();
                    real += '"';
                }
                TryReport();
                return new(SyntaxKind.String, _start - 1, real, Global.TypePool.StringType.GetRavelObject(_nowText));
            }
            //identity
            if (TakeTill(out _start, out _nowText, ch => char.IsLetter(ch) || ch == '_'))
            {
                SyntaxKind kind = Global.SyntaxFacts.GetKeywordKind(_nowText);
                TryReport();
                //if (kind == SyntaxKind.Boolean)
                //{
                //    return new(kind, _start, _nowText, RavelObject.GetBoolean(bool.Parse(_nowText), Global.TypePool));
                //}
                return new(kind, _start, _nowText, Global.TypePool.StringType.GetRavelObject(_nowText));
            }

            if (!lastBad)
            {
                badStart = _position;
            }
            badText.Append(Current);
            lastBad = true;

            return new(SyntaxKind.BadToken, _position++, Last.ToString(), Global.TypePool.StringType.GetRavelObject(Last.ToString()));
        }
    }
}
