using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    enum SortType
    {
        lettersCount,
        lineLength,
        wordsCount,
        signsCount

    }
    enum SortMethod
    {
        Bubble,
        Selection,
        Insertion,
        Shell
    }

    public partial class Form1 : Form
    {
        string[] strs = { "fdgdfgfdgdgdfggfdgdgfdgwgg", "abc", "bbbb", "gghfh", "gfdgdfgdsgsdfgdgs" };
        SortType st;
        SortMethod sm;
        private delegate void SetPropertyThreadSafeDelegate<TResult>(
        Control @this,
        Expression<Func<TResult>> property,
        TResult value); 
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Result_Click(object sender, EventArgs e)
        {

        }

        private void SortType_CheckedChanged(object sender, EventArgs e)
        {
            st = (SortType)Enum.Parse(typeof(SortType), (sender as RadioButton).Name);

        }

        private void SortMethod_CheckedChanged(object sender, EventArgs e)
        {

        }

        private async void start_Click(object sender, EventArgs e)
        {
            int[] sortedIndexs = strs.CountByLength();
            Series series = new Series("Length");
            String str;

            for (int index = strs.Length - 1, index2 = 0, tmp; index > 0; --index)
            {
                while (index2 < index)
                {
                    if (sortedIndexs[index2] > sortedIndexs[index2 + 1])
                    {
                        tmp = sortedIndexs[index2];
                        sortedIndexs[index2] = sortedIndexs[index2 + 1];
                        sortedIndexs[index2 + 1] = tmp;

                        str = strs[index2];
                        strs[index2] = strs[index2 + 1];
                        strs[index2 + 1] = str;
                        series = new Series("Length");
                        for (int index3 = 0, length = strs.Length; index3 < length; ++index3)
                        {
                            series.Points.AddXY(index3, sortedIndexs[index3]);
                        }

                        this.Result.Series.Clear();
                        this.bubble.Invoke((MethodInvoker)delegate {
                            // Running on the UI thread
                            this.Result.Series.Add(series);
                        });
                        Thread.Sleep(500);
                    }
                    ++index2;
                }
                index2 = 0;
            }
        }
        public void BubbleSort(Series series)
        {

        }

    }
}