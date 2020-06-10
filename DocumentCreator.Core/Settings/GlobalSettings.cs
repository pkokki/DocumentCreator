using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Core.Settings
{
    /// <summary>
    /// The settings of the API
    /// </summary>
    public class GlobalSettings
    {
        /// <summary>
        /// The type of document repository (templates, mappings and documents)
        /// </summary>
        public string DocumentRepositoryType { get; set; }

        /// <summary>
        /// The type of HTML repository
        /// </summary>
        public string HtmlRepositoryType { get; set; }

#pragma warning disable IDE0060 // Remove unused parameter
        public void Set(string settingKey, string value)
        {
            // TODO: implementation
        }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
