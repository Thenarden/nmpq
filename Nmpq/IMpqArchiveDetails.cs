using System.Collections.Generic;
using Nmpq.Parsing;

namespace Nmpq
{
    public interface IMpqArchiveDetails
    {
        ArchiveHeader ArchiveHeader { get; }
        MpqHashTable HashTable { get; }
        IList<BlockTableEntry> BlockTable { get; }

        int SectorSize { get; }
    }
}