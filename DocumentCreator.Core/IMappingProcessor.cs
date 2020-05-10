using DocumentCreator.Core.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentCreator.Core
{
    public interface IMappingProcessor
    {
        IEnumerable<MappingStats> GetMappingStats(string mappingName = null);
        IEnumerable<Mapping> GetMappings(string templateName = null, string templateVersion = null, string mappingName = null);
        Task<MappingDetails> GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion = null);
        Task<MappingDetails> CreateMapping(string templateName, string mappingName, Stream bytes);
        Task<FillMappingResult> BuildMapping(FillMappingInfo info);
        EvaluationOutput Evaluate(EvaluationRequest request);
    }
}
