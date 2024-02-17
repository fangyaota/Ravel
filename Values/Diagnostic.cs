using Ravel.Text;

namespace Ravel.Values
{
    public class Diagnostic
    {
        public Diagnostic(SourceText text, TextSpan span, string message, int id, EmergenceType type)
        {
            Text = text;
            Span = span;
            Message = message;
            Id = id;
            Type = type;
        }

        public SourceText Text { get; }
        public TextSpan Span { get; }
        public string Message { get; }
        public int Id { get; }
        public EmergenceType Type { get; }

        public override string ToString()
        {
            Text.GetSpanIndex(Span, out TextLine line, out int start, out int end);
            string message;
            if (Type == EmergenceType.Rerror)
            {
                message = "错误";
            }
            else
            {
                message = "警告";
            }

            return $"伟大的Ravel第{Id}号{message}指出：在你位于({line.LineIndex},{start + 1})的位置中{Message}";
        }
    }
}
