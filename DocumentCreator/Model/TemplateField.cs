using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class TemplateField
    {
        public TemplateField(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
