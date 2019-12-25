using System;
using System.Windows.Forms;
using Utils.Common;
using Utils.Demo;
using Utils.Win32_API;
using System.Threading;
using System.ComponentModel;
using Utils.ComonForm;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.DeviceNotify.Info;

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

                SetProgressBar(50);
                //foreach (var device in allDevice)
                //{
                //    SetTextBoxValue(device);
                //}
                for (int i = 0; i < allDevice.Length - 1; i++)
                {
                    Thread.Sleep(10);

                    SetTextBoxValue(allDevice[i]);

                    SetProgressBar((int)((double)i / (double)allDevice.Length * 50.00) + 50);
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
        /// USB插拔监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            //判断设备连接情况
            devNotifier = DeviceNotifier.OpenDeviceNotifier();
            devNotifier.OnDeviceNotify += onDevNotify;
        }


        /// <summary>
        /// 设备监听
        /// </summary>
        private IDeviceNotifier devNotifier;

        /// <summary>
        /// 设备事件触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onDevNotify(object sender, DeviceNotifyEventArgs e)
        {
            try
            {
                switch (e.DeviceType)
                {
                    case DeviceType.DeviceInterface:
                        var myDevice = (IUsbDeviceNotifyInfo)e.Object;

                        switch (e.EventType)
                        {
                            case EventType.DeviceArrival:
                                DeviceArrival(myDevice);
                                break;
                            case EventType.DeviceRemoveComplete:
                                DeviceRemove(myDevice);
                                break;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("设备热插拔事件触发异常" + "{0}异常信息：{1}", FileUtils.NEW_LINE_SPACE, ex));
            }
        }

        /// <summary>
        /// 热插拔设备插入 委托
        /// </summary>
        /// <param name="deviceInfo"></param>
        private delegate void DelegateDeviceArrival(IUsbDeviceNotifyInfo deviceInfo);

        /// <summary>
        /// 热插拔设备插入
        /// </summary>
        /// <param name="deviceInfo"></param>
        public void DeviceArrival(IUsbDeviceNotifyInfo deviceInfo)
        {
            try
            {
                string msg = "检测到插入设备！";
                if (richTextBox1.InvokeRequired)
                {
                    // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                    Action<string> actionDelegate = (x) => { this.richTextBox1.AppendText(msg); };
                    // 或者
                    // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                    this.richTextBox1.Invoke(actionDelegate, msg);
                }
                else
                {
                    this.richTextBox1.AppendText(msg);
                }

                SetTextBoxValue2(deviceInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("处理设备接入异常" + "{0}异常信息：{1}", FileUtils.NEW_LINE_SPACE, ex));
            }

        }

        /// <summary>
        /// 热插拔设备拔出 委托
        /// </summary>
        /// <param name="deviceInfo"></param>
        private delegate void DelegateDeviceRemove(IUsbDeviceNotifyInfo deviceInfo);
        /// <summary>
        /// 热插拔设备拔出
        /// </summary>
        /// <param name="deviceInfo"></param>
        public void DeviceRemove(IUsbDeviceNotifyInfo deviceInfo)
        {
            try
            {
                string msg = "检测到拔出设备！";
                if (richTextBox1.InvokeRequired)
                {
                    // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                    Action<string> actionDelegate = (x) => { this.richTextBox1.AppendText(msg); };
                    // 或者
                    // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                    this.richTextBox1.Invoke(actionDelegate, msg);
                }
                else
                {
                    this.richTextBox1.AppendText(msg);
                }

                SetTextBoxValue2(deviceInfo);

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("处理设备移除异常" + "{0}异常信息：{1}", FileUtils.NEW_LINE_SPACE, ex));
            }
        }

        /// <summary>
        /// 结果赋值2
        /// </summary>
        private delegate void SetTextBox2(IUsbDeviceNotifyInfo info);

        /// <summary>
        /// 结果赋值2
        /// </summary>
        private void SetTextBoxValue2(IUsbDeviceNotifyInfo info)
        {
            if (progressBar1.InvokeRequired)
            {
                SetTextBox2 setpos = new SetTextBox2(SetTextBoxValue2);
                this.Invoke(setpos, new object[] { info });
            }
            else
            {
                richTextBox1.AppendText("设备安装类GUID   ClassGuid    " + info.ClassGuid + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText("设备名称         Name         " + info.Name + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText("产品编号         ProductID    " + info.IdProduct + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText("供应商标识       VendorID     " + info.IdVendor + FileUtils.NEW_LINE_SPACE);
                richTextBox1.AppendText(FileUtils.NEW_LINE_SPACE);
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
        private void button16_Click(object sender, EventArgs e)
        {
            InkRecognition_PointCollection.Form1 frm = new InkRecognition_PointCollection.Form1();
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

        #region 窗体样式

        /// <summary>
        /// 进度条1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            this.progressBar2.Visible = true;

            Thread fThread = new Thread(new ThreadStart(RunProcess));
            fThread.Start();
        }

        private void RunProcess()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                SetProgressBar1(i);
            }
        }

        /// <summary>
        /// 进度条1
        /// </summary>
        /// <param name="ipos"></param>
        private delegate void SetPos1(int ipos);

        /// <summary>
        /// 进度条1
        /// </summary>
        /// <param name="ipos"></param>
        private void SetProgressBar1(int ipos)
        {
            if (progressBar2.InvokeRequired)
            {
                SetPos1 setpos = new SetPos1(SetProgressBar1);
                this.Invoke(setpos, new object[] { ipos });
            }
            else
            {
                this.progressBar2.Value = Convert.ToInt32(ipos);
                if (ipos >= 98)
                {
                    this.progressBar2.Visible = false;
                }
            }
        }

        /// <summary>
        /// 进度条2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.RunWorkerAsync(); // 运行 backgroundWorker 组件
            ProcessForm form = new ProcessForm(this.backgroundWorker1);// 显示进度条窗体
            form.ShowDialog(this);
            form.Close();
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

        /// <summary>
        /// 进度条2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 通知栏提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            //BalloonTipIcon —— 气泡提示的类型，有None（无）、Info（蓝色感叹号）、Warnning（黄色感叹号）、Error（小红叉）；BalloonTipText —— 气泡提示的内容，如上图的None等气泡类型信息；
            //BalloonTipTitle —— 气泡提示的标题，如上图的Tip；
            //ContextMenuStrip —— 绑定的右键菜单；
            //Icon —— 所显示的图标；
            //Text —— 鼠标移上去时，显示的提示信息；
            //Visible —— 是否显示图标，当然，不显示就看不到了。

            int timeout = 4000;
            string tiptitle = "提示";
            string showText = "显示提示信息内容！";

            notifyIcon1.Visible = true;
            notifyIcon1.Text = showText;
            notifyIcon1.ShowBalloonTip(0, tiptitle, showText, ToolTipIcon.Info);
            Thread.Sleep(timeout);
            notifyIcon1.Visible = false;
        }

        #endregion


    }
}
