using DotNet.Utilities;
using SufeiUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hilton
{
    public class HiltonLogin2
    {
        private string userAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchUrl, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);
        private static string GetCookieString(string url)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString();
        }
        public List<CookieItem> getWebCookie()
        {
            //string cookie1= "visitorId=3f9ef7d3-0b6d-4ab7-84d1-ae3f7eefeef2; ak_bmsc=293EDDAE0BD39CEC3D67572C622D65C16860A9577B6C0000EA65E459908BA304~pl926rdMTQxvzIcKvpnqCPLvZ/qvVdMCTaJtx6ppAcYgMMCKL/lv1yXv3sXC7j6SsIyl6V9vpy1gzCZ+0tei4WoerO4VIK4sTH5uFG1I7suMCqXgqGRQrLe3KDg5utVQWF6T+ExabvcGzfccWYGEf95YtM0Y3DUdWvxy6g3NLSANaQElTjEs6svRXUIT0pj+ZjqKTwB2XKuol1VsBX8yLnpcCWtq71+q+4OVFlCNl3DUkYDzKb5OGit2YdrnY3iCuD; _abck=E38A11146496EA9A9D4A35924E160E096860A9577B6C0000EA65E459EB03494D~-1~arHuZsLuQRlvRd2WIjI+gb/fPN5KkFL6rRPWbtAbgSk=~0~-1; bm_sz=9ADF3ED2AA6D33122EDE9535338C934C~QAAQV6lgaEduJSRfAQAAohsuJI9kjK/ZbcWbXLWIaQ8BIaRzq7P5/rACj6hnnrGUUjGQx++5hjMcdaPhatBhJ93dliJ1XBC8dYj/Db8KTeU83K5q5SqVE0fuN8m39/DlnsuCPOlJ6Oay+/eslewj14/bFybbNjJVtwYcR+Wr7hyHobaxl1o5wnl05MDC49E=; mmcore.tst=0.483; AMCV_F0C120B3534685700A490D45%40AdobeOrg=2121618341%7CMCIDTS%7C17456%7CMCMID%7C40926487365891987052735797300685766011%7CMCAAMLH-1508745331%7C11%7CMCAAMB-1508745331%7CNRX38WO0n5BH8Th-nqAG_A%7CMCOPTOUT-1508147731s%7CNONE%7CMCAID%7CNONE; bm_sv=84B5C14FC9DA61F368CABEF24E55D658~3MAZsiy/aAXu7aiRAU+mrOt+zljR8ylMSMPs3rBaPV4eGYcv6s0hNN1JXSk47bB55/AOlhTP0SjLaJ85+7tFHNtFDotPXI9po0Go7/pYNgVA7QLX4RX/3yCr3HxP/VKZ/YYteMJd3QthU4cT2pTH2f1OPNPMPi+788/2T29mCF4=; s_dfa=hiltonglobalprod; RT=\"sl = 12 & ss = 1508140530936 & tt = 0 & obo = 12 & bcn =% 2F % 2F36fb607e.akstat.io % 2F & sh = 1508140575435 % 3D12 % 3A12 % 3A0 % 2C1508140566944 % 3D11 % 3A11 % 3A0 % 2C1508140563518 % 3D10 % 3A10 % 3A0 % 2C1508140562888 % 3D9 % 3A9 % 3A0 % 2C1508140557249 % 3D8 % 3A8 % 3A0 & dm = hilton.com & si = 4ea3a548 - 43f5 - 4f2f - a34c - 0a5d7474021a & se = 900 & ld = 1508140575435 & r = https % 3A % 2F % 2Fsecure3.hilton.com % 2Fen % 2Fhh % 2Fcustomer % 2Flogin % 2Findex.htm & ul = 1508140575440\"; sn.ds=d||a; sn.as=tp||1012~ir||s~ctry||login~a||t~vg||0~ti||t1~co||1; xyz_cr_666_et_100; sn.vi=vi||909a90dc-79fe-4df5-88f6-d6e1cbc980de~cslt||1508140573501~as||1~st||1508140573527~cat||1508140573501~c||1; sn.rn=rv||59.83~rt||23.398~re||94.449; sn.wt=u||id; _sdsat_session_count=2; _sdsat_lt_pages_viewed=16; ClrSSID=1508140525256-7474; ClrOSSID=1508140525256-7474; ClrSCD=1508140525256";
            string cookie = GetCookieString("https://secure3.hilton.com/en/hh/customer/login/index.htm");
            Console.WriteLine(cookie);
            if (cookie == null)
                return new List<CookieItem>();
            return HttpCookieHelper.GetCookieList(cookie);
        }

        private List<CookieItem> getHttpCookie(string cookie)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                Host = "secure3.hilton.com",
                URL = "https://secure3.hilton.com/en/hh/customer/login/index.htm",//URL     必需项    
                Method = "get",//URL     可选项 默认为Get
                ResultType = ResultType.String,
                Cookie = cookie,
                UserAgent = userAgent,
                Accept = "*/*",
                Timeout = 20000,        //20秒
            };
            item.Header.Add("Accept-Language: zh-cn");
            item.Header.Add("Accept-Encoding: gzip, deflate");
            HttpResult result = http.GetHtml(item);
            if (result.Cookie == null)
                return null;
            
            List<CookieItem> cookieList = HttpCookieHelper.GetCookieList(result.Cookie);

            List<CookieItem> cookieResult = new List<CookieItem>();
            foreach (CookieItem coo in cookieList)
            {
                if (coo.Key.ToLower().Contains("domain") || coo.Key.ToLower().Contains("expires") || coo.Key.ToLower().Contains("path") || coo.Key.ToLower().Contains("max-age"))
                    continue;
                cookieResult.Add(coo);
            }
            return cookieResult;
        }
        
        public string getString(List<CookieItem> toCookieItem) {
            string result = "";
            foreach (CookieItem item in toCookieItem) {
                result += item.Key + "=" + item.Value + ";";
            }
            return result;
        }

        private List<CookieItem> getSensorTest(string cookie,string key)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                Host = "secure3.hilton.com",
                URL = "https://secure3.hilton.com/_bm/_data",//URL     必需项    
                Method = "post",//URL     可选项 默认为Get
                Postdata = key,
                UserAgent = userAgent,
                Accept = "*/*",
                ContentType = "application/json",
                Cookie = cookie,
                Referer = "https://secure3.hilton.com/en/hh/customer/login/index.htm",
                Timeout = 20000,        //20秒
                ResultType = ResultType.String,
            };
            item.Header.Add("Accept-Language: zh-cn");
            item.Header.Add("Accept-Encoding: gzip, deflate");
            item.Header.Add("Cache-Control: no-cache");

            HttpResult result = http.GetHtml(item);
           // Console.Write(result.RedirectUrl);
            if (result.Cookie == null)
                return null;
            
            List<CookieItem> cookieList = HttpCookieHelper.GetCookieList(result.Cookie);

            List<CookieItem> cookieResult = new List<CookieItem>();
            foreach (CookieItem coo in cookieList)
            {
                if (coo.Key.ToLower().Contains("domain") || coo.Key.ToLower().Contains("expires") || coo.Key.ToLower().Contains("path") || coo.Key.ToLower().Contains("max-age"))
                    continue;
                cookieResult.Add(coo);
            }
            return cookieResult;
        }

        List<CookieItem> cookie_final = new List<CookieItem>();
        private void subCookie(List<CookieItem> from) {
            foreach (CookieItem from_item in from) {
                bool find = false;
                foreach (CookieItem final_item in cookie_final)
                {
                    if (from_item.Key.Equals(final_item.Key))
                    {
                        find = true;
                    }
                }
                if (!find)
                {
                    cookie_final.Add(from_item);
                }
            }
        }

        public string getPointsTest(string username, string password,string key)
        {
            /*
            List<CookieItem> cookie_http2 = getWebCookie();
            if (cookie_http2 != null)
            {
                subCookie(cookie_http2);
            }*/
            
            List<CookieItem> cookie_http = getHttpCookie("");
            if (cookie_http != null) {
                subCookie(cookie_http);
            }
            
            List<CookieItem> cookie_sensor = getSensorTest(getString(cookie_final),key);
            if (cookie_sensor == null)
                return "timeout";
            subCookie(cookie_sensor);

            string post = String.Format("username={0}&password={1}", username, password);
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                Host = "secure3.hilton.com",
                URL = "https://secure3.hilton.com/en/hh/customer/login/index.htm",//URL     必需项    
                Method = "post",//URL     可选项 默认为Get
                Postdata = post,
                UserAgent = userAgent,
                Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, */*",
                ContentType = "application/x-www-form-urlencoded",
                Cookie = getString(cookie_final),
                Referer = "https://secure3.hilton.com/en/hh/customer/login/index.htm",
                Timeout = 20000,        //20秒
                ResultType = ResultType.String,
                Allowautoredirect = true,
            };
            item.Header.Add("Accept-Language: zh-CN");
            item.Header.Add("Accept-Encoding: gzip, deflate");

            HttpResult result = http.GetHtml(item);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (result.ResponseUri.Contains("https://secure3.hilton.com/en/hh/customer/account/index.htm"))
                {
                    string find = "<dt>Points:</dt>";
                    int index = result.Html.IndexOf(find);
                    int begin = result.Html.IndexOf("<dd>", index);
                    int end = result.Html.IndexOf("</dd>", index);
                    string point = result.Html.Substring(begin + 4, end - begin - 4);
                    return point;
                }
                else
                {
                    if(result.Cookie == null)
                        return "timeout";

                    Console.WriteLine(result.Header);
                    return "账户错误";
                }

            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return result.StatusDescription;
            }
            else
            {
                return "timeout";
            }
        }
    }
}
