using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Utils.Win32_API
{
    /// <summary>
    /// 介绍Win32Api基本使用方法和常用数据类型转换
    /// </summary>
    class A_DataType
    {
        //===================================================================
        //===================     1.平台调用      ===========================   
        /*
        1.平台调用(P/Invoke) 是完成这一任务的最常用方法。要使用 P/Invoke，您可以编写一个描述如何调用函数的原型，然后运行时将使用此信息进行调用。另一种方法是使用 Managed Extensions to C++ 来包装函数，这部分内容将在以后的专栏中介绍。
        在第一个示例中，我们将调用 Beep() API 来发出声音。首先，我需要为 Beep() 编写适当的定义。查看 MSDN 中的定义，我发现它具有以下原型：
        
        BOOL Beep(
                  DWORD dwFreq,　　　// 声音频率
　                DWORD dwDuration　 // 声音持续时间
                  );

        要用 C# 来编写这一原型，需要将 Win32 类型转换成相应的 C# 类型。
        由于 DWORD 是 4 字节的整数，因此我们可以使用 int 或 uint 作为 C# 对应类型。 由于 int 是 CLS 兼容类型（可以用于所有 .NET 语言），以此比 uint 更常用，并且在多数情况下，它们之间的区别并不重要。
        bool 类型与 BOOL 对应。
        现在我们可以用 C# 编写以下原型：
        public static extern bool Beep(int frequency, int duration);
        */

        //这是相当标准的定义，只不过我们使用了 extern 来指明该函数的实际代码在别处。此原型将告诉运行时如何调用函数；现在我们需要告诉它在何处找到该函数。
        //我们需要回顾一下 MSDN 中的代码。在参考信息中，我们发现 Beep() 是在 kernel32.lib 中定义的。这意味着运行时代码包含在 kernel32.dll 中。我们在原型中添加 DllImport 属性将这一信息告诉运行时：
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int frequency, int duration);
        //由于 DllImport 允许您调用 Win32 中的任何代码，因此就有可能调用恶意代码。所以您必须是完全受信任的用户，运行时才能进行 P/Invoke 调用。
        //下面是一个完整的示例，它生成的随机声音在二十世纪六十年代的科幻电影中很常见
        public static void run_0()
        {
            Random random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                Beep(random.Next(10000), 100);
            }
        }




        //=====================================================================
        //===================     2.枚举和常量      =========================== 
        /*2.枚举和常量
         Beep() 可用于发出任意声音，但有时我们希望发出特定类型的声音，因此我们改用 MessageBeep()。MSDN 给出了以下原型：

         BOOL MessageBeep(
                          UINT uType // 声音类型
                          );
        
        首先，uType 参数实际上接受一组预先定义的常量。
　　    其次，可能的参数值包括 -1，这意味着尽管它被定义为 uint 类型，但 int 会更加适合。
        对于 uType 参数，使用 enum 类型是合乎情理的。MSDN 列出了已命名的常量，但没有就具体值给出任何提示。由于这一点，我们需要查看实际的 API。
        如果您安装了 Visual Studio? 和 C++，则 Platform SDK 位于 \Program Files\Microsoft Visual Studio .NET\Vc7\PlatformSDK\Include 下。
　　    为查找这些常量，我在该目录中执行了一个 findstr。
　　    findstr "MB_ICONHAND" *.h
　　    它确定了常量位于 winuser.h 中，然后我使用这些常量来创建我的 enum 和原型：
        */
        public enum BeepType
        {
            SimpleBeep = -1,
            IconAsterisk = 0x00000040,
            IconExclamation = 0x00000030,
            IconHand = 0x00000010,
            IconQuestion = 0x00000020,
            Ok = 0x00000000,
        }

        [DllImport("user32.dll")]
        public static extern bool MessageBeep(BeepType beepType);





        //===================================================================
        //===================     3.处理结构      =========================== 
        /*3.处理结构
　       有时我需要确定我笔记本的电池状况。Win32 为此提供了电源管理函数。
　　     搜索 MSDN 可以找到 GetSystemPowerStatus() 函数。

            BOOL GetSystemPowerStatus(
            　                        LPSYSTEM_POWER_STATUS lpSystemPowerStatus
                                     );

　　     此函数包含指向某个结构的指针，我们尚未对此进行过处理。要处理结构，我们需要用 C# 定义结构。我们从非托管的定义开始：

            typedef struct _SYSTEM_POWER_STATUS 
            {
                 BYTE　 ACLineStatus;
                 BYTE　 BatteryFlag;
                 BYTE　 BatteryLifePercent;
                 BYTE　 Reserved1;
                 DWORD　BatteryLifeTime;
                 DWORD　BatteryFullLifeTime;
            } SYSTEM_POWER_STATUS, *LPSYSTEM_POWER_STATUS;

          然后，通过用 C# 类型代替 C 类型来得到 C# 版本。
        */
        public struct SystemPowerStatus
        {
            byte ACLineStatus;
            byte batteryFlag;
            byte batteryLifePercent;
            byte reserved1;
            int batteryLifeTime;
            int batteryFullLifeTime;
        }
        [DllImport("kernel32.dll")]
        public static extern bool GetSystemPowerStatus(
                                                        ref SystemPowerStatus systemPowerStatus
                                                       );

        //在此原型中，我们用“ref”指明将传递结构指针而不是结构值。这是处理通过指针传递的结构的一般方法。
        //此函数运行良好，但是最好将 ACLineStatus 和 batteryFlag 字段定义为 enum：
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
        //请注意，由于结构的字段是一些字节，因此我们使用 byte 作为该 enum 的基本类型。






        //=================================================================
        //===================     4.字符串      =========================== 
        /*4.字符串
          虽然只有一种 .NET 字符串类型，但这种字符串类型在非托管应用中却有几项独特之处。可以使用具有内嵌字符数组的字符指针和结构，其中每个数组都需要正确的封送处理。
　　      在 Win32 中还有两种不同的字符串表示：
　　          ANSI
　　          Unicode
　　      最初的 Windows 使用单字节字符，这样可以节省存储空间，但在处理很多语言时都需要复杂的多字节编码。Windows NT? 出现后，它使用双字节的 Unicode 编码。为解决这一差别，Win32 API 采用了非常聪明的做法。它定义了 TCHAR 类型，该类型在 Win9x 平台上是单字节字符，在 WinNT 平台上是双字节 Unicode 字符。对于每个接受字符串或结构（其中包含字符数据）的函数，Win32 API 均定义了该结构的两种版本，用 A 后缀指明 Ansi 编码，用 W 指明 wide 编码（即 Unicode）。如果您将 C++ 程序编译为单字节，会获得 A 变体，如果编译为 Unicode，则获得 W 变体。Win9x 平台包含 Ansi 版本，而 WinNT 平台则包含 W 版本。
　　      由于 P/Invoke 的设计者不想让您为所在的平台操心，因此他们提供了内置的支持来自动使用 A 或 W 版本。如果您调用的函数不存在，互操作层将为您查找并使用 A 或 W 版本。
　　      通过示例能够很好地说明字符串支持的一些精妙之处。
　　      简单字符串
　　      下面是一个接受字符串参数的函数的简单示例：

            BOOL GetDiskFreeSpace(
                                 LPCTSTR lpRootPathName, 　　　　// 根路径
                                 LPDWORD lpSectorsPerCluster,　　// 每个簇的扇区数
                                 LPDWORD lpBytesPerSector,　　　 // 每个扇区的字节数
                                 LPDWORD lpNumberOfFreeClusters, // 可用的扇区数
                                 LPDWORD lpTotalNumberOfClusters // 扇区总数
                                 );

          根路径定义为 LPCTSTR。这是独立于平台的字符串指针。
　　      由于不存在名为 GetDiskFreeSpace() 的函数，封送拆收器将自动查找“A”或“W”变体，并调用相应的函数。我们使用一个属性来告诉封送拆收器，API 所要求的字符串类型。
　　      以下是该函数的完整定义，就象我开始定义的那样：
        */

        //[DllImport("kernel32.dll")]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern bool GetDiskFreeSpace(
                                            [MarshalAs(UnmanagedType.LPTStr)] string rootPathName,
                                            ref int sectorsPerCluster,
                                            ref int bytesPerSector,
                                            ref int numberOfFreeClusters,
                                            ref int totalNumberOfClusters
                                            );

        /*
          不幸的是，当我试图运行时，该函数不能执行。问题在于，无论我们在哪个平台上，封送拆收器在默认情况下都试图查找 API 的 Ansi 版本，由于 LPTStr 意味着在 Windows NT 平台上会使用 Unicode 字符串，因此试图用 Unicode 字符串来调用 Ansi 函数就会失败。
　　      有两种方法可以解决这个问题：一种简单的方法是删除 MarshalAs 属性。如果这样做，将始终调用该函数的 A 版本，如果在您所涉及的所有平台上都有这种版本，这是个很好的方法。
          但是，这会降低代码的执行速度，因为封送拆收器要将 .NET 字符串从 Unicode 转换为多字节，然后调用函数的 A 版本（将字符串转换回 Unicode），最后调用函数的 W 版本。
　　      要避免出现这种情况，您需要告诉封送拆收器，要它在 Win9x 平台上时查找 A 版本，而在 NT 平台上时查找 W 版本。要实现这一目的，可以将 CharSet 设置为 DllImport 属性的一部分：

          [DllImport("kernel32.dll", CharSet = CharSet.Auto)] 

　　      在我的非正式计时测试中，我发现这一做法比前一种方法快了大约百分之五。
　　      对于大多数 Win32 API，都可以对字符串类型设置 CharSet 属性并使用 LPTStr。但是，还有一些不采用 A/W 机制的函数，对于这些函数必须采取不同的方法。
        */




        //=======================================================================
        //===================     5.字符串缓冲区      =========================== 
        /*5.字符串缓冲区
          .NET 中的字符串类型是不可改变的类型，这意味着它的值将永远保持不变。对于要将字符串值复制到字符串缓冲区的函数，字符串将无效。
          这样做至少会破坏由封送拆收器在转换字符串时创建的临时缓冲区；严重时会破坏托管堆，而这通常会导致错误的发生。无论哪种情况都不可能获得正确的返回值。
　　      要解决此问题，我们需要使用其他类型。StringBuilder 类型就是被设计为用作缓冲区的，我们将使用它来代替字符串。下面是一个示例：
        */

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(
                                                  [MarshalAs(UnmanagedType.LPTStr)] string path,
                                                  [MarshalAs(UnmanagedType.LPTStr)] StringBuilder shortPath,
                                                  int shortPathLength
                                                 );
        //使用此函数很简单：
        //请注意，StringBuilder 的 Capacity 传递的是缓冲区大小。
        public static void run_1()
        {
            StringBuilder shortPath = new StringBuilder(80);
            int result = GetShortPathName(@"d:\test.jpg", shortPath, shortPath.Capacity);
            string s = shortPath.ToString();
        }






        //=======================================================================
        //===================     6.具有内嵌字符数组的结构      =========================== 
        /*6.具有内嵌字符数组的结构
          某些函数接受具有内嵌字符数组的结构。例如，GetTimeZoneInformation() 函数接受指向以下结构的指针：

            typedef struct _TIME_ZONE_INFORMATION {
            　　LONG　　　 Bias;
            　　WCHAR　　　StandardName[ 32 ];
            　　SYSTEMTIME StandardDate;
            　　LONG　　　 StandardBias;
            　　WCHAR　　　DaylightName[ 32 ];
            　　SYSTEMTIME DaylightDate;
            　　LONG　　　 DaylightBias;
            } TIME_ZONE_INFORMATION, *PTIME_ZONE_INFORMATION;

　　      在 C# 中使用它需要有两种结构。一种是 SYSTEMTIME，它的设置很简单：
        */
        struct SystemTime
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }
        //这里没有什么特别之处；另一种是 TimeZoneInformation，它的定义要复杂一些：
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct TimeZoneInformation
        {
            public int bias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string standardName;
            SystemTime standardDate;
            public int standardBias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string daylightName;
            SystemTime daylightDate;
            public int daylightBias;
        }

        /* 
            此定义有两个重要的细节。第一个是 MarshalAs 属性：
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] 
            查看 ByValTStr 的文档，我们发现该属性用于内嵌的字符数组；另一个是 SizeConst，它用于设置数组的大小。
            我在第一次编写这段代码时，遇到了执行引擎错误。通常这意味着部分互操作覆盖了某些内存，表明结构的大小存在错误。我使用 Marshal.SizeOf() 来获取所使用的封送拆收器的大小，结果是 108 字节。我进一步进行了调查，很快回忆起用于互操作的默认字符类型是 Ansi 或单字节。而函数定义中的字符类型为 WCHAR，是双字节，因此导致了这一问题。
            我通过添加 StructLayout 属性进行了更正。结构在默认情况下按顺序布局，这意味着所有字段都将以它们列出的顺序排列。CharSet 的值被设置为 Unicode，以便始终使用正确的字符类型。
            经过这样处理后，该函数一切正常。您可能想知道我为什么不在此函数中使用 CharSet.Auto。这是因为，它也没有 A 和 W 变体，而始终使用 Unicode 字符串，因此我采用了上述方法编码。
        */





        //=======================================================================
        //===================     7.具有回调的函数      =========================== 
        /*7.具有回调的函数
            当 Win32 函数需要返回多项数据时，通常都是通过回调机制来实现的。开发人员将函数指针传递给函数，然后针对每一项调用开发人员的函数。
　　        在 C# 中没有函数指针，而是使用“委托”，在调用 Win32 函数时使用委托来代替函数指针。
　　        EnumDesktops() 函数就是这类函数的一个示例：

            BOOL EnumDesktops(
　                            HWINSTA hwinsta,　　　　　　 // 窗口实例的句柄
　                            DESKTOPENUMPROC lpEnumFunc,　// 回调函数
　                            LPARAM lParam　　　　　　　　// 用于回调函数的值
                            );

　　        HWINSTA 类型由 IntPtr 代替，而 LPARAM 由 int 代替。DESKTOPENUMPROC 所需的工作要多一些。下面是 MSDN 中的定义：

            BOOL CALLBACK EnumDesktopProc(
　                                        LPTSTR lpszDesktop,　// 桌面名称
　                                        LPARAM lParam　　　　// 用户定义的值
                                        );

　　      我们可以将它转换为以下委托：
        */
        delegate bool EnumDesktopProc([MarshalAs(UnmanagedType.LPTStr)] string desktopName, int lParam);

        //完成该定义后，我们可以为 EnumDesktops() 编写以下定义：
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool EnumDesktops(
                                         IntPtr windowStation,
                                         EnumDesktopProc callback,
                                         int lParam
                                        );

        //这样该函数就可以正常运行了。
        //在互操作中使用委托时有个很重要的技巧：封送拆收器创建了指向委托的函数指针，该函数指针被传递给非托管函数。但是，封送拆收器无法确定非托管函数要使用函数指针做些什么，因此它假定函数指针只需在调用该函数时有效即可。
        //结果是如果您调用诸如 SetConsoleCtrlHandler() 这样的函数，其中的函数指针将被保存以便将来使用，您就需要确保在您的代码中引用委托。如果不这样做，函数可能表面上能执行，但在将来的内存回收处理中会删除委托，并且会出现错误。







        //=======================================================================
        //===================     8.其他高级函数      =========================== 
        /*8.其他高级函数
            迄今为止我列出的示例都比较简单，但是还有很多更复杂的 Win32 函数。下面是一个示例：

                DWORD SetEntriesInAcl(
　                                     ULONG cCountOfExplicitEntries,　　　　　　// 项数
　                                     PEXPLICIT_ACCESS pListOfExplicitEntries, // 缓冲区
　                                     PACL OldAcl,　　　　　　　　　　　　　　 // 原始 ACL
　                                     PACL *NewAcl　　　　　　　　　　　　　　　// 新 ACL
                                     );

              前两个参数的处理比较简单：ulong 很简单，并且可以使用 UnmanagedType.LPArray 来封送缓冲区。
　　          但第三和第四个参数有一些问题。问题在于定义 ACL 的方式。ACL 结构仅定义了 ACL 标头，而缓冲区的其余部分由 ACE 组成。ACE 可以具有多种不同类型，并且这些不同类型的 ACE 的长度也不同。
　　          如果您愿意为所有缓冲区分配空间，并且愿意使用不太安全的代码，则可以用 C# 进行处理。但工作量很大，并且程序非常难调试。而使用 C++ 处理此 API 就容易得多。
        */



        //=======================================================================
        //===================     9.属性的其他选项      =========================== 
        /*9.属性的其他选项
　　          DLLImport 和 StructLayout 属性具有一些非常有用的选项，有助于 P/Invoke 的使用。下面列出了所有这些选项：

　　          DLLImport
　　          CallingConvention
　　          您可以用它来告诉封送拆收器，函数使用了哪些调用约定。您可以将它设置为您的函数的调用约定。通常，如果此设置错误，代码将不能执行。但是，如果您的函数是 Cdecl 函数，并且使用 StdCall（默认）来调用该函数，那么函数能够执行，但函数参数不会从堆栈中删除，这会导致堆栈被填满。
　　          
              CharSet
　　          控制调用 A 变体还是调用 W 变体。

　　          EntryPoint
　　          此属性用于设置封送拆收器在 DLL 中查找的名称。设置此属性后，您可以将 C# 函数重新命名为任何名称。

　　          ExactSpelling
　　          将此属性设置为 true，封送拆收器将关闭 A 和 W 的查找特性。

　　          PreserveSig
　　          COM 互操作使得具有最终输出参数的函数看起来是由它返回的该值。此属性用于关闭这一特性。

　　          SetLastError
　　          确保调用 Win32 API SetLastError()，以便您找出发生的错误。

　　          StructLayout
　　          LayoutKind
　　          结构在默认情况下按顺序布局，并且在多数情况下都适用。如果需要完全控制结构成员所放置的位置，可以使用 LayoutKind.Explicit，然后为每个结构成员添加 FieldOffset 属性。当您需要创建 union 时，通常需要这样做。

　　          CharSet
　　          控制 ByValTStr 成员的默认字符类型。

　　          Pack
　　          设置结构的压缩大小。它控制结构的排列方式。如果 C 结构采用了其他压缩方式，您可能需要设置此属性。

　　          Size
　　          设置结构大小。不常用；但是如果需要在结构末尾分配额外的空间，则可能会用到此属性。
        */




        //=======================================================================
        //===================     10.从不同位置加载      =========================== 
        /*10.从不同位置加载
　　          您无法指定希望 DLLImport 在运行时从何处查找文件，但是可以利用一个技巧来达到这一目的。
            　DllImport 调用 LoadLibrary() 来完成它的工作。如果进程中已经加载了特定的 DLL，那么即使指定的加载路径不同，LoadLibrary() 也会成功。

　　          这意味着如果直接调用 LoadLibrary()，您就可以从任何位置加载 DLL，然后 DllImport LoadLibrary() 将使用该 DLL。
　　          由于这种行为，我们可以提前调用 LoadLibrary()，从而将您的调用指向其他 DLL。如果您在编写库，可以通过调用 GetModuleHandle() 来防止出现这种情况，以确保在首次调用 P/Invoke 之前没有加载该库。

        */




        //=======================================================================
        //===================     11.P/Invoke 疑难解答      =========================== 
        /*11.P/Invoke 疑难解答
　　          如果您的 P/Invoke 调用失败，通常是因为某些类型的定义不正确。以下是几个常见问题：

　　          1.long != long。在 C++ 中，long 是 4 字节的整数，但在 C# 中，它是 8 字节的整数。
　　          2.字符串类型设置不正确。

        */
    }
}
