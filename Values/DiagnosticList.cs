using Ravel.Binding;
using Ravel.Syntax;
using Ravel.Text;

using System.Collections;

namespace Ravel.Values
{
    public sealed class DiagnosticList : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new();
        public bool StopRecord { get; set; }
        public SourceText Text { get; }

        public DiagnosticList(SourceText text)
        {
            Text = text;
        }

        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return _diagnostics.OrderBy(x => x.Span.Start).GetEnumerator();
        }

        public void AddRange(IEnumerable<Diagnostic> diagnostics)
        {
            _diagnostics.AddRange(diagnostics);
        }

        public void ReportInvalidLiteral(string text, int position)
        {
            Rerror(new(position, position + text.Length), $"存在无效的字符<{text}>", 1);
        }

        public void ReportNotSupportedToken(SyntaxNode current)
        {
            Rerror(current.Span, $"存在未支持的符号<{current.Kind}>", 2);
        }

        public void ReportMissingToken(TextSpan span, SyntaxToken current, SyntaxKind expected)
        {
            Rerror(span, $"存在意外的符号<{current.Kind}>，需要符号<{expected}>", 5);
        }

        public void ReportOperatorNotDefined(BoundExpression left, BinaryExpressionSyntax syntax, BoundExpression right)
        {
            Rerror(syntax.Span, $"双目运算符<{syntax.OperatorToken.Text}>未定义于<{left.Type}>和<{right.Type}>之间", 6);
        }

        public void ReportOperatorNotDefined(UnaryExpressionSyntax syntax, BoundExpression operand)
        {
            Rerror(syntax.Span, $"单目运算符<{syntax.OperatorToken.Text}>未定义于<{operand.Type}>中", 7);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _diagnostics.GetEnumerator();
        }

        private void Rerror(TextSpan span, string message, int id)
        {
            if(StopRecord)
            {
                return;
            }
            
            _diagnostics.Add(new(Text, span, message, id, EmergenceType.Rerror));
        }
        private void Rarning(TextSpan span, string message, int id)
        {
            if (StopRecord)
            {
                return;
            }
            _diagnostics.Add(new(Text, span, message, id, EmergenceType.Rarning));
        }

        public void ReportUndefinedName(SyntaxToken syntax)
        {
            Rerror(syntax.Span, $"未定义变量：<{syntax.Text}>", 11);
        }

        public void ReportTypeNotMatching(TextSpan span, RavelType type, RavelType wanted)
        {
            Rerror(span, $"类型<{type}>与<{wanted.Name}>不匹配", 12);
        }

        public void ReportNotAType(TextSpan span, RavelType type)
        {
            Rerror(span, $"<{type}>不是类型", 13);
        }

        public void ReportTypeNotConst(DeclareExpressionSyntax syntax)
        {
            Rerror(syntax.Type!.Span, $"目标类型不是常量", 14);
        }

        public void ReportReadOnlyVariableChange(lDeclare ob, TextSpan span)
        {
            Rerror(span, $"只读变量<{ob.Name}>不可赋值", 15);
        }

        public void ReportParametersTooMany(ExpressionSyntax function, int have, int wanted)
        {
            Rerror(function.Span, $"参数过多：{have}个参数，但只需要{wanted}个", 16);
        }

        public void ReportNotAFunction(TextSpan span, BoundExpression type)
        {
            Rerror(span, $"<{type.Type}>不是函数", 17);
        }

        public void ReportVariableAlreadyExists(DeclareExpressionSyntax identifier)
        {
            Rerror(identifier.Span, $"变量<{identifier.Identifier.Text}>已经存在", 18);
        }

        public void ReportFunctionNoParameters(TextSpan span)
        {
            Rerror(span, "函数不可无参", 19);
        }

        public void ReportOpenCloseNotMatch(TextSpan textSpan)
        {
            Rerror(textSpan, "括号不匹配", 20);
        }

        internal void ReportStringNotEnding(int start, int position)
        {
            Rerror(new(start, position), "引号左右不匹配", 21);
        }

        internal void ReportDirectionNotSame(SyntaxToken binaryOperator, OperatorDirection lastDirection)
        {
            Rerror(binaryOperator.Span, $"运算符结合性不匹配，上次为{(lastDirection == OperatorDirection.Left ? "左" : "右")}结合性", 22);
        }

        internal void ReportFunctionParameterNoType(SyntaxToken variable)
        {
            Rerror(variable.Span, "函数参数必须拥有类型", 23);
        }

        internal void ReportNotSupportedYet(TextSpan span)
        {
            Rerror(span, "特性仍未实现", 24);
        }

        internal void ReportBlockEmpty(BlockSyntax block)
        {
            Rerror(block.Span, "语句块不能为空", 25);
        }
    }
}
