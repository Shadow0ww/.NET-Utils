

/*
下載：
//Microsoft Windows XP Tablet PC Edition 2005 Recognizer Pack http://www.microsoft.com/zh-cn/download/details.aspx?id=1601  //包各种语言包
//Microsoft Windows XP Tablet PC Edition Software Development Kit 1.7  http://www.microsoft.com/en-us/download/details.aspx?id=20039
* Microsoft Speech Platform - Software Development Kit (SDK) (Version 11) http://www.microsoft.com/en-us/download/details.aspx?id=27226
* Microsoft SDKs http://msdn.microsoft.com/en-us/dd299405.aspx
*Microsoft.Ink 命名空间  http://msdn.microsoft.com/zh-cn/library/microsoft.ink%28v=vs.90%29.aspx
* Microsoft Ink Desktop for Windows Vista http://www.microsoft.com/zh-CN/download/details.aspx?id=6023
* http://msdn.microsoft.com/en-us/data/ms695600(v=vs.71)
* Microsoft.Ink.dll CLR2.0 Update (KB900722)  http://www.microsoft.com/zh-cn/download/details.aspx?id=22557
* Update for Tablet PC Microsoft Ink and .Net Framework 2.0 compatibility.
* Microsoft.Ink

The following tables list the members exposed by the Microsoft.Ink namespace.
* http://msdn.microsoft.com/en-us/library/ms826516.aspx
*
*  安裝的文件在：//Program Files\Microsoft Tablet PC Platform SDK\Include\Microsoft.Ink.dll
*  在Windows XP sp3 環境下測試
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Ink;////引用：Micosoft Tablet PC
                    //using Microsoft.Ink.Analysis;//Windows Vista



namespace WinForm_HandWritingRecognition
{
    /// <summary>
    /// 手写识别
    /// </summary>
    public partial class Form1 : Form
    {
        InkCollector ic;
        RecognizerContext rct;
        // Recognizer rc;
        string FullCACText;

        /// <summary>
        ///
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //this.rct.RecognitionWithAlternates += new RecognizerContextRecognitionWithAlternatesEventHandler(rct_RecognitionWithAlternates);

            ic = new InkCollector(PictureboxInk.Handle);
            this.ic.Stroke += new InkCollectorStrokeEventHandler(ic_Stroke);

            ic.Enabled = true;
            ink_();

            //   this.ic.Stroke += new InkCollectorStrokeEventHandler(ic_Stroke);
            this.rct.RecognitionWithAlternates += new RecognizerContextRecognitionWithAlternatesEventHandler(rct_RecognitionWithAlternates);

            rct.Strokes = ic.Ink.Strokes;

        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rct_RecognitionWithAlternates(object sender, RecognizerContextRecognitionWithAlternatesEventArgs e)
        {

            string ResultString = "";
            RecognitionAlternates alts;

            if (e.RecognitionStatus == RecognitionStatus.NoError)
            {
                alts = e.Result.GetAlternatesFromSelection();

                foreach (RecognitionAlternate alt in alts)
                {
                    ResultString = ResultString + alt.ToString() + " ";
                }
            }
            FullCACText = ResultString.Trim();
            Control.CheckForIllegalCrossThreadCalls = false;
            textBox1.Text = FullCACText;
            returnString(FullCACText);
            Control.CheckForIllegalCrossThreadCalls = true;

        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        private void returnString(string str)
        {
            string[] str_temp = str.Split(' ');
            string str_temp1 = "shibie_";
            string str_temp2 = "";
            if (str_temp.Length > 0)
            {
                for (int i = 0; i < str_temp.Length; i++)
                {
                    str_temp2 = str_temp1 + i.ToString();
                    Control[] con_temp = Controls.Find(str_temp2, true);
                    if (con_temp.Length > 0)
                    {
                        ((Button)(con_temp[0])).Text = str_temp[i];
                    }
                }
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ic_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            rct.StopBackgroundRecognition();
            rct.Strokes.Add(e.Stroke);
            rct.CharacterAutoCompletion = RecognizerCharacterAutoCompletionMode.Full;
            rct.BackgroundRecognizeWithAlternates(0);
        }
        /// <summary>
        ///
        /// </summary>
        private void ink_()
        {
            Recognizers recos = new Recognizers();
            Recognizer chineseReco = recos.GetDefaultRecognizer();

            rct = chineseReco.CreateRecognizerContext();
        }
        /// <summary>
        ///
        /// </summary>
        private void ic_Stroke()
        {

        }
        /// <summary>
        /// 获取字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";

            textBox1.SelectedText = ic.Ink.Strokes.ToString();
            /*  if (0 == rc.Count)
              {
                  MessageBox.Show("There are no handwriting recognizers installed.  You need to have at least one in order to recognize ink.");
              }
              else
              {

                  // Note that the Strokes' ToString() method is a
                  // shortcut  for retrieving the best match using the 
                  // default recognizer.  The same result can also be
                  // obtained using the RecognizerContext.  For an
                  // example, uncomment the following lines of code:
                  //
                   RecognizerContext myRecoContext = new RecognizerContext();
                   myRecoContext.CharacterAutoCompletion = RecognizerCharacterAutoCompletionMode.Full;
                   RecognitionStatus status;
                  RecognitionResult recoResult;
                  //
                   myRecoContext.Strokes = ic.Ink.Strokes;
                  recoResult = myRecoContext.Recognize(out status);
                  textBox1.SelectedText = recoResult.TopString;
                  //

                 // textBox1.SelectedText = ic.Ink.Strokes.ToString();
              }*/
        }
        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (!ic.CollectingInk)
            {
                Strokes strokesToDelete = ic.Ink.Strokes;
                rct.StopBackgroundRecognition();
                ic.Ink.DeleteStrokes(strokesToDelete);
                rct.Strokes = ic.Ink.Strokes;
                ic.Ink.DeleteStrokes();//清除手写区域笔画;
                PictureboxInk.Refresh();//刷新手写区域
            }

        }
        /// <summary>
        /// 选择颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            colorDialog1.FullOpen = true;
            colorDialog1.ShowDialog();
            ic.DefaultDrawingAttributes.Color = colorDialog1.Color;

        }

    }
}


