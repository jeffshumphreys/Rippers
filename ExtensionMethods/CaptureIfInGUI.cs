using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    // referenced from https://stackoverflow.com/a/8711036/147511

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#machine-types
    // !! BEWARE: this enum is 'ushort' sized --
    //    do not replace it 'System.Reflection.ImageFileMachine' (which is 'int' sized) !!
    public enum ImageFileMachine : ushort
    {
        Unknown         /**/ = 0x0000,
        I386            /**/ = 0x014C,
        WceMipsV2       /**/ = 0x0169,
        Alpha           /**/ = 0x0184,
        SH3             /**/ = 0x01A2,
        SH3Dsp          /**/ = 0x01A3,
        SH3E            /**/ = 0x01A4,
        SH4             /**/ = 0x01A6,
        SH5             /**/ = 0x01A8,
        Arm             /**/ = 0x01C0,
        Thumb           /**/ = 0x01C2,
        ArmThumb2       /**/ = 0x01C4,
        AM33            /**/ = 0x01D3,
        PowerPC         /**/ = 0x01F0,
        PowerPCFP       /**/ = 0x01F1,
        IA64            /**/ = 0x0200,
        MIPS16          /**/ = 0x0266,
        Alpha64         /**/ = 0x0284,
        MipsFpu         /**/ = 0x0366,
        MipsFpu16       /**/ = 0x0466,
        Tricore         /**/ = 0x0520,
        Ebc             /**/ = 0x0EBC,
        Amd64           /**/ = 0x8664,
        M32R            /**/ = 0x9041,
    };

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#characteristics
    [Flags]
    public enum ImageFileCharacteristics : ushort
    {
        RelocsStripped          /**/ = 0x0001,
        ExecutableImage         /**/ = 0x0002,
        LineNumsStripped        /**/ = 0x0004,
        LocalSymsStripped       /**/ = 0x0008,
        AggressiveWSTrim        /**/ = 0x0010,
        LargeAddressAware       /**/ = 0x0020,
        BytesReversedLo         /**/ = 0x0080,
        Bit32Machine            /**/ = 0x0100,
        DebugStripped           /**/ = 0x0200,
        RemovableRunFromSwap    /**/ = 0x0400,
        NetRunFromSwap          /**/ = 0x0800,
        System                  /**/ = 0x1000,
        Dll                     /**/ = 0x2000,
        UpSystemOnly            /**/ = 0x4000,
        BytesReversedHi         /**/ = 0x8000,
    };

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#windows-subsystem
    public enum Subsystem : ushort
    {
        Unknown                 /**/ = 0x0000,
        Native                  /**/ = 0x0001,
        WindowsGui              /**/ = 0x0002,
        WindowsCui              /**/ = 0x0003,
        OS2Cui                  /**/ = 0x0005,
        PosixCui                /**/ = 0x0007,
        NativeWindows           /**/ = 0x0008,
        WindowsCEGui            /**/ = 0x0009,
        EfiApplication          /**/ = 0x000A,
        EfiBootServiceDriver    /**/ = 0x000B,
        EfiRuntimeDriver        /**/ = 0x000C,
        EfiRom                  /**/ = 0x000D,
        Xbox                    /**/ = 0x000E,
        WindowsBootApplication  /**/ = 0x0010,
    };

    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#dll-characteristics
    [Flags]
    public enum DllCharacteristics : ushort
    {
        ProcessInit     /**/ = 0x0001,
        ProcessTerm     /**/ = 0x0002,
        ThreadInit      /**/ = 0x0004,
        ThreadTerm      /**/ = 0x0008,
        HighEntropyVA   /**/ = 0x0020,
        DynamicBase     /**/ = 0x0040,
        NxCompatible    /**/ = 0x0100,
        NoIsolation     /**/ = 0x0200,
        NoSeh           /**/ = 0x0400,
        NoBind          /**/ = 0x0800,
        AppContainer    /**/ = 0x1000,
        WdmDriver       /**/ = 0x2000,
        TSAware         /**/ = 0x8000,
    };
    
    class CaptureIfInGUI
    {
    }
}
