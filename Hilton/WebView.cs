using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hilton
{
    public partial class WebView : Form
    {
        int status = 0;
        public WebView()
        {
            InitializeComponent();
        }

        private void WebView_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://secure3.hilton.com/en/hh/customer/login/index.htm");
            status = 0;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().Contains("https://secure3.hilton.com/en/hh/customer/login/index.htm"))
            {
                switch (status)
                {
                    case 0:
                        HtmlElementCollection htmls = webBrowser1.Document.GetElementById("formSignIn").GetElementsByTagName("A");
                        for (int i = 0; i < htmls.Count; i++)
                        {
                            if (htmls[i].GetAttribute("className").Contains("linkBtn"))
                                htmls[i].InvokeMember("click");
                            status = 1;
                        }
                        break;
                    case 1:
                        webBrowser1.Navigate("https://secure3.hilton.com/en/hh/customer/login/index.htm");
                        status = 0;
                        break;
                }
            }
        }
        
    }
}
