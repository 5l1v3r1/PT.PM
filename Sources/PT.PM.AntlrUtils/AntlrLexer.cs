using System;
using System.Collections.Generic;
using System.Diagnostics;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using PT.PM.Common;
using PT.PM.Common.Exceptions;
using PT.PM.Common.Files;

namespace PT.PM.AntlrUtils
{
    public abstract class AntlrLexer : AntlrBaseHandler, ILanguageLexer
    {
        public static Dictionary<Language, ATN> Atns = new Dictionary<Language, ATN>();

        public virtual CaseInsensitiveType CaseInsensitiveType { get; } = CaseInsensitiveType.None;

        protected abstract string LexerSerializedATN { get; }

        public abstract IVocabulary Vocabulary { get; }

        public int LineOffset { get; set; }

        public abstract Lexer InitLexer(ICharStream inputStream);

        public IList<IToken> GetTokens(TextFile sourceFile, out TimeSpan lexerTimeSpan)
        {
            var errorListener = new AntlrMemoryErrorListener(sourceFile)
            {
                Logger = Logger,
                LineOffset = LineOffset
            };

            var preprocessedText = PreprocessText(sourceFile);
            var inputStream = new LightInputStream(Vocabulary, sourceFile, preprocessedText, CaseInsensitiveType);

            IList<IToken> tokens;
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Lexer lexer = InitLexer(inputStream);
                lexer.Interpreter = new LexerATNSimulator(lexer, GetOrCreateAtn(LexerSerializedATN));
                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(errorListener);
                lexer.TokenFactory = new LightTokenFactory();

                tokens = lexer.GetAllTokens();

                stopwatch.Stop();
                lexerTimeSpan = stopwatch.Elapsed;
            }
            catch (Exception ex)
            {
                Logger.LogError(new LexingException(sourceFile, ex));
                tokens = new List<IToken>();
            }

            return tokens;
        }

        protected virtual string PreprocessText(TextFile file) => file.Data;
    }
}
