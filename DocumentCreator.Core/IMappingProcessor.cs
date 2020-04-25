using DocumentCreator.Core.Model;
using System.Collections.Generic;

namespace DocumentCreator.Core
{
    public interface IMappingProcessor
    {
        IEnumerable<MappingStats> GetMappingStats(string mappingName = null);
        IEnumerable<Mapping> GetMappings(string templateName = null, string templateVersions = null, string mappingName = null);
        MappingDetails GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion = null);
        MappingDetails CreateMapping(string templateName, string mappingName, byte[] bytes);
        MappingDetails CreateMapping(string templateName, string mappingName, string testEvaluationsUrl);
        Evaluation Evaluate(EvaluationRequest request);
    }
}
