using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class Mapping
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public int Templates { get; set; }
        public int Documents { get; set; }
    }
}
