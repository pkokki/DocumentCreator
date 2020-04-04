using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    /// <summary>
    /// The DataAssociation class is used to model how data is pushed into a template
    /// 
    /// DataAssociations have one or more sources and a target; the source of the association 
    /// is copied into the target.
    /// 
    /// The ItemDefinition from the souceRef and targetRef MUST have the same ItemDefinition or the
    /// DataAssociation MUST have a transformation Expression that transforms the source ItemDefinition 
    /// into the target ItemDefinition.
    /// 
    /// The core concepts of a DataAssociation are that they have sources, a target, and an optional transformation.
    /// When a data association is “executed,” data is copied to the target. What is copied depends if there is
    /// a transformation defined or not.
    /// If there is no transformation defined or referenced, then only one source MUST be defined, and the contents 
    /// of this source will be copied into the target.
    /// If there is a transformation defined or referenced, then this transformation Expression will be evaluated and 
    /// the result of the evaluation is copied into the target. There can be zero (0) to many sources defined in this 
    /// case, but there is no requirement that these sources are used inside the Expression.
    /// </summary>
    public class DataAssociation
    {
        /// <summary>
        /// [0..*] Identifies the source of the Data Association. 
        /// </summary>
        public IEnumerable<ItemAwareElement> Sources { get; set; }

        /// <summary>
        /// Identifies the target of the Data Association.
        /// </summary>
        public ItemAwareElement Target { get; set; }

        /// <summary>
        /// Specifies one or more data elements Assignments. By using an
        /// Assignment, single data structure elements can be assigned from the
        /// source structure to the target structure.
        /// </summary>
        public IEnumerable<Assignment> Assignments { get; set; }

        /// <summary>
        /// [0..1] Specifies an optional transformation Expression. The actual scope of
        /// accessible data for that Expression is defined by the source and target of
        /// the specific Data Association types.
        /// </summary>
        public Expression Transformation { get; set; }
    }

    public class Expression
    {

    }

    /// <summary>
    /// The Assignment class is used to specify a simple mapping of data elements using a specified Expression language.
    /// </summary>
    public class Assignment
    {
        /// <summary>
        /// The Expression that evaluates the source of the Assignment
        /// </summary>
        public Expression From { get; set; }

        /// <summary>
        /// The Expression that defines the actual Assignment operation and the target data element.
        /// </summary>
        public Expression To { get; set; }

    }

    public class ItemAwareElement
    {
        public ItemDefinition ItemSubject { get; set; }
        public DataState DataState { get; set; }
    }

    public enum DataState
    {

    }

    public class ItemDefinition
    {
        public string StructureRef { get; set; }
        public bool IsCollection { get; set; }
        //public ItemKind ItemKind { get; set; }
    }
}
