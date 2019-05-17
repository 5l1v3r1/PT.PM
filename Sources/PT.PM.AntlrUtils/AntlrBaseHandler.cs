using Antlr4.Runtime.Atn;
using PT.PM.Common;
using PT.PM.Common.Files;

namespace PT.PM.AntlrUtils
{
    public abstract class AntlrBaseHandler
    {
        public AntlrMemoryErrorListener ErrorListener { get; set; }

        public static ILogger StaticLogger { get; set; } = DummyLogger.Instance;

        public ILogger Logger { get; set; } = DummyLogger.Instance;

        public TextFile SourceFile { get; set; }

        public abstract Language Language { get; }

        public bool UseFastParseStrategyAtFirst { get; set; } = true;

        protected ATN GetOrCreateAtn(string atnText)
        {
            bool lexer = this is AntlrLexer;
            ATN atn;
            var atns = lexer ? AntlrLexer.Atns : AntlrParser.Atns;
            lock (atns)
            {
                if (!atns.TryGetValue(Language, out atn))
                {
                    atn = new ATNDeserializer().Deserialize(atnText.ToCharArray());
                    atns.Add(Language, atn);
                    Logger.LogDebug($"New ATN initialized for {Language} {(lexer ? "lexer" : "parser")}.");
                }
            }

            return atn;
        }
    }
}