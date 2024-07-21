using Ravel.Binding;
using Ravel.Values;

using System.Collections;

namespace Ravel
{

    public class NeoEvaluator
    {
        public BoundExpression Root { get; }
        public RavelGlobal Global { get; }

        public RavelCallStack CurrentCallStack { get; internal set; }

        public bool CalculateDone { get; private set; }
        public RavelObject Result { get; private set; }

        public NeoEvaluator(BoundProgram root, RavelGlobal global, RavelScope? scope = null)
        {
            Root = root.Expression;
            Global = global;
            RavelScope s = scope ?? new(Global.Variables);
            CurrentCallStack = new(s, root.Expression);
        }
        internal NeoEvaluator(BoundExpression root, RavelGlobal global, RavelScope? scope = null)
        {
            Root = root;
            Global = global;
            RavelScope s = scope ?? new(Global.Variables);
            CurrentCallStack = new(s, root);
        }
        internal RavelObject EvaluateInstantResult(RavelCallStack last)
        {
            while (CurrentCallStack.Parent != last.Parent)
            {
                Step();
            }
            var l = CurrentCallStack.SonResults[^1];
            CurrentCallStack = last;
            return l;
        }
        public RavelObject Evaluate()
        {
            while (!CalculateDone)
            {
                Step();
            }
            return Result;
        }
        internal void Step()
        {
            switch (CurrentCallStack.Expression)
            {
                case BoundLiteralExpression literal:
                    EvaluateLiteral(literal);
                    break;
                case BoundAssignmentExpression assignment:
                    EvaluateAssignment(assignment);
                    break;
                case BoundDefiningExpression defining:
                    EvaluateDefining(defining);
                    break;
                case BoundVariableExpression variable:
                    EvaluateVariable(variable);
                    break;
                case BoundUnaryExpression unary:
                    EvaluateUnary(unary);
                    break;
                case BoundBinaryExpression binary:
                    EvaluateBinary(binary);
                    break;
                case BoundFunctionCallExpression call:
                    EvaluateFunctionCall(call);
                    break;
                case BoundBlockExpression block:
                    EvaluateBlock(block);
                    break;
                case BoundListExpression list:
                    EvaluateList(list);
                    break;
                case BoundDotExpression dot:
                    EvaluateDot(dot);
                    break;
                case BoundFunctionDefiningExpression functionDefining:
                    EvaluateFunctionDefining(functionDefining);
                    break;
                case BoundIfExpression @if:
                    EvaluateIf(@if);
                    break;
                case BoundWhileExpression @while:
                    EvaluateWhile(@while);
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected node '{CurrentCallStack.Expression.Kind}'");
            }
        }
        internal void AddResultAndReturn(RavelObject result)
        {
            if (CurrentCallStack.Parent != null)
            {
                CurrentCallStack = CurrentCallStack.Parent.AddSonResult(result);
            }
            else
            {
                CalculateDone = true;
                Result = result;
            }
        }
        internal void AddResult(RavelObject result)
        {
            CurrentCallStack = CurrentCallStack.AddSonResult(result);
        }
        private void EvaluateWhile(BoundWhileExpression @while)
        {
            switch (CurrentCallStack.SonResults.Count)
            {
                case 0:
                    CurrentCallStack.Scope = new(CurrentCallStack.Scope);
                    CurrentCallStack = new(CurrentCallStack, @while.Condition);
                    break;
                case 1:
                    if (CurrentCallStack.SonResults[0].GetValue<bool>())
                    {
                        CurrentCallStack = new(CurrentCallStack, @while.Expression);
                    }
                    else
                    {
                        AddResultAndReturn(Global.TypePool.Unit);
                    }
                    break;
                case 2:
                    CurrentCallStack = CurrentCallStack.ClearSonResult();
                    CurrentCallStack = new(CurrentCallStack, @while.Condition);
                    break;

            }
        }

        private void EvaluateIf(BoundIfExpression @if)
        {
            switch (CurrentCallStack.SonResults.Count)
            {
                case 0:
                    CurrentCallStack = new(CurrentCallStack, @if.Condition);
                    break;
                case 1:
                    CurrentCallStack.Scope = new(CurrentCallStack.Scope);
                    bool b = CurrentCallStack.SonResults[0].GetValue<bool>();
                    CurrentCallStack = new(CurrentCallStack, b ? @if.ExpTrue : @if.ExpFalse);
                    break;
                case 2:
                    AddResultAndReturn(CurrentCallStack.SonResults[^1]);
                    break;
            }
        }

        private void EvaluateFunctionDefining(BoundFunctionDefiningExpression functionDefining)
        {
            RavelLambda function = new(functionDefining, CurrentCallStack.Scope, Global);
            AddResultAndReturn(function.GetRavelObject());//?
        }

        private void EvaluateDot(BoundDotExpression dot)
        {
            switch (CurrentCallStack.SonResults.Count)
            {
                case 0:
                    CurrentCallStack = new(CurrentCallStack, dot.Owner);
                    break;
                case 1:
                    RavelObject owner = CurrentCallStack.SonResults[0];
                    if (!owner.TryReturnSonValue(this, dot.Son))
                    {
                        throw new InvalidOperationException(dot.Son);
                    }
                    break;
                case 2:
                    AddResultAndReturn(CurrentCallStack.SonResults[^1]);
                    break;
            }
        }

        private void EvaluateAssignment(BoundAssignmentExpression assignment)
        {
            switch (CurrentCallStack.SonResults.Count)
            {
                case 0:
                    CurrentCallStack = new(CurrentCallStack, assignment.Expression);
                    break;
                case 1:
                    RavelScope scope = CurrentCallStack.Scope;
                    if (!scope.TryGetVariable(assignment.Name, out RavelVariable value))
                    {
                        throw new InvalidOperationException();
                    }
                    if (value.IsReadOnly)
                    {
                        throw new InvalidOperationException();
                    }
                    value.Object = CurrentCallStack.SonResults[0];
                    AddResultAndReturn(CurrentCallStack.SonResults[0]);
                    break;
            }
        }

        private void EvaluateBlock(BoundBlockExpression block)
        {
            int resultCount = CurrentCallStack.SonResults.Count;
            if (resultCount == 0)
            {
                CurrentCallStack.Scope = new(CurrentCallStack.Scope);
            }
            if (resultCount < block.Expressions.Count)
            {
                CurrentCallStack = new(CurrentCallStack, block.Expressions[resultCount]);
            }
            else
            {
                AddResultAndReturn(CurrentCallStack.SonResults[^1]);
            }
        }
        private void EvaluateList(BoundListExpression block)
        {
            int resultCount = CurrentCallStack.SonResults.Count;
            if (resultCount == 0)
            {
                CurrentCallStack.Scope = new(CurrentCallStack.Scope);
            }
            if (resultCount < block.Expressions.Count)
            {
                CurrentCallStack = new(CurrentCallStack, block.Expressions[resultCount]);
            }
            else
            {
                AddResultAndReturn(block.Type.GetRavelObject(new List<RavelObject>(CurrentCallStack.SonResults)));
            }
        }
        private void EvaluateFunctionCall(BoundFunctionCallExpression call)
        {
            int resultCount = CurrentCallStack.SonResults.Count;
            if (resultCount == 0)
            {
                CurrentCallStack = new(CurrentCallStack, call.Function);
            }
            else if (resultCount < call.Parameters.Count + 1)
            {
                CurrentCallStack = new(CurrentCallStack, call.Parameters[resultCount - 1]);
            }
            else if (resultCount == call.Parameters.Count + 1)
            {
                RavelObject func = CurrentCallStack.SonResults[0];
                func.Call(this, CurrentCallStack.SonResults.ToArray()[1..]);
            }
            else
            {
                AddResultAndReturn(CurrentCallStack.SonResults[^1]);
            }
        }

        private void EvaluateBinary(BoundBinaryExpression binary)
        {
            switch (CurrentCallStack.SonResults.Count)
            {
                case 0:
                    CurrentCallStack = new(CurrentCallStack, binary.Left);
                    break;
                case 1:
                    RavelObject left = CurrentCallStack.SonResults[0];
                    switch (binary.Op.Kind)
                    {
                        case RavelBinaryOperatorKind.ShortCutAnd:
                            {
                                if (!left.GetValue<bool>())
                                {
                                    AddResultAndReturn(Global.TypePool.False);
                                    return;
                                }
                            }
                            break;
                        case RavelBinaryOperatorKind.ShortCutOr:
                            {
                                if (left.GetValue<bool>())
                                {
                                    AddResultAndReturn(Global.TypePool.True);
                                    return;
                                }
                            }
                            break;
                    }
                    CurrentCallStack = new(CurrentCallStack, binary.Right);
                    break;
                case 2:
                    binary.Op.Function.Invoke(this, CurrentCallStack.SonResults[0], CurrentCallStack.SonResults[1]);
                    break;
                case 3:
                    AddResultAndReturn(CurrentCallStack.SonResults[^1]);
                    break;
            }
        }

        private void EvaluateUnary(BoundUnaryExpression unary)
        {
            switch (CurrentCallStack.SonResults.Count)
            {
                case 0:
                    CurrentCallStack = new(CurrentCallStack, unary.Operand);
                    break;
                case 1:
                    unary.Op.Function.Invoke(this, CurrentCallStack.SonResults[0]);
                    break;
                case 2:
                    AddResultAndReturn(CurrentCallStack.SonResults[^1]);
                    break;
            }
        }

        private void EvaluateVariable(BoundVariableExpression variable)
        {
            if (CurrentCallStack.Scope.TryGetVariable(variable.Name, out RavelVariable? value))
            {
                RavelObject obj = value!.Object;
                switch (CurrentCallStack.SonResults.Count)
                {
                    case 0:
                        if (value.IsFunctionSelf)
                        {
                            obj.Call(this, Global.TypePool.Unit);
                        }
                        else
                        {
                            AddResultAndReturn(obj);
                        }
                        break;
                    case 1:
                        AddResultAndReturn(CurrentCallStack.SonResults[^1]);
                        break;
                }

            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private void EvaluateDefining(BoundDefiningExpression defining)
        {
            switch (CurrentCallStack.SonResults.Count)
            {
                case 0:
                    CurrentCallStack = new(CurrentCallStack, defining.Expression);
                    break;
                case 1:
                    if (!CurrentCallStack.Scope.TryDeclare(defining.Declare.Name, CurrentCallStack.SonResults[^1], false, false))
                    {
                        throw new InvalidOperationException();
                    }
                    AddResultAndReturn(CurrentCallStack.SonResults[^1]);
                    break;
            }
        }

        private void EvaluateLiteral(BoundLiteralExpression num)
        {
            AddResultAndReturn(num.Value);
        }


    }
    public class RavelCallStack
    {
        public RavelCallStack(RavelCallStack? parent, BoundExpression expression)
        {
            Parent = parent;
            LastFunctionCall = parent?.LastFunctionCall ?? this;
            Expression = expression;
            Scope = parent?.Scope ?? new();
        }
        public RavelCallStack(RavelScope scope, BoundExpression expression)
        {
            Parent = null;
            LastFunctionCall = this;
            Expression = expression;
            Scope = scope;
        }
        private RavelCallStack(RavelCallStack basis, RavelObject append)
        {
            Parent = basis.Parent;
            LastFunctionCall = basis.LastFunctionCall;
            Expression = basis.Expression;
            Scope = basis.Scope;
            SonResults = new(append, basis.SonResults);
        }
        private RavelCallStack(RavelCallStack basis)
        {
            Parent = basis.Parent;
            LastFunctionCall = basis.LastFunctionCall;
            Expression = basis.Expression;
            Scope = basis.Scope;
            SonResults = SingleLinkedList<RavelObject>.Empty;
        }
        public RavelCallStack? Parent { get; }
        public RavelCallStack LastFunctionCall { get; internal set; }
        public BoundExpression Expression { get; }
        public RavelScope Scope { get; internal set; }

        public SingleLinkedList<RavelObject> SonResults { get; } = SingleLinkedList<RavelObject>.Empty;

        public RavelCallStack AddSonResult(RavelObject result)
        {
            return new(this, result);
        }
        public RavelCallStack ClearSonResult()
        {
            return new(this);
        }
    }
    public class SingleLinkedList<T> : IReadOnlyList<T>
    {
        private T Value { get; }
        public SingleLinkedList<T>? Next { get; }
        public int Count { get; }

        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException(nameof(index));
                }
                SingleLinkedList<T>? list = this;
                for (int i = 0; i < Count - 1 - index; i++)
                {
                    list = list!.Next;
                }
                return list!.Value;
            }
        }
        public T this[Index index]
        {
            get
            {
                return index.IsFromEnd ? this[Count - index.Value] : this[index.Value];
            }
        }
        public static SingleLinkedList<T> Empty
        {
            get
            {
                return new();
            }
        }
        public SingleLinkedList(T value, SingleLinkedList<T>? next = null)
        {
            Value = value;
            if (next == null || next.Count == 0)
            {
                Next = null;
                Count = 1;
            }
            else
            {
                Next = next;
                Count = next.Count + 1;
            }
        }
        private SingleLinkedList()
        {
            Value = default!;
            Count = 0;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return GetReversed().Reverse().GetEnumerator();
        }

        public IEnumerable<T> GetReversed()
        {
            if(Count == 0)
            {
                yield break;
            }
            SingleLinkedList<T>? list = this;
            while (list != null)
            {
                yield return list.Value;
                list = list.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
