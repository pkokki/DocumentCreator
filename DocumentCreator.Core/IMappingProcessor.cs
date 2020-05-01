using DocumentCreator.Core.Model;
using System.Collections.Generic;
using System.IO;

namespace DocumentCreator.Core
{
    public interface IMappingProcessor
    {
        IEnumerable<MappingStats> GetMappingStats(string mappingName = null);
        IEnumerable<Mapping> GetMappings(string templateName = null, string templateVersion = null, string mappingName = null);
        MappingDetails GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion = null);
        MappingDetails CreateMapping(string templateName, string mappingName, Stream bytes);
        MappingDetails CreateMapping(string templateName, string mappingName, string testEvaluationsUrl);
        EvaluationOutput Evaluate(EvaluationRequest request);
    }
}
