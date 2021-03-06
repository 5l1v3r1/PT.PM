﻿using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using PT.PM.Cli.Common;
using PT.PM.Common;
using PT.PM.Common.SourceRepository;
using PT.PM.Common.Files;
using PT.PM.Dsl;
using PT.PM.Matching;
using PT.PM.Matching.Json;
using PT.PM.Matching.Patterns;
using PT.PM.TestUtils;

namespace PT.PM.Tests
{
    [TestFixture]
    public class PatternTests
    {
        SourceRepository codeRepository = new MemorySourceRepository("<?php $a = 42;", "test.php", Language.Php);

        PatternRoot pattern = new PatternRoot
        {
            Node = new PatternAssignmentExpression
            {
                Left = new PatternIdRegexToken(),
                Right = new PatternAny()
            },
        };

        JsonPatternSerializer jsonPatternSerializer = new JsonPatternSerializer
        {
            Indented = true,
        };

        [Test]
        public void Check_RepositoryFactory_Pattern()
        {
            string serialized = jsonPatternSerializer.Serialize(pattern);
            string filePath = Path.Combine(TestUtility.TestsOutputPath, "pattern.json");
            File.WriteAllText(filePath, serialized);

            Check(filePath);
        }

        [Test]
        public void Check_RepositoryFactory_Patterns()
        {
            string serialized = jsonPatternSerializer.Serialize(new[] { pattern });
            string filePath = Path.Combine(TestUtility.TestsOutputPath, "patterns.json");
            File.WriteAllText(filePath, serialized);

            Check(filePath);
        }

        [Test]
        public void Check_RepositoryFactory_PatternDto()
        {
            string serialized = jsonPatternSerializer.Serialize(pattern);
            string patternDto = JsonConvert.SerializeObject(new PatternDto() { Value = serialized });
            string filePath = Path.Combine(TestUtility.TestsOutputPath, "patternDto.json");
            File.WriteAllText(filePath, patternDto);

            Check(filePath);
        }

        [Test]
        public void Check_RepositoryFactory_PatternDtos()
        {
            string serialized = jsonPatternSerializer.Serialize(pattern);
            string patternDto = JsonConvert.SerializeObject(new[] { new PatternDto() { Value = serialized } });
            string filePath = Path.Combine(TestUtility.TestsOutputPath, "patternDtos.json");
            File.WriteAllText(filePath, patternDto);

            Check(filePath);
        }

        [Test]
        public void Check_RepositoryFactory_Dsl()
        {
            Check("<[.*]> = #");
        }

        [Test]
        public void Check_IncorrectLanguageInPattern_CorrectlyProcessed()
        {
            string serialized = jsonPatternSerializer.Serialize(pattern);
            serialized = serialized.Replace("\"Languages\": []", "\"Languages\": [ \"Fake\" ]");
            string filePath = Path.Combine(TestUtility.TestsOutputPath, "pattern.json");
            File.WriteAllText(filePath, serialized);

            Check(filePath);
        }

        [Test]
        public void JsonSerialize_PatternWithVar_JsonEqualsToDsl()
        {
            var patternRoot = new PatternRoot
            {
                Node = new PatternStatements
                {
                    Statements = new List<PatternUst>
                    {
                        new PatternAssignmentExpression
                        {
                             Left = new PatternVar("pwd") { Value = new PatternIdRegexToken("password") },
                             Right = new PatternAny()
                        },

                        new PatternInvocationExpression
                        {
                            Target = new PatternAny(),
                            Arguments = new PatternArgs(
                                new PatternMultipleExpressions(),
                                new PatternVar("pwd"),
                                new PatternMultipleExpressions())
                        }
                    }
                }
            };

            var jsonSerializer = new JsonPatternSerializer
            {
                Indented = true,
                IncludeTextSpans = false
            };

            string json = jsonSerializer.Serialize(patternRoot);
            PatternRoot nodeFromJson = jsonSerializer.Deserialize(new TextFile(json) { PatternKey = "PatternWithVar" });

            var dslSeializer = new DslProcessor() { PatternExpressionInsideStatement = false };
            var nodeFromDsl = dslSeializer.Deserialize(
                new TextFile("<[@pwd:password]> = #; ... #(#*, <[@pwd]>, #*);") { PatternKey = "PatternWithVar2" });

            Assert.IsTrue(nodeFromJson.Node.Equals(patternRoot.Node));
            Assert.IsTrue(nodeFromJson.Node.Equals(nodeFromDsl.Node));
        }

        [Test]
        public void JsonDeserialize_PatternWithoutFormatAndLanguages_CorrectlyProcessed()
        {
            var data = "[{\"Key\":\"96\",\"Value\":\"(<[expr]>.)?<[(?i)(password|pwd)]> = <[\\\"\\\\w*\\\"]>\"}]";
            var patternDtos = JsonConvert.DeserializeObject<List<PatternDto>>(data);
            Assert.AreEqual(1, patternDtos.Count);
        }

        private void Check(string patternsString)
        {
            var logger = new TestLogger();
            var patternsRepository = RepositoryFactory.CreatePatternsRepository(patternsString, null, logger);

            var workflow = new Workflow(codeRepository)
            {
                PatternsRepository = patternsRepository,
                Logger = logger
            };
            workflow.Process();

            Assert.AreEqual(0, logger.ErrorCount, logger.ErrorsString);
            Assert.AreEqual(1, logger.Matches.Count);
        }
    }
}
