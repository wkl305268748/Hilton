using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SufeiUtil;
using System.Runtime.InteropServices;
using DotNet.Utilities;
using System.IO;
using System.Threading;

namespace Hilton
{
    public partial class Form1 : Form
    {
        private const int STRING_LOG = 0;
        private const int UPDATE = 1;

        string resultPaht;
        string errorPaht;
        string filePaht;

        List<string> mAccount = new List<string>();
        List<HiltonModel> hiltonModel = new List<HiltonModel>();

        HiltonBM hiltonBM = new HiltonBM();

        int count_time = 0;
        int count_forbidden = 0;
        int count_error = 0;
        int count_success = 0;

        //声明一个delegate（委托）类型
        public delegate void testDelegate(int type,object str);
        //声明一个testDelegate类型的对象
        public testDelegate mainThread;
        int hiltonIndex = 0;

        private void log(int type, object str)
        {
            switch (type) {
                case STRING_LOG:
                    textBox1.AppendText(str.ToString());
                    if (textBox1.Text.Length > 2000)
                        textBox1.Text = "";
                    break;
                case UPDATE:
                    HiltonModel model = (HiltonModel)str;
                    this.listView1.Items[model.Index].SubItems[2].Text = model.Status;
                    //写文件

                    StreamWriter writer = new StreamWriter(resultPaht,true);
                    StreamWriter writer_error = new StreamWriter(errorPaht, true);
                    if (!model.Status.Equals("账户错误"))
                        writer.WriteLine(model.Username + "----" + model.Password + "----" + model.Status);
                    else
                    {
                        writer_error.WriteLine(model.Username + "----" + model.Password + "----" + model.Status);

                    }
                    writer.Close();
                    writer_error.Close();

                    break;
            }

            textBoxCount.Text = "";
            textBoxCount.AppendText("超时次数:      " + count_time + "\r\n\r\n");
            textBoxCount.AppendText("服务器拒绝次数:" + count_forbidden + "\r\n\r\n");
            textBoxCount.AppendText("账户错误:      " + count_error + "\r\n\r\n");
            textBoxCount.AppendText("成功:          " + count_success + "\r\n");
        }
        public Form1()
        {
            InitializeComponent();
            mainThread = new testDelegate(log);
        }

        private void 打开账号文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                hiltonModel = new List<HiltonModel>();
                resultPaht = openFileDialog1.FileName.Replace(".txt", "result.txt");
                errorPaht = openFileDialog1.FileName.Replace(".txt", "error.txt");
                
                filePaht = openFileDialog1.FileName;

                StreamReader f2 = new StreamReader(openFileDialog1.FileName, System.Text.Encoding.GetEncoding("gb2312"));
                string strReadline;
                int index = 0;
                while ((strReadline = f2.ReadLine()) != null)
                {
                    string[] split = strReadline.Split(new string[] { "----" }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length==2)
                    { 
                        HiltonModel hilto = new HiltonModel();
                        hilto.Username = split[0];
                        hilto.Password = split[1];
                        hilto.Status = "待检测";
                        hilto.Index = index;
                        hiltonModel.Add(hilto);
                        index++;
                    }
                }
                f2.Close();
                initView();
                textBox1.AppendText("导入成功！共计" + hiltonModel.Count + "个账户\r\n");
                hiltonIndex = 0;
            }
        }

        private void initView() {


            this.listView1.BeginUpdate();
            listView1.Items.Clear();
            for (int i = 0; i < hiltonModel.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = hiltonModel[i].Username;
                lvi.SubItems.Add(hiltonModel[i].Password);
                lvi.SubItems.Add(hiltonModel[i].Status);
                this.listView1.Items.Add(lvi);
            }
            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。  

            StreamWriter writer = new StreamWriter(resultPaht, false);
            writer.Write("");
            writer.Close();


            StreamWriter writer_error = new StreamWriter(errorPaht, false);
            writer_error.Write("");
            writer_error.Close();
        }
        
        bool isComplated;
        bool isThreadAsdl;
        int asdlCount = 0;
        int forbidden_count = 0;
        int forbidden_count_time = 0;


        private HiltonModel getNextHiltonModel() {
            HiltonModel model = hiltonModel[hiltonIndex];
            hiltonIndex++;
            return model;
        }
        private void myThread() {
            
            HiltonLogin2 hilton = new HiltonLogin2();
            int keyId = hiltonBM.getNext();
            string key = hiltonBM.getBm();
            HiltonModel model = getNextHiltonModel();

            while (hiltonIndex < hiltonModel.Count && isComplated)
            {
                //拨号
                while (isThreadAsdl) {
                    Thread.Sleep(100);
                }

                asdlCount++;
                if (asdlCount % countADSL == 0 && isADSL)
                {
                    asdlCount++;
                    isThreadAsdl = true;

                    string outStr = null;
                    CmdHelper.RunCmd("rasdial.exe /disconnect", out outStr);
                    this.Invoke(mainThread, STRING_LOG, Thread.CurrentThread.Name + " --- " + "断开连接" + "\r\n");
                    CmdHelper.RunCmd("rasdial.exe " + adsl_str, out outStr);
                    this.Invoke(mainThread, STRING_LOG, Thread.CurrentThread.Name + " --- " + "开始连接" + "\r\n");

                    isThreadAsdl = false;
                } 

                string re = hilton.getPointsTest(model.Username, model.Password, key);
                model.Status = re;
                Console.WriteLine(re);

                if (re.Equals("timeout"))
                {
                    this.Invoke(mainThread, STRING_LOG, Thread.CurrentThread.Name + " --- " + "出现错误: Timeout超时 重新发送请求\r\n");
                    count_time++;
                    forbidden_count_time++;
                    if (forbidden_count_time > 4)
                    {
                        keyId = hiltonBM.getNext();
                        key = hiltonBM.getBm();
                        this.Invoke(mainThread, STRING_LOG, Thread.CurrentThread.Name + " --- " + "下一个KEY：" + keyId + "\r\n");
                        forbidden_count_time = 0;
                    }
                }
                else if (re.Equals("Forbidden"))
                {
                    this.Invoke(mainThread, STRING_LOG, Thread.CurrentThread.Name + " --- " + "出现错误: Forbidden 重新发送请求\r\n");
                    count_forbidden++;
                    forbidden_count++;
                    if (forbidden_count > 4)
                    {
                        keyId = hiltonBM.getNext();
                        key = hiltonBM.getBm();
                        this.Invoke(mainThread, STRING_LOG, Thread.CurrentThread.Name + " --- " + "下一个KEY：" + keyId + "\r\n");
                        forbidden_count = 0;
                    }
                }
                else
                {
                    if (re.Equals("账户错误"))
                        count_error++;
                    else
                        count_success++;

                    this.Invoke(mainThread, UPDATE, model);
                    this.Invoke(mainThread, STRING_LOG, Thread.CurrentThread.Name + " --- " + "执行第" + (model.Index + 1) + "个账户  " + DateTime.Now.ToShortTimeString() + "\r\n");
                    model = getNextHiltonModel();
                }
            }
            if (!isComplated)
                this.Invoke(mainThread, STRING_LOG, Thread.CurrentThread.Name + " --- " + "停止运行  " + DateTime.Now.ToShortTimeString() + "\r\n");
            else
                this.Invoke(mainThread, STRING_LOG, Thread.CurrentThread.Name + " --- " + "执行完成  " + DateTime.Now.ToShortTimeString() + "\r\n");
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        
        private void btnBegin_Click(object sender, EventArgs e)
        {

            isComplated = true;
            textBox1.AppendText("开始执行操作  " + DateTime.Now.ToShortTimeString() + "\r\n");
            textBox1.AppendText("线程数量：  " + textThread.Text + "\r\n");
            btnBegin.Enabled = false;
            btnStop.Enabled = true;
            asdlCount = 0;
            isThreadAsdl = false;


            int threadNumber = int.Parse(textThread.Text);
            for (int i = 0; i < threadNumber; i++)
            {
                Thread thread = new Thread(myThread);     //执行的必须是无返回值的方法
                thread.Name = "线程" + (i + 1);
                thread.Start();
                thread.IsBackground = false;
            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isComplated = false;
            btnBegin.Enabled = true;
            btnStop.Enabled = false;
        }

        int countADSL = 10;
        bool isADSL = false;
        string adsl_str = "";
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isADSL = checkBox1.Checked;
            textADSL.Enabled = !isADSL;
            if (isADSL)
            {
                countADSL = int.Parse(textADSL.Text);
                adsl_str = tb_adsl_name.Text + " " + tb_adsl_user.Text + " " + tb_adsl_pass.Text;
            }
        }

        private void webToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new WebView().Show();
        }
    }
}
