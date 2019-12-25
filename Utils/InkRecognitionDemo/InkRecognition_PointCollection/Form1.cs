using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Ink;
using System.Windows.Ink;
using System.Windows.Input;
using System.IO;

namespace InkRecognition_PointCollection
{
    public partial class Form1 : Form
    {
        InkCollector ic;
        private List<StylusPoint> signPoints;
        private StrokeCollection strokes = new StrokeCollection();
        private StylusPointCollection strokeColl = new StylusPointCollection();
        string[] bpoint = new string[3] { "0", "0", "0" };

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //显示笔迹
            ic = new InkCollector(ink_here.Handle);
            ic.Enabled = true;

            signPoints = new List<StylusPoint>();
        }

        /// <summary>
        /// 通过传递的坐标点转换为笔迹
        /// </summary>
        /// <param name="point"></param>
        private void SignDiscern(string[] point)
        {
            try
            {

                if (int.Parse(point[0]) == 0 && signPoints.Count > 0)
                {
                    foreach (var item in signPoints)
                    {
                        strokeColl.Add(item);
                    }
                    strokes.Add(new System.Windows.Ink.Stroke(strokeColl));
                    strokeColl = new StylusPointCollection();
                    signPoints = new List<StylusPoint>();
                }

                if (int.Parse(point[0]) > 0 && !(bpoint[1] == point[1] && bpoint[2] == point[2]))
                {
                    signPoints.Add(new StylusPoint(double.Parse(point[1]), double.Parse(point[2])));
                    bpoint = point;
                }
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// 鼠标移动记录点信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ink_here_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (e.Button != MouseButtons.None && e.Button != MouseButtons.Left)
            {
                return;
            }

            string[] point = new string[3] { "0", "0", "0" };
            if (e.Button == MouseButtons.Left)
            {
                point[0] = "1";
            }
            point[1] = e.Location.X.ToString();
            point[2] = e.Location.Y.ToString();

            SignDiscern(point);
        }

        /// <summary>
        /// 识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            List<string> resultStrs = Recognize(strokes);
            foreach (var str in resultStrs)
            {
                richTextBox1.AppendText(" " + str + "\r\n");
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (!ic.CollectingInk)
            {
                Strokes strokesToDelete = ic.Ink.Strokes;
                ic.Ink.DeleteStrokes(strokesToDelete);
                ic.Ink.DeleteStrokes();//清除手写区域笔画;
                ink_here.Refresh();//刷新手写区域

                richTextBox1.Clear();

                signPoints = new List<StylusPoint>();
                strokes = new StrokeCollection();
                strokeColl = new StylusPointCollection();
                bpoint = new string[3] { "0", "0", "0" };
            }
        }

        /// <summary>
        /// 笔记识别
        /// </summary>
        /// <param name="strokes"></param>
        /// <returns></returns>
        public List<string> Recognize(StrokeCollection strokes)
        {
            List<string> lstr = new List<string>();
            using (MemoryStream ms = new MemoryStream())
            {
                strokes.Save(ms);
                var ink = new Ink();
                ink.Load(ms.ToArray());
                using (RecognizerContext context = new RecognizerContext())
                {
                    if (ink.Strokes.Count > 0)
                    {
                        context.Strokes = ink.Strokes;
                        RecognitionStatus status;

                        var result = context.Recognize(out status);

                        if (status == RecognitionStatus.NoError)
                        {
                            //lstr.Add(result.TopString);//最可能的识别

                            //获取所有备选词
                            RecognitionAlternates alternates = result.GetAlternatesFromSelection(0, result.TopString.Length, 10);
                            foreach (RecognitionAlternate alternate in alternates)
                            {
                                lstr.Add(alternate.ToString());
                            }
                        }
                        //else
                        //    lstr.Add("识别失败");
                    }
                    //else
                    //    lstr.Add("没有侦测到签名");
                }
                return lstr;
            }

        }

    }
}
