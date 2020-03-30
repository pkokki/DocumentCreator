using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class TemplateDocument
    {
        public TemplateDocument(long id, byte[] buffer)
        {
            this.Id = id;
            this.Buffer = buffer;
        }

        public long Id { get; }
        public byte[] Buffer { get; }
    }
}
