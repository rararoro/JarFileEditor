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
            label2.Text = files[0];

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

            if (jarFileExtention ==  ".jar")
            {             //Buttonを表示する
                button1 = new Button();
                button1.Name = "Button1";
                button1.Text = "jarファイルを解凍";
                button1.Location = new Point(105, 127);
                button1.Size = new Size(160, 20);
                button1.Click += new EventHandler(Button1_Click);
                this.Controls.Add(button1);
            }
            else {
                MessageBox.Show("拡張子が違います｡");
            }
        }

        private void Button1_Click(object sender, EventArgs e) {

            label2.Text = jarFile.FileName;
           
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
                    button2.Text = "datファイルを読み込み";
                    button2.Location = new Point(105, 150);
                    button2.Size = new Size(160, 20);
                    button2.Click += new EventHandler(Button2_Click);
                    this.Controls.Add(button2);
                    button2.Enabled = true;

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
                       
            for(int i =0; i<datFile.Count;i++)
            {
                checkbox1[i] = new CheckBox();
                checkbox1[i].Name = "CheckBox"+i.ToString();
                checkbox1[i].Text = Right((string)jarFile.DatFile[i],8);
                checkbox1[i].Size = new Size(151, 28);
                checkbox1[i].Location = new Point(100,170+(30*i));
                this.Controls.Add(checkbox1[i]);
                //MessageBox.Show((string)jarFile.DatFiles1[i]);

            }

            button3 = new Button();//OSVdat読み込み用のボタンを表示
            button3.Name = "Button1";
            button3.Text = "datファイルを読み込み";
            button3.Location = new Point(310, 150);
            button3.Size = new Size(160, 20);
            button3.Click += new EventHandler(Button3_Click);
            this.Controls.Add(button3);
            button3.Enabled = true;

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ArrayList osvDatFile =jarFile.OsvDatFile;
            ArrayList fotaDatFile = jarFile.FotaDatFile;
            button3.Enabled = false;
            for (int i = 0; i < checkbox1.Length; i++)
            {

                if (checkbox1[i].Checked == true)
                {
                    osvDatFile.Add(jarFile.DatFile[i]);
                }
                else if (checkbox1[i].Checked == false)
                {
                    fotaDatFile.Add(jarFile.DatFile[i]);
                }

                checkbox1[i].Enabled = false;
            }

            //for (int i = 0; i < osvDatFile.Count; i++)
            //{
               
            //   // MessageBox.Show("osvは"+ Right((string)jarFile.DatFiles1[i], 8));

            //}


            create_dmi_file_button = new Button();//OSVdat読み込み用のボタンを表示
            create_dmi_file_button.Name = "Button1";
            create_dmi_file_button.Text = "datファイルを作成";
            create_dmi_file_button.Location = new Point(310, 175);
            create_dmi_file_button.Size = new Size(160, 20);
            create_dmi_file_button.Click += new EventHandler(Create_DMI_File_Button_Click);
            this.Controls.Add(create_dmi_file_button);
            create_dmi_file_button.Enabled = true;

        }

        private void Create_DMI_File_Button_Click(object sender, EventArgs e)
        {
            create_dmi_file_button.Enabled = false;
            string dmiFileFolderName = jarFile.DirectoryName + "/unzip/dmi/updatefile";
            string dmioFileFolderName = jarFile.DirectoryName + "/unzip/dmio/updatefile";
            System.IO.DirectoryInfo di;

            //tempフォルダを作成
            di = System.IO.Directory.CreateDirectory(dmiFileFolderName);
            di = System.IO.Directory.CreateDirectory(dmioFileFolderName);

            try {
                MessageBox.Show("y");
                createDatFile();



            }
            catch (System.IO.IOException ex) {

                MessageBox.Show(ex.Message);
                //test

            }





        }

        public void createDatFile(){

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(jarFileUnZipTempFolder);
            System.IO.FileInfo[] files =
            di.GetFiles("*.txt", System.IO.SearchOption.TopDirectoryOnly);
           string indexfilePath=  files[0].FullName;


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


            string dmiVersionInfo = "";

            for (int i =0; i <indexFile.VersionInfo.Count;i++) {
                
                string targetVersionInfo = (string)indexFile.VersionInfo[i];

                for(int j = 0; j < jarFile.FotaDatFile.Count; j++) {
                    Regex r_fotaDatVersionInfo =new Regex(@"\[VERSIONINFO\].*?" + Regex.Escape(Right((string)jarFile.FotaDatFile[j], 8)) + @".*?\[END-VERSIONINFO\]",RegexOptions.Singleline);
                    Match m_fotaDatVersionInfo = r_fotaDatVersionInfo.Match(targetVersionInfo);

                    if (m_fotaDatVersionInfo.Success)
                    {
                        MessageBox.Show("if"+i.ToString());
                        string dummyNumPattern = @"(?<pkgversion>PkgVersion.*?=.*?\.?\.?)(?<version>\d*?)\r";
                        string fotaFileUrlPattern = @"(?<first>FileURL.*?=.*?:.*?)(?<port>:\d*)(?<last>/.*?dat)";
                        string dummyFileSizePattern = @"(?<index>FileSize.*?)=(?<filesize>\d*)";

                        Regex r_dummyNumPattern = new Regex(dummyNumPattern, RegexOptions.Singleline);
                        Match m_dummyNumPattern = r_dummyNumPattern.Match(targetVersionInfo);
                        string targetVersion = m_dummyNumPattern.Groups["version"].Value;
                        string dummyVersion = (j + 1).ToString().PadLeft(targetVersion.Length, '0');

                        targetVersionInfo = Regex.Replace(targetVersionInfo, dummyNumPattern, "${pkgversion}" + dummyVersion);
                        targetVersionInfo = Regex.Replace(targetVersionInfo, fotaFileUrlPattern, "${first}" + ":46105" + "${last}");
                        targetVersionInfo = Regex.Replace(targetVersionInfo, dummyFileSizePattern, "$1=2");

                        StreamWriter sw1 = new StreamWriter(jarFileUnZipTempFolder + "/dmi/updatefile/" + jarFile.BaseName + "_" + Right((string)jarFile.FotaDatFile[j], 8), false, Encoding.GetEncoding("shift_jis"));
                        sw1.Write("1\n");
                        sw1.Close();
                    }
                    MessageBox.Show("else" + i.ToString());

                }
                    File.Copy(jarFileUnZipTempFolder + "/updatefile/" + jarFile.BaseName + "_" + Right((string)jarFile.DatFile[i], 8), jarFileUnZipTempFolder + "/dmi/updatefile/" + jarFile.BaseName + "_" + Right((string)jarFile.DatFile[i], 8));

                dmiVersionInfo = dmiVersionInfo + targetVersionInfo+"\r\n";
                
            }
            string dmiIndexVersionInfoPattern = @"\[VERSIONINFO\].*\[END-VERSIONINFO\]";
            string dmiIndexFileAllText = indexFile.IndexFileAllText;

            dmiIndexFileAllText = Regex.Replace(dmiIndexFileAllText,dmiIndexVersionInfoPattern,dmiVersionInfo,RegexOptions.Singleline);
            MessageBox.Show(dmiIndexFileAllText);

            //書き込むファイルが既に存在している場合は、上書きする
            StreamWriter sw = new StreamWriter(jarFileUnZipTempFolder+"/dmi/"+jarFile.BaseName+".txt",false,Encoding.GetEncoding("shift_jis"));
            //TextBox1.Textの内容を書き込む
            sw.Write(dmiIndexFileAllText);
            //閉じる
            sw.Close();



        }



    }


}
