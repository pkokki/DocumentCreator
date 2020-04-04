using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    /// <summary>
    /// Templates need data in order to create documents for a specific operation. 
    /// Data requirements are captured as Data Inputs and InputSets.
    /// These elements are aggregated in an InputSpecification class.
    /// </summary>
    public class InputSpecification
    {
        public string Name { get; set; }

        /// <summary>
        /// [1..*] A reference to the InputSets defined by the InputSpecification. 
        /// Every InputOutputSpecification MUST contain at least one InputSet.
        /// </summary>
        public IEnumerable<InputSet> InputSets { get; set; }

        /// <summary>
        /// [0..*] An optional reference to the Data Inputs of the InputSpecification. 
        /// If the InputSpecification defines no Data Input, it means no data is REQUIRED 
        /// to start the document creation. This is an ordered set.
        /// </summary>
        public IEnumerable<DataInput> DataInputs { get; set; }
    }

    /// <summary>
    /// An InputSet is a collection of DataInput elements that together define a valid set of data 
    /// inputs for an InputSpecification.
    /// 
    /// An InputSet MAY reference zero or more DataInput elements. 
    /// A single DataInput MAY be associated with multiple InputSet elements, 
    /// but it MUST always be referenced by at least one InputSet.
    /// 
    /// InputSet elements are contained by InputOutputSpecification elements; the order in which 
    /// these elements are included defines the order in which they will be evaluated.
    /// </summary>
    public class InputSet
    {
        public string Name { get; set; }

        public IEnumerable<DataInput> DataInputs { get; set; }
        public IEnumerable<DataInput> OptionalInputs { get; set; }
        public IEnumerable<DataInput> WhileExecutingInput { get; set; }
    }

    /// <summary>
    /// A Data Input is a declaration that a particular kind of data will be used as input of the
    /// InputSpecification. There may be multiple Data Inputs associated with an InputSpecification.
    /// </summary>
    public class DataInput
    {
        /// <summary>
        /// A descriptive name for the element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Defines if the DataInput represents a collection of elements. It is needed
        /// when no itemDefinition is referenced. If an itemDefinition is
        /// referenced, then this attribute MUST have the same value as the
        /// isCollection attribute of the referenced itemDefinition.The
        /// default value for this attribute is false.
        /// </summary>
        public bool IsCollection { get; set; } = false;
    }
}
