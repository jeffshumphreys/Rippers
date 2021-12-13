using System;

using static RipSSIS.SharedConstants;

namespace RipSSIS
{
    public class CapturedPackageDetailsBeforeLoading
    {
        public string           fileIdentifierGivenByUser;
        public string           expandedPath;
        public FolderOrFileEnum folderorfile;
        public DateTime         originalLastWritten;
        public DateTime         created;
        public long             originalFileSize;
        public byte[]           originalFileMD5Hash;  // Capture algorithm used in name so that we don't accidentally compare different algorithm values in future.
    }
}
