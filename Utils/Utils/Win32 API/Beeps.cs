using System;
using System.Runtime.InteropServices;

namespace Utils.Win32_API
{
    public class Beeps
    {


        //使用MessageBeep(unit uType) :
        //对于 uType 参数，使用 enum 类型是合乎情理的。MSDN 列出了已命名的常量，但没有就具体值给出任何提示。由于这一点，我们需要查看实际的 API。
        public enum BeepType
        {
            SimpleBeep = -1,
            IconAsterisk = 0x00000040,
            IconExclamation = 0x00000030,
            IconHand = 0x00000010,
            IconQuestion = 0x00000020,
            Ok = 0x00000000,
        }
        public const int MB_ICONEXCLAMATION = 48;

        [DllImport("user32.dll")]
        public static extern bool MessageBeep(uint uType);

        public static void Run_MessageBeep()
        {
            MessageBeep(MB_ICONEXCLAMATION);
        }

        //调用Beep(Int freq, int duration)函数

        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);

        public static void Run_Beep()
        {
            Beep(800, 300);
        }

        //调用PlaySound(string pszSound, int hmod, int fdwSound)
        [DllImport("winmm.dll")]
        public static extern bool PlaySound(string pszSound, int hmod, int fdwSound);
        public const int SND_FILENAME = 0x00020000;
        public const int SND_ASYNC = 0x0001;
        public static void Run_PlaySound()
        {
            PlaySound("一度だけの恋なら.WAV", 0, SND_ASYNC | SND_FILENAME);
        }

        public static void Random_Beep()
        {
            Random random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                Beep(random.Next(10000), 100);
            }
        }

    }
}
