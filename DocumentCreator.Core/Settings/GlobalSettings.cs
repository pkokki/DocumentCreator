using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Core.Settings
{
    public class GlobalSettings
    {
        public const string REPO_FILE_SYSTEM = "FileSystem";
        public const string REPO_AZURITE = "Azurite";
        public const string REPO_AZURE_BLOB = "AzureBlob";

        public GlobalSettings()
        {
            DocumentRepositoryType = REPO_FILE_SYSTEM;
            HtmlRepositoryType = REPO_FILE_SYSTEM;
        }

        public string DocumentRepositoryType { get; set; }
        public string HtmlRepositoryType { get; set; }

#pragma warning disable IDE0060 // Remove unused parameter
        public void Set(string settingKey, string value)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // TODO: implementation
        }
    }
}
