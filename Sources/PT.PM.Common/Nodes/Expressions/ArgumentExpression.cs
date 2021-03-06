﻿using MessagePack;
using PT.PM.Common.Nodes.Tokens.Literals;

namespace PT.PM.Common.Nodes.Expressions
{
    [MessagePackObject]
    public class ArgumentExpression : Expression
    {
        [Key(0)] public override UstType UstType => UstType.ArgumentExpression;

        [Key(UstFieldOffset)]
        public InOutModifierLiteral Modifier { get; set; }

        [Key(UstFieldOffset + 1)]
        public Expression Argument { get; set; }

        public ArgumentExpression(InOutModifierLiteral argumentModifier, Expression argument,
            TextSpan textSpan)
            : base(textSpan)
        {
            Modifier = argumentModifier;
            Argument = argument;
        }

        public ArgumentExpression(TextSpan textSpan)
            : base(textSpan)
        {
        }

        public ArgumentExpression()
        {
        }

        public override Expression[] GetArgs() => new[] { Argument };

        public override Ust[] GetChildren() => new[] { Argument };

        public override string ToString()
        {
            return $"{Modifier.ToStringWithTrailSpace()}{Argument}";
        }
    }
}
