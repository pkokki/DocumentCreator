using System.Collections.Generic;

namespace DocumentCreator.Model
{
    public class TemplateVersion
    {
        public TemplateVersion(long id, byte[] buffer)
        {
            this.Id = id;
            this.Fields = new List<TemplateField>();
            this.Buffer = buffer;
        }

        public long Id { get; }
        public byte[] Buffer { get; }
        public List<TemplateField> Fields { get; private set; }
    }
}
