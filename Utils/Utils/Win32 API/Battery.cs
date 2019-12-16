using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Win32_API
{
    /// <summary>
    /// 需要确定我笔记本的电池状况。Win32 为此提供了电源管理函数。
    /// GetSystemPowerStatus() 
    /// </summary>
    class Battery
    {
        [DllImport("kernel32.dll")]
        public static extern bool GetSystemPowerStatus(ref SystemPowerStatus systemPowerStatus);

        public struct SystemPowerStatus
        {
            byte ACLineStatus;
            byte batteryFlag;
            byte batteryLifePercent;
            byte reserved1;
            int batteryLifeTime;
            int batteryFullLifeTime;
        }

        //在此原型中，我们用“ref”指明将传递结构指针而不是结构值。这是处理通过指针传递的结构的一般方法。
        //此函数运行良好，但是最好将 ACLineStatus 和 batteryFlag 字段定义为 enum：
        //请注意，由于结构的字段是一些字节，因此我们使用 byte 作为该 enum 的基本类型。
        enum ACLineStatus : byte
        {
            Offline = 0,
            Online = 1,
            Unknown = 255,
        }

        enum BatteryFlag : byte
        {
            High = 1,
            Low = 2,
            Critical = 4,
            Charging = 8,
            NoSystemBattery = 128,
            Unknown = 255,
        }


        /*
        字符串
        虽然只有一种.NET 字符串类型，但这种字符串类型在非托管应用中却有几项独特之处。可以使用具有内嵌字符数组的字符指针和结构，其中每个数组都需要正确的封送处理。
        在 Win32 中还有两种不同的字符串表示：
        ANSI
        Unicode
        最初的 Windows 使用单字节字符，这样可以节省存储空间，但在处理很多语言时都需要复杂的多字节编码。Windows NT? 出现后，它使用双字节的 Unicode 编码。为解决这一差别，Win32 API 采用了非常聪明的做法。它定义了 TCHAR 类型，该类型在 Win9x 平台上是单字节字符，在 WinNT 平台上是双字节 Unicode 字符。对于每个接受字符串或结构（其中包含字符数据）的函数，Win32 API 均定义了该结构的两种版本，用 A 后缀指明 Ansi 编码，用 W 指明 wide 编码（即 Unicode）。如果您将 C++ 程序编译为单字节，会获得 A 变体，如果编译为 Unicode，则获得 W 变体。Win9x 平台包含 Ansi 版本，而 WinNT 平台则包含 W 版本。
        由于 P/Invoke 的设计者不想让您为所在的平台操心，因此他们提供了内置的支持来自动使用 A 或 W 版本。如果您调用的函数不存在，互操作层将为您查找并使用 A 或 W 版本。
        通过示例能够很好地说明字符串支持的一些精妙之处。
        简单字符串
        下面是一个接受字符串参数的函数的简单示例
        */
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern bool GetDiskFreeSpace(
                                            [MarshalAs(UnmanagedType.LPTStr)]string rootPathName,
                                            ref int sectorsPerCluster,
                                            ref int bytesPerSector,
                                            ref int numberOfFreeClusters,
                                            ref int totalNumberOfClusters
                                            );

    }
}
