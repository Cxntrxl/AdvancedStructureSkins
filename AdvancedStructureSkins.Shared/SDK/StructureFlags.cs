using System;

namespace AdvancedStructureSkins.Shared.SDK
{
    [Flags]
    public enum StructureFlags : byte
    {
        None = 0,
    
        Disc = 1 << 0,
        Pillar = 1 << 1,
        Ball = 1 << 2,
        Cube = 1 << 3,
        Wall = 1 << 4,
        SmallRock = 1 << 5,
        LargeRock = 1 << 6,
    
        Global = 1 << 7
    }
}

