using Ravel.Binding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ravel.Rewrite
{
    internal class Rewriter
    {
        public BoundProgram Expression { get; }

        public Rewriter(BoundProgram expression)
        {
            Expression = expression;
        }
        public BoundProgram Rewrite()
        {
            return new(Rewrite(Expression.Expression));
        }
        protected BoundExpression Rewrite(BoundExpression root)
        {
            return root switch
            {
                BoundLiteralExpression literal => RewriteLiteral(literal),
                BoundAssignmentExpression assignment => RewriteAssignment(assignment),
                BoundDefiningExpression Defining => RewriteDefining(Defining),
                BoundVariableExpression variable => RewriteVariable(variable),
                BoundUnaryExpression unary => RewriteUnary(unary),
                BoundBinaryExpression binary => RewriteBinary(binary),
                BoundFunctionCallExpression call => RewriteFunctionCall(call),
                BoundBlockExpression block => RewriteBlock(block),
                BoundDotExpression dot => RewriteDot(dot),
                BoundFunctionDefiningExpression functionDefining => RewriteFunctionDefining(functionDefining),
                BoundIfExpression @if => RewriteIf(@if),
                BoundWhileExpression @while => RewriteWhile(@while),
                _ => root,
                //_ => throw new InvalidOperationException($"Unexpected node '{root.Kind}'"),
            };
        }

        private BoundExpression RewriteWhile(BoundWhileExpression @while)
        {
            var cond = Rewrite(@while.Condition);
            var expTrue = Rewrite(@while.Expression);
            if ((cond, expTrue) == (@while.Condition, @while.Expression))
            {
                return @while;
            }
            return new BoundWhileExpression(cond, expTrue);
        }

        protected virtual BoundExpression RewriteIf(BoundIfExpression @if)
        {
            var cond = Rewrite(@if.Condition);
            var expTrue = Rewrite(@if.ExpTrue);
            var expFalse = Rewrite(@if.ExpFalse);
            if ((cond, expTrue, expFalse) == (@if.Condition, @if.ExpTrue, @if.ExpFalse))
            {
                return @if;
            }
            return new BoundIfExpression(cond, expTrue, expFalse);
        }

        protected virtual BoundExpression RewriteFunctionDefining(BoundFunctionDefiningExpression functionDefining)
        {
            var sentence = Rewrite(functionDefining.Sentence);
            if(sentence == functionDefining.Sentence)
            {
                return functionDefining;
            }
            return new BoundFunctionDefiningExpression(functionDefining.Parameters, sentence, functionDefining.Type);
        }

        protected virtual BoundExpression RewriteDot(BoundDotExpression dot)
        {
            var owner = Rewrite(dot.Owner);
            if(owner == dot.Owner)
            {
                return dot;
            }
            return new BoundDotExpression(owner, dot.Son, dot.Type);
        }

        protected virtual BoundExpression RewriteBlock(BoundBlockExpression block)
        {
            List<BoundExpression> expressions = new();
            bool diff = false;
            foreach(var sent in block.Expressions)
            {
                BoundExpression item = Rewrite(sent);
                if(item != sent)
                {
                    diff = true;
                }
                expressions.Add(item);
            }
            if (!diff)
            {
                return block;
            }
            return new BoundBlockExpression(expressions);
        }

        protected virtual BoundExpression RewriteFunctionCall(BoundFunctionCallExpression call)
        {
            List<BoundExpression> expressions = new();
            bool diff = false;
            foreach (var sent in call.Parameters)
            {
                BoundExpression item = Rewrite(sent);
                if (item == sent)
                {
                    diff = true;
                }
                expressions.Add(item);
            }
            var func = Rewrite(call.Function);
            if (func != call.Function)
            {
                diff = true;
            }
            if (!diff)
            {
                return call;
            }
            return new BoundFunctionCallExpression(func, expressions);
        }

        protected virtual BoundExpression RewriteBinary(BoundBinaryExpression binary)
        {
            var l = Rewrite(binary.Left);
            var r = Rewrite(binary.Right);
            if((l, r) == (binary.Left, binary.Right))
            {
                return binary;
            }
            return new BoundBinaryExpression(l, binary.Op, r);
        }

        protected virtual BoundExpression RewriteUnary(BoundUnaryExpression unary)
        {
            var o = Rewrite(unary.Operand);
            if(o == unary.Operand)
            {
                return unary;
            }
            return new BoundUnaryExpression(unary.Op, o);
        }

        protected virtual BoundExpression RewriteVariable(BoundVariableExpression variable)
        {
            return variable;
        }

        protected virtual BoundExpression RewriteDefining(BoundDefiningExpression defining)
        {
            var exp = Rewrite(defining.Expression);
            if(exp == defining.Expression)
            {
                return defining;
            }
            return new BoundDefiningExpression(defining.Declare, exp);
        }

        protected virtual BoundExpression RewriteAssignment(BoundAssignmentExpression assignment)
        {
            var exp = Rewrite(assignment.Expression);
            if (exp == assignment.Expression)
            {
                return assignment;
            }
            return new BoundAssignmentExpression(assignment.Name, exp);
        }

        protected virtual BoundExpression RewriteLiteral(BoundLiteralExpression literal)
        {
            return literal;
        }
    }
}
