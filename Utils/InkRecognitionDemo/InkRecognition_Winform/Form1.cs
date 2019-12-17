using Microsoft.Ink;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InkRecognition2
{
    public partial class Form1 : Form
    {
        private InkCollector myInkCollector = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myInkCollector = new InkCollector(pictureBox1.Handle);
            myInkCollector.Enabled = true;

            recoContext = new RecognizerContext();
            strokesToRecognize = myInkCollector.Ink.CreateStrokes();
            recoContext.Strokes = strokesToRecognize;
            recoContext.RecognitionWithAlternates += new RecognizerContextRecognitionWithAlternatesEventHandler(reco_RWA);
            myInkCollector.Stroke += InkOverlay_Stroke;
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!myInkCollector.CollectingInk)
            {
                Strokes strokesToDelete = myInkCollector.Ink.Strokes;
                myInkCollector.Ink.DeleteStrokes(strokesToDelete);
                myInkCollector.Ink.DeleteStrokes();//清除手写区域笔画;
                pictureBox1.Refresh();//刷新手写区域
                strokesToRecognize.Clear();
            }

            textBox1.Text = "";
            listBox1.Items.Clear();
        }

        /// <summary>
        /// 显示坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            //string text = "";
            // Query the Ink object to get the count of Stokes      
            // and iterate through.     
            foreach (Stroke s in myInkCollector.Ink.Strokes)
            {
                // Now for each Stoke get the count of points        
                // and iterate through.        
                Point[] points = s.GetPoints();
                foreach (Point point in points)
                {
                    // Place the point values into the text box.           
                    //text += point.X + "," + point.Y + "；";
                    listBox1.Items.Add(point.X + "," + point.Y);
                }
            }
            //textBox1.Text = text;
        }



        private RecognizerContext recoContext;
        private Strokes strokesToRecognize;
        RecognitionResult recognitionResult;
        void InkOverlay_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            strokesToRecognize.Add(e.Stroke);
            recoContext.BackgroundRecognizeWithAlternates();
        }

        void reco_RWA(object sender, RecognizerContextRecognitionWithAlternatesEventArgs e)
        {
            if (textBox1.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action<string> actionDelegate = (x) => { this.textBox1.Text = x.ToString(); };
                // 或者
                // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                this.textBox1.Invoke(actionDelegate, e.Result.TopString);
            }
            else
            {
                textBox1.Text = e.Result.TopString;
            }

            textBox1.Text = e.Result.TopString;
            // The handler receives a subclassed EventArgs object that     
            // exposes the RecognitionResult object through a Result property.     
            // The RecognitionResult object has four properties:      
            // Strokes, TopAlternate, TopConfidence, and TopString.      
            // The TopString property merely returns the ToString() value     
            // of the TopAlternate object.  Finally, assign to the previously     
            // declared class-level variable the RecognitionResult object    
            // passed to the recognition event handler.

            recognitionResult = e.Result;
        }


        // Listbox code to place all the possible results into the list   
        private void textBox1_MouseUp(object sender, MouseEventArgs e)
        {
            listBox1.Items.Clear();
            if (textBox1.SelectionLength > 0)
            {
                RecognitionAlternates alternates = recognitionResult.GetAlternatesFromSelection(textBox1.SelectionStart, textBox1.SelectionLength);
                foreach (RecognitionAlternate alternate in alternates)
                {
                    listBox1.Items.Add(alternate);
                }
                //inkOverlay.Selection = alternates.Strokes;
            }
        }


    }
}
