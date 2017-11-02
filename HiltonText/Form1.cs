using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HiltonText
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                string resultPaht = openFileDialog1.FileName.Replace(".txt", "--过滤.txt");
                string filePaht = openFileDialog1.FileName;

                StreamReader f1 = new StreamReader(openFileDialog1.FileName, System.Text.Encoding.GetEncoding("gb2312"));
                StreamWriter f2 = new StreamWriter(resultPaht, false);

                string strReadline;
                while ((strReadline = f1.ReadLine()) != null)
                {
                    string[] split = strReadline.Split(new string[] { "----" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if(split.Length == 2)
                    { 
                        string Username = split[0];
                        string Password = split[1];
                        if(Regex.IsMatch(Password, "[A-Z]") && Password.Length>7)
                            f2.WriteLine(strReadline);
                    }

                }
                f1.Close();
                f2.Close();

                MessageBox.Show("转换完成！");
            }
        }
    }
}
