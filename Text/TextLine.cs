namespace Ravel.Text
{
    public readonly struct TextLine
    {
        public TextLine(SourceText text, int lineIndex, int start, int end, int endIncludingLineBreaking)
        {
            Text = text;
            LineIndex = lineIndex;
            Start = start;
            End = end;
            EndIncludingLineBreaking = endIncludingLineBreaking;
        }
        public SourceText Text { get; }
        public int LineIndex { get; }
        public int Start { get; }
        public int End { get; }
        public int EndIncludingLineBreaking { get; }
        public int Length => End - Start;
        public int LengthIncludingLineBreaking => EndIncludingLineBreaking - Start;
        public TextSpan Span => new(Start, End);
        public TextSpan SpanIncludingLineBreaking => new(Start, EndIncludingLineBreaking);
        public char this[int index] => Text[index + Start];
        public string this[int from, int to]
        {
            get
            {
                int start = from + Start;
                int end = to + Start;
                return Text[start..end];
            }
        }

        public override string ToString()
        {
            return Text.ToString(Span);
        }
    }
}
