namespace Ravel.Text
{
    public readonly struct TextSpan
    {
        public int Start { get; }
        public int End { get; }
        public int Length => End - Start;
        public TextSpan(int start, int end)
        {
            Start = start;
            End = end;
        }
        public override string ToString()
        {
            return $"index {Start}, length {Length}";
        }
        public static implicit operator Range(TextSpan span)
        {
            return new(span.Start, span.End);
        }
    }
}
