using Newtonsoft.Json;

namespace DocumentCreator.Model
{
    public class TemplateFieldExpression
    {
        public string Name { get; set; }
        public string Cell { get; set; }
        public string Expression { get; set; }
        public string Parent { get; set; }
        public bool IsCollection { get; set; }
        public string Content { get; set; }
    }
}
