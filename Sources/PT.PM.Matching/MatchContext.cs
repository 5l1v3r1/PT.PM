﻿using PT.PM.Common;
using PT.PM.Common.Nodes;
using PT.PM.Common.Nodes.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace PT.PM.Matching
{
    public class MatchContext : ILoggable
    {
        public PatternRoot PatternUst { get; }

        public ILogger Logger { get; set; } = DummyLogger.Instance;

        public bool FindAllAlternatives { get; set; }

        public List<TextSpan> Locations { get; } = new List<TextSpan>();

        public Dictionary<string, IdToken> Vars { get; } = new Dictionary<string, IdToken>();

        public bool Success { get; private set; }

        public bool MatchedWithFolded { get; set; }

        public bool IgnoreLocations { get; set; }

        public UstConstantFolder UstConstantFolder { get; }

        public static MatchContext CreateWithInputParamsAndVars(MatchContext context)
        {
            return CreateWithInputParams(context, context.Vars);
        }

        public static MatchContext CreateWithInputParams(MatchContext context, Dictionary<string, IdToken> vars = null)
        {
            return new MatchContext(context.PatternUst, context.UstConstantFolder, vars)
            {
                Logger = context.Logger,
                FindAllAlternatives = context.FindAllAlternatives
            };
        }

        public MatchContext(PatternRoot patternUst, UstConstantFolder ustConstantFolder, Dictionary<string, IdToken> vars = null)
        {
            PatternUst = patternUst;
            UstConstantFolder = ustConstantFolder;
            if (vars != null)
            {
                Vars = vars;
            }
        }

        public static implicit operator bool(MatchContext context) => context.Success;

        public MatchContext AddUstIfSuccess(Ust ust)
        {
            if (Success && !IgnoreLocations && !ust.TextSpan.IsZero)
            {
                Locations.Add(ust.TextSpan);
            }
            return this;
        }

        public MatchContext AddMatch(Ust ust)
        {
            Success = true;
            if (!IgnoreLocations && ust != null && !ust.TextSpan.IsZero)
            {
                Locations.Add(ust.TextSpan);
            }
            return this;
        }

        public MatchContext AddMatch(TextSpan textSpan)
        {
            Success = true;
            if (!IgnoreLocations && !textSpan.IsZero)
            {
                Locations.Add(textSpan);
            }
            return this;
        }

        public MatchContext AddMatches(IEnumerable<TextSpan> textSpans)
        {
            Success = true;
            if (!IgnoreLocations)
            {
                Locations.AddRange(textSpans.Where(textSpan => !textSpan.IsZero));
            }
            return this;
        }

        public MatchContext Fail()
        {
            Success = false;
            return this;
        }

        public MatchContext MakeSuccess()
        {
            Success = true;
            return this;
        }

        public MatchContext Set(bool success)
        {
            Success = success;
            return this;
        }
        public override string ToString()
        {
            string vars = string.Join(", ", Vars.Select(v => $"{v.Key}: {v.Value}"));
            if (vars != "")
            {
                vars = "; Vars: " + vars;
            }
            return $"Status: {Success}{vars}";
        }
    }
}
