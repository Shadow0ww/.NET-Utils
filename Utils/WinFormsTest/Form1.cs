using System;
using System.Windows.Forms;
using Utils.Common;
using Utils.Demo;
using Utils.Win32_API;
using System.Threading;
using System.ComponentModel;
using Utils.ComonForm;

namespace WinFormsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 设备检测
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            this.progressBar1.Visible = true;

            Thread fThread = new Thread(new ThreadStart(AllUsbDevices));
            fThread.Start();

        }

        private void button12_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.RunWorkerAsync(); // 运行 backgroundWorker 组件
            ProcessForm form = new ProcessForm(this.backgroundWorker1);// 显示进度条窗体
            form.ShowDialog(this);
            form.Close();
        }
        /// <summary>
        /// 设备检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllUsbDevices()
        {
            try
            {
                SetProgressBar(0);

                PnPEntityInfo[] allDevice = DeviceUtils.AllUsbDevices;
                //foreach (var device in allDevice)
                //{
                //    SetTextBoxValue(device);
                //}
                for (int i = 0; i < allDevice.Length - 1; i++)
                {
                    Thread.Sleep(100);

                    SetTextBoxValue(allDevice[i]);

                    SetProgressBar((int)((double)i / (double)allDevice.Length * 100.00));
                }


                SetProgressBar(100);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 进度条
        /// </summary>
        /// <param name="ipos"></param>
        private delegate void SetPos(int ipos);

        ///代理
        /// <summary>
        /// 进度条
        /// </summary>
        /// <param name="ipos"></param>
        private void SetProgressBar(int ipos)
        {
            if (progressBar1.InvokeRequired)
            {
                SetPos setpos = new SetPos(SetProgressBar);
                this.Invoke(setpos, new object[] { ipos });
            }
            else
            {
                this.progressBar1.Value = Convert.ToInt32(ipos);
                if (ipos == 100)
                {
                    this.progressBar1.Visible = false;
                }
            }
        }

        /// <summary>
        /// 结果赋值
        /// </summary>
        private delegate void SetTextBox(PnPEntityInfo info);

        ///代理
        /// <summary>
        /// 结果赋值
        /// </summary>
        private void SetTextBoxValue(PnPEntityInfo info)
        {
            if (progressBar1.InvokeRequired)
            {
                SetTextBox setpos = new SetTextBox(SetTextBoxValue);
                this.Invoke(setpos, new object[] { info });
            }
            else
            {
                richTextBox1.AppendText("设备安装类GUID   ClassGuid    " + info.ClassGuid + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText("设备描述         Description  " + info.Description + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText("设备名称         Name         " + info.Name + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText("设备ID           PNPDeviceID  " + info.PNPDeviceID + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText("产品编号         ProductID    " + info.ProductID + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText("供应商标识       VendorID     " + info.VendorID + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText("服务             Service      " + info.Service + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText(FileUtils.NEW_LINE_SPACE);
            }
        }

        /// <summary>
        /// 进度条2
        /// 你可以在这个方法内，实现你的调用，方法等。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                worker.ReportProgress(i);
                if (worker.CancellationPending)  // 如果用户取消则跳出处理数据代码 
                {
                    e.Cancel = true;
                    break;
                }
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
            }
            else
            {
            }
        }
        #endregion

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
            InkRecognition_Base.Form1 frm = new InkRecognition_Base.Form1();
            frm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            InkRecognition_Base.InkRecognition frm = new InkRecognition_Base.InkRecognition();
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
