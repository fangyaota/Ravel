namespace Ravel.Text
{
    public sealed class SourceText
    {
        public SourceText(string text)
        {
            Text = text;
            Lines = ParseLine();
        }
        public int GetLineIndex(int position)
        {
            int lower = 0;
            int upper = Lines.Count - 1;
            if (position >= Text.Length)
            {
                return upper;
            }
            while (true)
            {
                int i = (lower + upper) / 2;
                if (Lines[i].Start > position)
                {
                    upper = i;
                    continue;
                }
                if (Lines[i].EndIncludingLineBreaking <= position)
                {
                    lower = i;
                    continue;
                }
                return i;
            }
        }
        public void GetSpanIndex(TextSpan span, out TextLine line, out int start, out int end)
        {
            int lineIndex = GetLineIndex(span.Start);
            line = Lines[lineIndex];
            start = span.Start - line.Start;
            end = span.End - line.Start;
        }
        private List<TextLine> ParseLine()
        {
            List<TextLine> lines = new List<TextLine>();
            int start = 0;
            int index = 0;
            int id = 0;
            while (index < Text.Length)
            {
                int lineBreakWidth = GetLineBreakWidth(Text, index);
                if (lineBreakWidth > 0)
                {
                    lines.Add(new(this, id, start, index, index + lineBreakWidth));
                    index += lineBreakWidth;
                    start = index;
                    id++;
                }
                else
                {
                    index++;
                }
            }
            if (index >= start)
            {
                lines.Add(new(this, id, start, index, index));
            }
            return lines;
        }
        private static int GetLineBreakWidth(string text, int index)
        {
            char a = text[index];
            char b = text.Length <= index + 1 ? '\0' : text[index + 1];
            if (a == '\r' && b == '\n')
            {
                return 2;
            }
            if (a == '\r' || b == '\n')
            {
                return 1;
            }
            return 0;
        }

        public string Text { get; }
        public char this[int index] => Text[index];
        public string this[Range range] => Text[range];
        public int Length => Text.Length;
        public List<TextLine> Lines { get; }
        public override string ToString()
        {
            return Text;
        }
        public string ToString(int start, int end)
        {
            return Text[start..end];
        }
        public string ToString(TextSpan span)
        {
            return Text[span.Start..span.End];
        }
    }
}
