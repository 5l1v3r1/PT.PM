﻿using PT.PM.AntlrUtils;
using PT.PM.Common;

namespace PT.PM.PhpParseTreeUst
{
    public class PhpAntlrParseTree : AntlrParseTree
    {
        public override LanguageInfo SourceLanguage => Php.Language;

        public PhpAntlrParseTree()
        {
        }

        public PhpAntlrParseTree(PhpParser.HtmlDocumentContext syntaxTree)
            : base(syntaxTree)
        {
        }
    }
}
