﻿using PT.PM.Common.Nodes.TypeMembers;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace PT.PM.Common.Nodes.Expressions
{
    [MessagePackObject]
    public class AnonymousObjectExpression : Expression
    {
        [Key(0)] public override UstType UstType => UstType.AnonymousObjectExpression;

        [Key(UstFieldOffset)]
        public List<PropertyDeclaration> Properties { get; set; } = new List<PropertyDeclaration>();

        public AnonymousObjectExpression(IEnumerable<PropertyDeclaration> properties, TextSpan textSpan)
            : base(textSpan)
        {
            Properties = properties as List<PropertyDeclaration> ?? properties.ToList();
        }

        public AnonymousObjectExpression()
        {
        }

        public override Expression[] GetArgs() => new Expression[0];

        public override Ust[] GetChildren()
        {
            return Properties.ToArray();
        }

        public override string ToString()
        {
            return "{ " + string.Join("; ", Properties).ToStringWithTrailSpace() + "}";
        }
    }
}
