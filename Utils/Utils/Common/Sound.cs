using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utils.Common
{
    //C#中声音的播放主要有六种方法：
    //1.播放系统事件声音 
    //2.使用SoundPlayer
    //3.使用API函数播放
    //4.使用axWindowsMediaPlayer的COM组件来播放
    //5.Microsoft speech object Library
    //6.使用directX

    public class Sound
    {

        //1.播放系统事件声音 
        public static void Sound1()
        {
            System.Media.SystemSounds.Asterisk.Play();
            Thread.Sleep(500);
            System.Media.SystemSounds.Beep.Play();
            Thread.Sleep(500);
            System.Media.SystemSounds.Exclamation.Play();
            Thread.Sleep(500);
            System.Media.SystemSounds.Hand.Play();
            Thread.Sleep(500);
            System.Media.SystemSounds.Question.Play();
        }

    }
}
