using System;
using System.Windows.Forms;
using Utils.Common;
using Utils.Demo;
using Utils.Win32_API;
using WinForm_HandWritingRecognition;

namespace WinFormsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Clear();

                PnPEntityInfo[] allDevice = DeviceUtils.AllUsbDevices;
                foreach (var device in allDevice)
                {
                    richTextBox1.AppendText("设备安装类GUID   ClassGuid    " + device.ClassGuid + FileUtils.NEW_LINE_SPACE);
                    richTextBox1.AppendText("设备描述         Description  " + device.Description + FileUtils.NEW_LINE_SPACE);
                    richTextBox1.AppendText("设备名称         Name         " + device.Name + FileUtils.NEW_LINE_SPACE);
                    richTextBox1.AppendText("设备ID           PNPDeviceID  " + device.PNPDeviceID + FileUtils.NEW_LINE_SPACE);
                    richTextBox1.AppendText("产品编号         ProductID    " + device.ProductID + FileUtils.NEW_LINE_SPACE);
                    richTextBox1.AppendText("供应商标识       VendorID     " + device.VendorID + FileUtils.NEW_LINE_SPACE);
                    richTextBox1.AppendText("服务             Service      " + device.Service + FileUtils.NEW_LINE_SPACE);
                    richTextBox1.AppendText(FileUtils.NEW_LINE_SPACE);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        #region 委托测试
        //委托
        delegate void MyDel(string x, RichTextBox rt);
        //委托调用的外部方法
        event MyDel PrintEvent;

        /// <summary>
        /// 委托测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            DelegateDemo1 p = new DelegateDemo1();

            MyDel del1, del2, del3;

            //赋值
            del1 = p.Print1;
            del2 = p.Print2;

            //组合委托
            del3 = del1 + del2;

            //给委托赋新值
            del1 = p.Print2;

            //给委托添加方法
            del1 += p.Print3;

            //从委托移除方法
            del1 -= p.Print2;

            richTextBox2.AppendText(FileUtils.NEW_LINE_SPACE + "运行单个委托" + FileUtils.NEW_LINE_SPACE);
            //调用
            PrintFun(del1, "运行", richTextBox2);

            richTextBox2.AppendText(FileUtils.NEW_LINE_SPACE + "运行组合委托" + FileUtils.NEW_LINE_SPACE);
            //调用
            PrintFun(del3, "运行", richTextBox2);
        }

        /// <summary>
        /// 执行委托方法
        /// </summary>
        /// <param name="d">委托类型参数</param>
        /// <param name="value">委托内方法的参数</param>
        void PrintFun(MyDel d, string value, RichTextBox richTextBox2)
        {
            d(value, richTextBox2);
        }


        #endregion

        #region 手写识别

        private void button3_Click(object sender, EventArgs e)
        {
            WinForm_HandWritingRecognition.Form1 frm = new WinForm_HandWritingRecognition.Form1();
            frm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WinForm_HandWritingRecognition.InkRecognition frm = new WinForm_HandWritingRecognition.InkRecognition();
            frm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            InkRecognition.Window1 frm = new InkRecognition.Window1();
            frm.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            InkRecognition2.Form1 frm = new InkRecognition2.Form1();
            frm.ShowDialog();

        }

        #endregion



        #region 声音播放
        private void button10_Click(object sender, EventArgs e)
        {
            Sound.Sound1();
        }
        #endregion

        #region Win32 API
        private void button7_Click(object sender, EventArgs e)
        {
            Beeps.Run_MessageBeep();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            Beeps.Run_Beep();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            Beeps.Run_PlaySound();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            Beeps.Random_Beep();
        }
        #endregion

    }
}
