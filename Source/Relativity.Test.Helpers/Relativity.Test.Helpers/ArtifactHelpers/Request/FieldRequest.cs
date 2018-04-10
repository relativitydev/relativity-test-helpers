using kCura.Relativity.Client;
using System;
using DTOs = kCura.Relativity.Client.DTOs;
namespace Relativity.Test.Helpers.ArtifactHelpers.Request
{
    public class FieldRequest
    {
        public int WorkspaceID { get; set; }
        public int ArtifactType { get; set; }
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; }
        public bool AllowGroupBy { get; set; } = false;
        public bool AllowPivot { get; set; } = false;
        public bool AllowSortTally { get; set; } = true;
        public bool IsRequired { get; set; } = false;
        public bool Linked { get; set; } = false;
        public string Width { get; set; } = "";
        public bool IsRelational { get; set; } = false;
        public bool AllowHTML { get; set; } = false;
        public bool IncludeInTextIndex { get; set; } = false;
        public bool OpenToAssociations { get; set; } = false;
        public int Length { get; set; } = 255;
        public bool Unicode { get; set; } = true;
        public bool Wrapping { get; set; } = true;
        public bool AvailableInViewer { get; set; } = false;
        public string YesValue { get; set; } = "Yes";
        public string NoValue { get; set; } = "No";
        public bool AvailableInFieldTree { get; set; } = false;
        public FieldRequest(int workspaceID, FieldType type, int artifactType)
        {
            if (workspaceID == default(int))
            {
                throw new ArgumentNullException(nameof(workspaceID));
            }
            if (type == FieldType.Empty)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (artifactType == default(int))
            {
                throw new ArgumentNullException(nameof(artifactType));
            }
            WorkspaceID = workspaceID;
            FieldType = type;
            ArtifactType = artifactType;
        }
        public FieldRequest(int workspaceID, FieldType type) : this(workspaceID, type, (int)kCura.Relativity.Client.ArtifactType.Document)
        {
        }
        public void HydrateFieldDTO(DTOs.Field fieldDTO)
        {
            hydrateCommonData(fieldDTO);
            hydrateByFieldType(fieldDTO);
        }
        private void hydrateCommonData(DTOs.Field fieldDTO)
        {
            //Set primary fields
            //The name of the sample data is being set to a random string so that sample data can be debugged
            //and never causes collisions. You can set this to any string that you want
            fieldDTO.Name = FieldName ?? $"{FieldType}{Guid.NewGuid()}";
            if (fieldDTO.Name.Length > 50)
            {
                fieldDTO.Name = fieldDTO.Name.Substring(0, 50);
            }
            fieldDTO.FieldTypeID = FieldType;
            fieldDTO.ObjectType = new DTOs.ObjectType
            {
                DescriptorArtifactTypeID = ArtifactType
            };
            fieldDTO.AllowGroupBy = AllowGroupBy;
            fieldDTO.AllowPivot = AllowPivot;
            fieldDTO.AllowSortTally = AllowSortTally;
            fieldDTO.IsRequired = IsRequired;
            fieldDTO.Linked = Linked;
            fieldDTO.Width = Width;
        }
        private void hydrateByFieldType(DTOs.Field fieldDTO)
        {
            switch (FieldType)
            {
                case FieldType.FixedLengthText:
                    //TODO: move to atomic methods
                    fieldDTO.AllowHTML = AllowHTML;
                    fieldDTO.IncludeInTextIndex = IncludeInTextIndex;
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Length = Length;
                    fieldDTO.Unicode = Unicode;
                    fieldDTO.Wrapping = Wrapping;
                    fieldDTO.IsRelational = IsRelational;
                    break;
                case FieldType.WholeNumber:
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Wrapping = Wrapping;
                    break;
                case FieldType.Date:
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Wrapping = Wrapping;
                    break;
                case FieldType.YesNo:
                    fieldDTO.YesValue = YesValue;
                    fieldDTO.NoValue = NoValue;
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Wrapping = true;
                    break;
                case FieldType.LongText:
                    fieldDTO.AllowHTML = AllowHTML;
                    fieldDTO.AvailableInViewer = AvailableInViewer;
                    fieldDTO.IncludeInTextIndex = IncludeInTextIndex;
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Unicode = Unicode;
                    fieldDTO.Wrapping = Wrapping;
                    break;
                case FieldType.SingleChoice:
                    fieldDTO.AvailableInFieldTree = AvailableInFieldTree;
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Unicode = Unicode;
                    fieldDTO.Wrapping = Wrapping;
                    break;
                case FieldType.Decimal:
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Wrapping = Wrapping;
                    break;
                case FieldType.Currency:
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Wrapping = Wrapping;
                    break;
                case FieldType.MultipleChoice:
                    fieldDTO.AvailableInFieldTree = AvailableInFieldTree;
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Unicode = Unicode;
                    fieldDTO.Wrapping = Wrapping;
                    break;
                case FieldType.User:
                    fieldDTO.OpenToAssociations = OpenToAssociations;
                    fieldDTO.Wrapping = Wrapping;
                    break;
                case FieldType.MultipleObject:
                case FieldType.SingleObject:
                case FieldType.File:
                    throw new NotImplementedException();
            }
        }
    }
}