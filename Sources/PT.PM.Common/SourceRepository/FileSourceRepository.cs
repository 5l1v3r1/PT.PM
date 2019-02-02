﻿using PT.PM.Common.Exceptions;
using PT.PM.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using PT.PM.Common.Files;

namespace PT.PM.Common.SourceRepository
{
    public class FileSourceRepository : SourceRepository
    {
        protected IEnumerable<string> fullNames;

        public FileSourceRepository(string fileName, Language? language = null)
            : this(new [] { fileName }, language)
        {
        }

        public FileSourceRepository(IEnumerable<string> fileNames, Language? language = null)
        {
            var fileNamesArray = fileNames as string[] ?? fileNames.ToArray();
            fullNames = fileNamesArray;

            if (language.HasValue)
            {
                Languages = new HashSet<Language> { language.Value };
            }

            try
            {
                RootPath = fileNamesArray.Length == 1 ? Path.GetFullPath(fileNamesArray[0]) : "";
            }
            catch
            {
            }
        }

        public override IEnumerable<string> GetFileNames() => fullNames;

        public override IFile ReadFile(string fileName)
        {
            IFile result;
            string rootPath = "";
            string name = "";

            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    rootPath = Path.GetDirectoryName(fileName);
                }
                name = Path.GetFileName(fileName);
                result = Read(fileName);
            }
            catch (Exception ex) when (!(ex is ThreadAbortException))
            {
                result = TextFile.Empty;
                Logger.LogError(new ReadException(result, ex));
            }

            result.RootPath = rootPath;
            result.Name = name;

            return result;
        }

        public override bool IsFileIgnored(string fileName, bool withParser, out Language language)
        {
            if (Languages.Count == 1)
            {
                language = Languages.First();
                return withParser && !language.IsParserExists();
            }

            return base.IsFileIgnored(fileName, withParser, out language);
        }

        protected virtual IFile Read(string fileName)
        {
            IFile result;
            if (CommonUtils.GetFormatByFileName(fileName) == SerializationFormat.MsgPack)
            {
                result = new BinaryFile(FileExt.ReadAllBytes(fileName));
            }
            else
            {
                result = new TextFile(FileExt.ReadAllText(fileName));
            }
            return result;
        }
    }
}
