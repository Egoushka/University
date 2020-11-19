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
        string[] strs = { "hghgh", "hgdhgfh", "gfgdfdgd", "hghghgfhfhhwhrthrtether", "hgghgfh" };
        SortType st;
        SortMethod sm;
        private Thread progressThread = null;
        private string fname = null;
        public Form1()
        {
            InitializeComponent();
            ViewController.SetForm(this);

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
            sm = (SortMethod)Enum.Parse(typeof(SortType), (sender as RadioButton).Name);

        }

        private void start_Click(object sender, EventArgs e)
        {
            //strs = System.IO.File.ReadAllLines(fname);

            progressThread = new Thread(
                Sort
            );
            progressThread?.Start();
        }
        public void Sort()
        {
            int[] sortedArrByIndexes = new int[strs.Length];
            switch (st)
            {
                case SortType.lettersCount:
                    {
                        sortedArrByIndexes = SortingTypes.CountByLettersA(strs);
                        break;
                    }
                case SortType.lineLength:
                    {
                        sortedArrByIndexes = SortingTypes.CountByLength(strs);
                        break;
                    }
                case SortType.wordsCount:
                    {
                        sortedArrByIndexes = SortingTypes.CountByWords(strs);
                        break;
                    }
                case SortType.signsCount:
                    {
                        sortedArrByIndexes = SortingTypes.CountByPunctSings(strs);
                        break;
                    }
                default:
                    break;
            }
            try
            { 
            switch (sm)
            {
                case SortMethod.Bubble:
                    {
                        Sorting.BubbleSort(strs, sortedArrByIndexes);
                        break;
                    }
                case SortMethod.Insertion:
                    {
                        Sorting.InsertSort(strs, sortedArrByIndexes);
                        break;
                    }
                case SortMethod.Selection:
                    {
                        Sorting.SelectionSort(strs, sortedArrByIndexes);

                        break;
                    }
                case SortMethod.Shell:
                    {
                        Sorting.ShellSorting(strs, sortedArrByIndexes);
                        break;
                    }
                default:
                    break; }
                    catch (ThreadAbortException ex)
                    {
                        MessageBox.Show("Goodbye :(");
          
                    }
            }
        }
      
        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "File Dialog";
            fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                fname = fdlg.FileName;
            }
        }
    }
}