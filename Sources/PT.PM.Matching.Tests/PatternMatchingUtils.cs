﻿using PT.PM.Common;
using PT.PM.Common.CodeRepository;
using PT.PM.Dsl;
using PT.PM.Matching.PatternsRepository;
using System.Collections.Generic;
using System.Linq;

namespace PT.PM.Matching.Tests
{
    public class PatternMatchingUtils
    {
        public static MatchingResultDto[] GetMatchings(string code, string pattern, LanguageInfo analyseLanguage)
        {
            return GetMatchings(code, pattern, new[] { analyseLanguage });
        }

        public static MatchingResultDto[] GetMatchings(string code, string pattern,
            IEnumerable<LanguageInfo> analyzedLanguages,
            IEnumerable<LanguageInfo> patternLanguages = null)
        {
            var sourceCodeRep = new MemoryCodeRepository(code);
            var patternsRep = new MemoryPatternsRepository();
            var workflow = new Workflow(sourceCodeRep, analyzedLanguages, patternsRep)
            {
                Logger = new LoggerMessageCounter()
            };

            var processor = new DslProcessor();
            PatternRoot patternNode = processor.Deserialize(pattern);
            patternNode.Languages = new HashSet<LanguageInfo>(patternLanguages ?? LanguageUtils.PatternLanguages.Values);
            patternNode.DebugInfo = pattern;
            var patternsConverter = new PatternConverter();
            patternsRep.Add(patternsConverter.ConvertBack(new List<PatternRoot>() { patternNode }));
            WorkflowResult workflowResult = workflow.Process();
            MatchingResultDto[] matchingResults = workflowResult.MatchingResults.ToDto()
                .OrderBy(r => r.PatternKey)
                .ToArray();

            return matchingResults;
        }
    }
}
