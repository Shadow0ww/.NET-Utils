using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utils.ComonForm
{
    public partial class ProcessForm : Form
    {
        public ProcessForm(BackgroundWorker backgroundWorker1)
        {
            InitializeComponent();

            this.backgroundWorker1 = backgroundWorker1;
            this.backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();//执行完之后，直接关闭页面
        }
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.backgroundWorker1.CancelAsync();
            this.button1.Enabled = false;
            this.Close();
        }

        private void ProcessForm_Load(object sender, EventArgs e)
        {
            //this.backgroundWorker1.RunWorkerAsync(); // 运行 backgroundWorker 组件
        }
    }
}
