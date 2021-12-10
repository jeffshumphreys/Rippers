using System;

using static ExtensionMethods.SharedConstants;

namespace PackageExpansions
{
    public class CapturedPackageDetailsBeforeLoading
    {
        public string           fileIdentifierGivenByUser;
        public string           expandedPath;
        public FolderOrFileEnum folderorfile;
        public DateTime         originalLastWritten;
        public DateTime         created;
        public long             originalFileSize;
        public byte[]           originalFileMD5Hash;  // Capture algorithm so that we don't accidentally compare different algorithm values in future.
    }
}
