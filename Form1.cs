using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.Native;

namespace Wallpaper_Power
{
    public partial class Form1 : Form
    {
        bool downloaded = false;
        string url = "";
        string er1 = "https://api.ixiaowai.cn/api/api.php";
        string er2 = "http://api.btstu.cn/sjbz/?lx=dongman";
        string er3 = "https://api.dujin.org/pic/";
        string er4 = "https://api.dongmanxingkong.com/suijitupian/acg/1080p/index.php";
        string er5 = "http://www.dmoe.cc/random.php";
        string er6 = "https://api-misc.iclart.com/v1/acgapi";
        string er7 = "https://api.isoyu.com/";
        string er8 = "https://api.btstu.cn/doc/sjbz.php";
        string er9 = "http://api.mtyqx.cn/";
        string er10 = "https://api.yuzhitu.cn/";
        string er11 = "https://api.yuzhitu.cn/sjbz/";
        string fengjing = "https://api.ixiaowai.cn/gqapi/gqapi.php";
        string meizi = "http://api.btstu.cn/sjbz/?lx=meizi";
        string selected = "";
        bool auto = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["autochange"] == "True")
            {
                timer1.Enabled = true;
                try
                {
                    timer1.Interval = Convert.ToInt16(ConfigurationManager.AppSettings["delay"]) * 1000;
                    textBox1.Text = ConfigurationManager.AppSettings["delay"];
                }
                catch
                {
                    MessageBox.Show(this, "输入的时间间隔不合法！必须大于0！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            comboBox1.SelectedItem = ConfigurationManager.AppSettings["paper"];
            if (ConfigurationManager.AppSettings["autorun"] == "True")
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }
            if (ConfigurationManager.AppSettings["autochange"] == "True")
            {
                checkBox1.Checked = true;
            }
            else checkBox1.Checked = false;
            if (checkBox2.Checked == true)
            {
                bool isexc = false;
                RegistryKey RKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                //设置自启的程序叫获取目录下的程序名字
                string[] ar = RKey.GetValueNames();
                foreach (string st in ar)
                {
                    if (st.Equals("WPP"))
                    {
                        isexc = true;
                    }
                }
                if (!isexc)
                {
                    //设置自启的程序叫test
                    RKey.SetValue("WPP", Application.StartupPath + @"\Wallpaper Power.exe");
                }
                RKey.Close();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                try
                {
                    rk2.DeleteValue("WallPaper Power");
                }
                catch { }
                rk2.Close();
                rk.Close();
            }
            if (checkBox2.Checked == true)
            {
                UpdateAppSettings("autorun", "True");
            }
            else UpdateAppSettings("autorun", "False");
        }
        public static bool UpdateAppSettings(string key, string value)
        {
            var _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (!_config.HasFile)
            {
                throw new ArgumentException("程序配置文件缺失！");
            }
            KeyValueConfigurationElement _key = _config.AppSettings.Settings[key];
            if (_key == null)
                _config.AppSettings.Settings.Add(key, value);
            else
                _config.AppSettings.Settings[key].Value = value;
            _config.Save(ConfigurationSaveMode.Modified);
            return true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                auto = false;
            }
            else auto = true;
            if (checkBox1.Checked == true)
            {
                UpdateAppSettings("autochange", "True");
            }
            else UpdateAppSettings("autochange", "False");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAppSettings("paper", comboBox1.SelectedItem.ToString());
            if (comboBox1.SelectedItem.ToString() == "360动漫壁纸")
            {
                checkBox3.Enabled = true;

            }
            else checkBox3.Enabled = false;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            change();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateAppSettings("delay", textBox1.Text);
            timer1.Enabled = true;
            try
            {
                timer1.Interval = Convert.ToInt16(textBox1.Text) * 1000;
                if (comboBox1.SelectedItem.ToString() == "360动漫壁纸") {
                    timer2.Interval = Convert.ToInt16(textBox1.Text) * 1000 / 2;
                    timer2.Enabled = true;
                }
            }
            catch
            {
                MessageBox.Show(this, "输入的时间间隔不合法！必须大于0！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem.ToString() == "360动漫壁纸") {
                button2.Enabled = false;
                return; }
            change();
        }
        void down() {
            if (downloaded == true) return;
            string url = "http://wallpaper.apc.360.cn/index.php?c=WallPaperAndroid&a=getAppsByCategory&cid=26&start=0&count=99";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            request.AllowAutoRedirect = false;
            HttpWebResponse myResp = (HttpWebResponse)request.GetResponse();
            if (myResp.StatusCode == HttpStatusCode.Redirect)
            { url = myResp.GetResponseHeader("Location"); }
            WebClient webclient = new WebClient();
            string json = webclient.DownloadString(url);
            JObject jo = (JObject)JsonConvert.DeserializeObject(json);
            JArray ja = JArray.Parse(jo["data"].ToString());
            string[] urls = new string[99];
            for (int count = 0; count <= ja.Count - 1; count++)
            {
                JObject jb = JObject.Parse(ja[count].ToString());
                urls[count] = jb["url"].ToString();
            }
            for (int b = 0; b < 10; b++)
            {
                for (int count = 0; count < 98; count++)
                {
                    WebClient wb = new WebClient();
                    Random rand = new Random();
                    int a = rand.Next();
                    Uri uri = new Uri(urls[count]);
                    wb.DownloadFileAsync(uri, Application.StartupPath + @"\img\360\" + a.ToString() + ".jpg");
                }
            }
            downloaded = true;
        }
        void change()
        {
            if (comboBox1.SelectedItem.ToString() == "360动漫壁纸")
            {
                ArrayList pics = new ArrayList();
                DirectoryInfo TheFolder = new DirectoryInfo(Application.StartupPath+@"\img\360");
                int a = 0;
                foreach (FileInfo NextFile in TheFolder.GetFiles()) {
                    pics.Add(NextFile.Name);
                }
                Random rand = new Random();
                int random = rand.Next(0, pics.Count);
                var ads = shlobj.GetActiveDesktop();
                ads.SetWallpaper(Application.StartupPath+@"\img\360\"+pics[random], 0);
                string temp = Application.StartupPath + @"\img\360\" + pics[random];
                ads.ApplyChanges(AD_Apply.ALL | AD_Apply.FORCE | AD_Apply.BUFFERED_REFRESH);
                Marshal.ReleaseComObject(ads);
                return;
            }
            switch (comboBox1.SelectedItem.ToString())
            {
                case "二次元源1":
                    selected = er1;
                    break;
                case "二次元源2":
                    selected = er2;
                    break;
                case "二次元源3":
                    selected = er3;
                    break;
                case "二次元源4":
                    selected = er4;
                    break;
                case "二次元源5":
                    selected = er5;
                    break;
                case "二次元源6":
                    selected = er6;
                    break;
                case "二次元源7":
                    selected = er7;
                    break;
                case "二次元源8":
                    selected = er8;
                    break;
                case "二次元源9":
                    selected = er9;
                    break;
                case "二次元源10":
                    selected = er10;
                    break;
                case "二次元源11":
                    selected = er11;
                    break;
                case "二次元色图（慎用！）":
                    MessageBox.Show("你在想屁吃");
                    return;
                    break;
                case "风景":
                    selected = fengjing;
                    break;
                case "妹子":
                    selected = meizi;
                    break;
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(selected);
            req.Method = "HEAD";
            req.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse myResp = (HttpWebResponse)req.GetResponse();
                if (myResp.StatusCode == HttpStatusCode.Redirect)
                { url = myResp.GetResponseHeader("Location"); }
            }
            catch
            {
                MessageBox.Show("使用该源获取壁纸失败，请尝试其他源");
            }

            WebClient web = new WebClient();
            Random rd = new Random();
            int i = rd.Next();
            string pathp = Application.StartupPath + @"\img\" + i.ToString() + ".jpg";
            try
            {
                web.DownloadFile(new Uri(url), pathp);
            }
            catch
            {
                MessageBox.Show("使用该源获取壁纸失败，请尝试其他源");
            }
            var ad = shlobj.GetActiveDesktop();
            ad.SetWallpaper(pathp, 0);
            ad.ApplyChanges(AD_Apply.ALL | AD_Apply.FORCE | AD_Apply.BUFFERED_REFRESH);
            Marshal.ReleaseComObject(ad);
        }

        private void Form_SizeChanged(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.Show();
            this.notifyIcon1.Visible = false;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {     //可以要，也可以不要，取决于是否隐藏主窗体
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "由于服务器限制，自动预载图片仅能在壁纸源选择360后使用\n注意：使用360壁纸动漫后可能会出现小猪佩奇等其他奇怪东西！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            down();
        }
    }
    public class Data
    {
        public string pid { get; set; }
        public string cid { get; set; }
        public string dl_cnt { get; set; }
        public string c_t { get; set; }
        public string imgcut { get; set; }
        public string url { get; set; }
        public string tempdata { get; set; }
        public string fav_total { get; set; }
    }

    public class RootObject
    {
        public string errno { get; set; }
        public string errmsg { get; set; }
        public string consume { get; set; }
        public string total { get; set; }
        public List<Data> data { get; set; }
    }
}
