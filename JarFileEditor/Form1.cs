using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace JarFileEditor
{
    public partial class Form1 : Form
    {
        JarFile jarFile = new JarFile();
        IndexFile indexFile = new IndexFile();
        Button button1 = new Button();
        Button button2 = new Button();
        Button button3 = new Button();
        Button create_dmi_file_button = new Button();
        BackgroundWorker BackgroundWorker1 = new BackgroundWorker();
        ProgressBar ProgressBar1 = new ProgressBar();
        string jarFileUnZipTempFolder;
        CheckBox[] checkbox1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string d in drags)
            {
                if (!System.IO.File.Exists(d))
                {
                    return;
                }
            }
            e.Effect = DragDropEffects.Move;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //label2.Text = files[0];

            string jarFilePath;
            jarFilePath = files[0];
            jarFile.FilePath = jarFilePath;
            string jarFileExtention;

            //jarファイルの情報を取得
            jarFile.FilePath = jarFilePath;
            jarFile.FileName = System.IO.Path.GetFileName(jarFilePath);
            jarFile.DirectoryName = System.IO.Path.GetDirectoryName(jarFilePath);
            jarFile.BaseName = System.IO.Path.GetFileNameWithoutExtension(jarFilePath);
            jarFileExtention = System.IO.Path.GetExtension(jarFilePath);

            if (jarFileExtention == ".jar")
            {             //Buttonを表示する
                button1 = new Button();
                button1.Name = "Button1";
                button1.Text = "jarファイルを解凍";
                button1.Location = new Point(105, 100);
                button1.Size = new Size(160, 20);
                button1.Click += new EventHandler(Button1_Click);
                this.Controls.Add(button1);
                label1.Text="jarファイルを解凍します。";
            }
            else {
                MessageBox.Show("拡張子が違います｡");
            }
        }

        private void Button1_Click(object sender, EventArgs e) {

            //label2.Text = jarFile.FileName;

            button1.Enabled = false;
            JarUnZip();


        }


        private void JarUnZip() {

            string jarFileDirectoryName = jarFile.DirectoryName;
            jarFileUnZipTempFolder = jarFileDirectoryName + "/unzip";
            string jarFilePath = jarFile.FilePath;
            string jarFileName = jarFile.FileName;

            System.IO.DirectoryInfo di;
            System.IO.FileInfo fi = new System.IO.FileInfo(jarFilePath);

            //tempフォルダを作成
            di = System.IO.Directory.CreateDirectory(jarFileUnZipTempFolder);
            try
            {

                button1.Enabled = false;

                //Processオブジェクトを作成
                System.Diagnostics.Process p = new System.Diagnostics.Process();

                //jarのパスが通っているかをコマンドプロンプトで確認   
                p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = false;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.Arguments = @"/c where jar.exe";
                p.Start();
                string results = p.StandardOutput.ReadToEnd();
                results = results.Replace("\r\n", "");//改行コードを置換
                p.WaitForExit();
                p.Close();

                if (results != "")//パスが取得できていたら
                {
                    //MessageBox.Show(results);//jarのパス
                    string jarExePath = jarFile.JarExePath = results;

                    Process p2 = new Process();
                    p2.StartInfo.FileName = "jar";
                    p2.StartInfo.UseShellExecute = false;
                    p2.StartInfo.RedirectStandardOutput = true;
                    p2.StartInfo.RedirectStandardInput = false;
                    p2.StartInfo.CreateNoWindow = false;
                    p2.StartInfo.WorkingDirectory = jarFileUnZipTempFolder;
                    p2.StartInfo.Arguments = @"xvf " + jarFilePath;
                    p2.Start();
                    string results2 = p2.StandardOutput.ReadToEnd();
                    p2.WaitForExit();
                    p2.Close();
                    MessageBox.Show(results2);//jarを解凍

                    button2 = new Button();//dat読み込み用のボタンを表示
                    button2.Name = "Button1";
                    button2.Text = "osvdatファイルを選択する";
                    button2.Location = new Point(105, 150);
                    button2.Size = new Size(160, 20);
                    button2.Click += new EventHandler(Button2_Click);
                    this.Controls.Add(button2);
                    button2.Enabled = true;
                    label1.Text = "osvのdatファイルを選択してください。";
                }
                else {
                    MessageBox.Show("jar.exeのパスが通っていないです。");

                }

            }
            catch (System.IO.IOException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public static string Right(string str, int len)
        {
            if (len < 0)
            {
                throw new ArgumentException("引数'len'は0以上でなければなりません。");
            }
            if (str == null)
            {
                return "";
            }
            if (str.Length <= len)
            {
                return str;
            }
            return str.Substring(str.Length - len, len);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(jarFileUnZipTempFolder);
            System.IO.FileInfo[] files =
            di.GetFiles("*.dat", System.IO.SearchOption.AllDirectories);
            ArrayList datFile = jarFile.DatFile;
            int checkboxCount = 0;

            foreach (System.IO.FileInfo f in files)
            {
                datFile.Add(f.FullName);
                checkboxCount += 1;
            }

            checkbox1 = new CheckBox[checkboxCount];

            for (int i = 0; i < datFile.Count; i++)
            {
                checkbox1[i] = new CheckBox();
                checkbox1[i].Name = "CheckBox" + i.ToString();
                checkbox1[i].Text = Right((string)jarFile.DatFile[i], 8);
                checkbox1[i].Size = new Size(151, 28);
                checkbox1[i].Location = new Point(100, 170 + (30 * i));
                this.Controls.Add(checkbox1[i]);

            }

            button3 = new Button();//OSVdat読み込み用のボタンを表示
            button3.Name = "Button1";
            button3.Text = "ファイル作成";
            button3.Location = new Point(310, 120);
            button3.Size = new Size(160, 20);
            button3.Click += new EventHandler(Create_DMI_File_Button_Click);
            this.Controls.Add(button3);
            button3.Enabled = true;

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ArrayList osvDatFile = jarFile.OsvDatFile;
            ArrayList fotaDatFile = jarFile.FotaDatFile;
            button3.Enabled = false;
            for (int i = 0; i < checkbox1.Length; i++)
            {

                if (checkbox1[i].Checked == true)
                {
                    osvDatFile.Add(jarFile.DatFile[i]);
                    fotaDatFile.Add("dummy");
                }
                else if (checkbox1[i].Checked == false)
                {
                    fotaDatFile.Add(jarFile.DatFile[i]);
                    osvDatFile.Add("dummy");
                }

                checkbox1[i].Enabled = false;
            }

        }

        private void Create_DMI_File_Button_Click(object sender, EventArgs e)
        {
            ArrayList osvDatFile = jarFile.OsvDatFile;
            ArrayList fotaDatFile = jarFile.FotaDatFile;
            button3.Enabled = false;
            for (int i = 0; i < checkbox1.Length; i++)
            {

                if (checkbox1[i].Checked == true)
                {
                    osvDatFile.Add(jarFile.DatFile[i]);
                    fotaDatFile.Add("dummy");
                }
                else if (checkbox1[i].Checked == false)
                {
                    fotaDatFile.Add(jarFile.DatFile[i]);
                    osvDatFile.Add("dummy");
                }

                checkbox1[i].Enabled = false;
            }



            create_dmi_file_button.Enabled = false;
            string dmiFileFolderName = jarFile.DirectoryName + "/unzip/dmi/updatefile";
            string dmioFileFolderName = jarFile.DirectoryName + "/unzip/dmio/updatefile";
            System.IO.DirectoryInfo di;

            //tempフォルダを作成
            di = System.IO.Directory.CreateDirectory(dmiFileFolderName);
            di = System.IO.Directory.CreateDirectory(dmioFileFolderName);

            try {
                MessageBox.Show("y");
                createJarFile();



            }
            catch (System.IO.IOException ex) {

                MessageBox.Show(ex.Message);
                //test

            }

        }

        public void createJarFile() {

            DirectoryInfo di = new DirectoryInfo(jarFileUnZipTempFolder);
            FileInfo[] files =
            di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
            string indexfilePath = files[0].FullName;


            StreamReader sr = new StreamReader(indexfilePath, Encoding.GetEncoding("Shift_JIS"));
            indexFile.IndexFileAllText = sr.ReadToEnd();
            sr.Close();

            string indexFileAllText = indexFile.IndexFileAllText;

            Regex r_versionInfo = new Regex(@"\[VERSIONINFO\].*?\[END-VERSIONINFO\]", RegexOptions.Singleline);
            Match m_versionInfo = r_versionInfo.Match(indexFileAllText);

            while (m_versionInfo.Success)
            {
                indexFile.VersionInfo.Add(m_versionInfo.Value);
                m_versionInfo = m_versionInfo.NextMatch();
            }

            createDMIFile();
            createDMIOFile();


        }

        public void createDMIFile()
        {


            string dmiVersionInfo = "";
            int dummyIndex = 0;

            for (int i = 0; i < indexFile.VersionInfo.Count; i++)
            {

                string targetVersionInfo = (string)indexFile.VersionInfo[i];

                Regex r_fotaDatVersionInfo = new Regex(@"\[VERSIONINFO\].*?" + Regex.Escape(Right((string)jarFile.FotaDatFile[i], 8)) + @".*?\[END-VERSIONINFO\]", RegexOptions.Singleline);
                Match m_fotaDatVersionInfo = r_fotaDatVersionInfo.Match(targetVersionInfo);


                if (m_fotaDatVersionInfo.Success)
                {
                    //common
                    string fotaFileUrlPattern = @"(?<first>FileURL.*?=.*?:.*?)(?<port>:\d*)(?<last>/.*?dat)";
                    targetVersionInfo = Regex.Replace(targetVersionInfo, fotaFileUrlPattern, "${first}" + ":46105" + "${last}");


                    //fotadat on dmi
                    string dummyNumPattern = @"(?<pkgversion>PkgVersion.*?=.*?\.?\.?)(?<version>\d*)";
                    Regex r_dummyNumPattern = new Regex(dummyNumPattern, RegexOptions.Singleline);
                    Match m_dummyNumPattern = r_dummyNumPattern.Match(targetVersionInfo);
                    string targetVersion = m_dummyNumPattern.Groups["version"].Value;
                    dummyIndex = dummyIndex + 1;
                    string dummyVersion = (dummyIndex).ToString().PadLeft(targetVersion.Length, '0');

                    targetVersionInfo = Regex.Replace(targetVersionInfo, dummyNumPattern, "${pkgversion}" + dummyVersion);


                    //fota
                    string dummyFileSizePattern = @"(?<FileSize>FileSize.*?)=(?<filesize>\d*)";
                    targetVersionInfo = Regex.Replace(targetVersionInfo, dummyFileSizePattern, "${FileSize}=2");


                    //dummydatファイルの作成
                    StreamWriter sw1 = new StreamWriter(jarFileUnZipTempFolder + "/dmi/updatefile/" + jarFile.BaseName + "_" + Right((string)jarFile.FotaDatFile[i], 8), false, Encoding.GetEncoding("shift_jis"));
                    sw1.Write("1\n");
                    sw1.Close();




                }
                else
                {
                    MessageBox.Show("else " + i.ToString());
                    File.Copy(jarFileUnZipTempFolder + "/updatefile/" + jarFile.BaseName + "_" + Right((string)jarFile.DatFile[i], 8), jarFileUnZipTempFolder + "/dmi/updatefile/" + jarFile.BaseName + "_" + Right((string)jarFile.DatFile[i], 8));

                }


                dmiVersionInfo = dmiVersionInfo + targetVersionInfo + "\r\n";

            }
            string dmiIndexVersionInfoPattern = @"\[VERSIONINFO\].*\[END-VERSIONINFO\]";
            string dmiIndexFileAllText = indexFile.IndexFileAllText;

            dmiIndexFileAllText = Regex.Replace(dmiIndexFileAllText, dmiIndexVersionInfoPattern, dmiVersionInfo, RegexOptions.Singleline);
            MessageBox.Show(dmiIndexFileAllText);

            //書き込むファイルが既に存在している場合は、上書きする
            StreamWriter sw = new StreamWriter(jarFileUnZipTempFolder + "/dmi/" + jarFile.BaseName + ".txt", false, Encoding.GetEncoding("shift_jis"));
            //TextBox1.Textの内容を書き込む
            sw.Write(dmiIndexFileAllText);
            //閉じる
            sw.Close();



        }


        public void createDMIOFile()
        {


            string dmioVersionInfo = "";
            int dummyIndex = 0;

            for (int i = 0; i < indexFile.VersionInfo.Count; i++)
            {

                string targetVersionInfo = (string)indexFile.VersionInfo[i];


                //string datFileNamePattern = @"\[VERSIONINFO\].*?FileName.*?=.*?_(?<datVersion>.*?\d\d\d\d.dat).*?\[END-VERSIONINFO\]";
                //Match m_datFileName = Regex.Match(targetVersionInfo,datFileNamePattern,RegexOptions.Singleline);
                //MessageBox.Show("aaaaaaa="+m_datFileName.Groups["datVersion"].Value);
                //string datFileName = m_datFileName.Groups["datVersion"].Value;


                Regex r_osvDatVersionInfo = new Regex(@"\[VERSIONINFO\].*?" + Regex.Escape(Right((string)jarFile.OsvDatFile[i], 8)) + @".*?\[END-VERSIONINFO\]", RegexOptions.Singleline);
                Match m_osvDatVersionInfo = r_osvDatVersionInfo.Match(targetVersionInfo);

                if (m_osvDatVersionInfo.Success)
                {

                    string dummyNumPattern = @"(?<pkgversion>PkgVersion.*?=.*?\.?\.?)(?<version>\d*)";
                    string osvFileUrlPattern = @"(?<first>FileURL.*?=.*?:.*?)(?<port>:\d*)(?<last>/.*?dat)";
                    string dummyFileSizePattern = @"(?<FileSize>FileSize.*?)=(?<filesize>\d*)";

                    Regex r_dummyNumPattern = new Regex(dummyNumPattern, RegexOptions.Singleline);
                    Match m_dummyNumPattern = r_dummyNumPattern.Match(targetVersionInfo);
                    string targetVersion = m_dummyNumPattern.Groups["version"].Value;

                    dummyIndex = dummyIndex + 1;
                    string dummyVersion = (dummyIndex).ToString().PadLeft(targetVersion.Length, '0');

                    targetVersionInfo = Regex.Replace(targetVersionInfo, dummyNumPattern, "${pkgversion}" + dummyVersion);
                    targetVersionInfo = Regex.Replace(targetVersionInfo, osvFileUrlPattern, "${first}" + ":46110" + "${last}");
                    targetVersionInfo = Regex.Replace(targetVersionInfo, dummyFileSizePattern, "${FileSize}=2");

                    //dummydatファイルの作成
                    StreamWriter sw1 = new StreamWriter(jarFileUnZipTempFolder + "/dmio/updatefile/" + jarFile.BaseName + "_" + Right((string)jarFile.OsvDatFile[i], 8), false, Encoding.GetEncoding("shift_jis"));
                    sw1.Write("1\n");
                    sw1.Close();
                }
                else
                {
                    //common
                    string fotaFileUrlPattern = @"(?<first>FileURL.*?=.*?:.*?)(?<port>:\d*)(?<last>/.*?dat)";
                    targetVersionInfo = Regex.Replace(targetVersionInfo, fotaFileUrlPattern, "${first}" + ":46105" + "${last}");

                    File.Copy(jarFileUnZipTempFolder + "/updatefile/" + jarFile.BaseName + "_" + Right((string)jarFile.DatFile[i], 8), jarFileUnZipTempFolder + "/dmio/updatefile/" + jarFile.BaseName + "_" + Right((string)jarFile.DatFile[i], 8));

                }


                dmioVersionInfo = dmioVersionInfo + targetVersionInfo + "\r\n";

            }
            string dmioIndexVersionInfoPattern = @"\[VERSIONINFO\].*\[END-VERSIONINFO\]";
            string dmioIndexFileAllText = indexFile.IndexFileAllText;

            dmioIndexFileAllText = Regex.Replace(dmioIndexFileAllText, dmioIndexVersionInfoPattern, dmioVersionInfo, RegexOptions.Singleline);
            MessageBox.Show(dmioIndexFileAllText);

            //書き込むファイルが既に存在している場合は、上書きする
            StreamWriter sw = new StreamWriter(jarFileUnZipTempFolder + "/dmio/" + jarFile.BaseName + ".txt", false, Encoding.GetEncoding("shift_jis"));
            //TextBox1.Textの内容を書き込む
            sw.Write(dmioIndexFileAllText);
            //閉じる
            sw.Close();



        }

    }

   

}
