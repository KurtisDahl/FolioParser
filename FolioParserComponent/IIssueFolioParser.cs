﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FolioParserComponent
{
    internal interface IIssueFolioParser
    {
        Task<Issue> ParseAsync(IStorageFile issueFolioFile, string pathPrefix);
    }
}
