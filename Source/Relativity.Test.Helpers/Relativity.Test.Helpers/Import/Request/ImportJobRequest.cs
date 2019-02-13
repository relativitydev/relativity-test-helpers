using kCura.Relativity.Client;
using kCura.Relativity.DataReaderClient;
using System;
using System.Text;
using static Relativity.Test.Helpers.Import.Constants;

namespace Relativity.Test.Helpers.Import.Request
{
    /// <summary>
    /// Request for an import job
    /// </summary>
    public class ImportJobRequest
    {
        /// <summary>
        /// Target workspace for Import
        /// </summary>
        public int WorkspaceID { get; }
        /// <summary>
        /// Target object type id for import (Defaulted to Document)
        /// </summary>
        public int ArtifactTypeID { get; }
        public bool ExtractedTextFieldContainsFilePath { get; set; } = false;
        public bool DisableExtractedTextEncodingCheck { get; set; } = true;
        public bool DisableExtractedTextFileLocationValidation { get; set; } = true;
        public bool DisableNativeLocationValidation { get; set; } = true;
        public bool DisableNativeValidation { get; set; } = true;
        public NativeFileCopyModeEnum NativeFileCopyMode { get; set; } = NativeFileCopyModeEnum.CopyFiles;
        public OverwriteModeEnum OverwriteMode { get; set; } = OverwriteModeEnum.Append;
        public Encoding ExtractedTextEncoding { get; set; } = Encoding.GetEncoding(ENCODING_ISO_8859_1_NAME);
        public string ParentObjectIdSourceFieldName { get; set; }
        /// <summary>
        /// Constructor with workspace id and object type id
        /// </summary>
        /// <param name="workspaceID">target workspace</param>
        /// <param name="artifactTypeID">target type</param>
        public ImportJobRequest(int workspaceID, int artifactTypeID)
        {
            if (workspaceID == default(int))
            {
                throw new ArgumentNullException(nameof(workspaceID));
            }
            if (artifactTypeID <= default(int))
            {
                throw new ArgumentNullException(nameof(artifactTypeID));
            }
            WorkspaceID = workspaceID;
            ArtifactTypeID = artifactTypeID;
        }
        /// <summary>
        /// Constructor with workspace id
        /// </summary>
        /// <param name="workspaceID">target workspace</param>
        public ImportJobRequest(int workspaceID) : this(workspaceID, (int)ArtifactType.Document)
        {
        }

        public void HydrateImportJob(ImportBulkArtifactJob importJob)
        {
            importJob.Settings.CaseArtifactId = WorkspaceID;
            importJob.Settings.ExtractedTextFieldContainsFilePath = ExtractedTextFieldContainsFilePath;
            importJob.Settings.DisableExtractedTextEncodingCheck = DisableExtractedTextEncodingCheck;
            importJob.Settings.DisableExtractedTextFileLocationValidation = DisableExtractedTextFileLocationValidation;
            importJob.Settings.NativeFilePathSourceFieldName = NATIVE_FILE;
            importJob.Settings.DisableNativeLocationValidation = DisableNativeLocationValidation;
            importJob.Settings.DisableNativeValidation = DisableNativeValidation;
            importJob.Settings.NativeFileCopyMode = NativeFileCopyMode;
            importJob.Settings.OverwriteMode = OverwriteMode;
            importJob.Settings.ExtractedTextEncoding = ExtractedTextEncoding;
            if (!string.IsNullOrEmpty(ParentObjectIdSourceFieldName))
            {
                importJob.Settings.ParentObjectIdSourceFieldName = ParentObjectIdSourceFieldName;
            }

        }
    }
}
