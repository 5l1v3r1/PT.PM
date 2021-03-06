﻿using System;
using Newtonsoft.Json;
using PT.PM.Common.Nodes;
using PT.PM.Common.Nodes.Expressions;
using PT.PM.Common.Nodes.Tokens;
using PT.PM.Common.Nodes.Tokens.Literals;

namespace PT.PM.Matching.Patterns
{
    public class PatternUnaryOperatorExpression : PatternUst, IPatternExpression
    {
        [JsonIgnore]
        public Type UstType => typeof(UnaryOperatorExpression);

        public PatternUst Expression { get; set; }

        public PatternUnaryOperatorLiteral Operator { get; set; }

        public override string ToString()
        {
            UnaryOperator op = Operator.UnaryOperator;
            if (UnaryOperatorLiteral.PrefixTextUnaryOperator.ContainsValue(op))
            {
                string spaceOrEmpty = op == UnaryOperator.Delete || op == UnaryOperator.DeleteArray || op == UnaryOperator.TypeOf ||
                                      op == UnaryOperator.Await || op == UnaryOperator.Void
                    ? " "
                    : "";
                return $"{Operator}{spaceOrEmpty}{Expression}";
            }

            if (UnaryOperatorLiteral.PostfixTextUnaryOperator.ContainsValue(op))
            {
                return $"{Expression}{Operator}";
            }

            return $"{Operator} {Expression}";
        }

        protected override MatchContext Match(Ust ust, MatchContext context)
        {
            var unaryOperatorExpression = ust as UnaryOperatorExpression;
            if (unaryOperatorExpression == null)
            {
                return context.Fail();
            }

            MatchContext newContext = Expression.MatchUst(unaryOperatorExpression.Expression, context);
            if (!newContext.Success)
            {
                return newContext;
            }

            newContext = Operator.MatchUst(unaryOperatorExpression.Operator, newContext);

            return newContext.AddUstIfSuccess(ust);
        }

        public PatternUst[] GetArgs()
        {
            return new [] {Expression};
        }
    }
}
