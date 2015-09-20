using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.WebClient.WidgetComplements.Model.JSObject.Output
{
    public class DsdObject {

        public string Name { get; set; }
        public string Desc { get; set; }

        public string Id { get; set; }
        public string Agency { get; set; }
        public string Version { get; set; }

    }
    public class ConceptObject
    {
        public string Name { get; set; }
        public string Desc { get; set; }

        public string Id { get; set; }
        public string Agency { get; set; }
        public string Version { get; set; }

        public List<ItemObject> Items { get; set; }
    }
    public class ItemObject
    {
        public string Name { get; set; }
        public string Desc { get; set; }

        public string Code { get; set; }
        public string Parent { get; set; }
    
    }
    public class CodelistObject
    {
        public string Name { get; set; }
        public string Desc { get; set; }

        public string Id { get; set; }
        public string Agency { get; set; }
        public string Version { get; set; }

        public List<ItemObject> Items { get; set; }
    
    }
    public class DimensionObject
    {
        public string Concept_Id { get; set; }
        public string Concept_Name { get; set; }

        public string ConceptScheme_Id { get; set; }
        public string ConceptScheme_Agency { get; set; }
        public string ConceptScheme_Version { get; set; }

        public string Codelist_Id { get; set; }
        public string Codelist_Agency { get; set; }
        public string Codelist_Version { get; set; }

        public string TextFormat { get; set; }
        public string DimensionType { get; set; }

    }
    public class AttributeObject
    {                             
        public string AttachmentLevel { get; set; }
        public string AssignmentStatus { get; set; }
        public string AttributeType { get; set; }

        public string Concept_Id { get; set; }
        public string Concept_Name { get; set; }

        public string ConceptScheme_Id { get; set; }
        public string ConceptScheme_Agency { get; set; }
        public string ConceptScheme_Version { get; set; }

        public string Codelist_Id { get; set; }
        public string Codelist_Agency { get; set; }
        public string Codelist_Version { get; set; }

        public string TextFormat { get; set; }
    }
    public class MeasureObject
    {
        public string Type { get; set; }
        public string MeasureDimension { get; set; }
        public string Code { get; set; }

        public string Concept_Id { get; set; }
        public string Concept_Name { get; set; }

        public string ConceptScheme_Id { get; set; }
        public string ConceptScheme_Agency { get; set; }
        public string ConceptScheme_Version { get; set; }

        public string Codelist_Id { get; set; }
        public string Codelist_Agency { get; set; }
        public string Codelist_Version { get; set; }

        public string TextFormat { get; set; }
    }

    public class StructureResponceObject
    {
        public DsdObject Dsd { get; set; }
        public List<ConceptObject> Concept { get; set; }
        public List<CodelistObject> Codelist { get; set; }
    }
    public class DsdResponceObject
    {
        public DsdObject Dsd { get; set; }
        public List<DimensionObject> Dimension { get; set; }
        public List<AttributeObject> Attribute { get; set; }
        public List<MeasureObject> Measure { get; set; }
        
    }
}
