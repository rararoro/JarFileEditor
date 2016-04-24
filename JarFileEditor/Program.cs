using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace JarFileEditor
{
    class JarFile {
        private string fileName;
        private string directoryName;
        private string baseName;
        private string filePath;
        private string jarExePath;
        private ArrayList datFiles;
        private ArrayList osvDatFile;
        private ArrayList fotaDatFile;

        public JarFile(){
            
            fileName="";
            directoryName="";
            baseName = "";
            filePath = "";
            jarExePath="";
            datFiles =new ArrayList();
            osvDatFile = new ArrayList();
            fotaDatFile = new ArrayList();
            }

        public string FileName{
            set{ this.fileName = value;}
            get { return this.fileName; }
            
            }
        public string DirectoryName {

            set { this.directoryName = value; }
            get { return this.directoryName; }
        }
        public string BaseName
        {

            set { this.baseName = value; }
            get { return this.baseName; }
        }
        public string FilePath
        {

            set { this.filePath = value; }
            get { return this.filePath; }
        }
        public string JarExePath
        {
            set { this.jarExePath = value; }
            get { return this.jarExePath; }
        }
       
        public ArrayList DatFile
        {
            get
            {
                return this.datFiles;
            }

            set
            {
                this.datFiles = value;
            }
        }
        public ArrayList OsvDatFile
        {
            get{return this.osvDatFile;}
            set{this.osvDatFile = value;}
        }
        public ArrayList FotaDatFile
        {
            get { return this.fotaDatFile;}
            set {this.fotaDatFile = value; }
            
        }

    }

    class IndexFile {

        private string indexFileAllText;//indexファイル全文
        private ArrayList versionInfo;
        private ArrayList latestInfo;

        public IndexFile() {

            indexFileAllText = "";
            versionInfo = new ArrayList();
            latestInfo = new ArrayList();


        }

        public string IndexFileAllText {
            set{this.indexFileAllText = value;}
            get{return this.indexFileAllText;}
        }

        public ArrayList VersionInfo
        {
            set { this.versionInfo = value; }
            get { return this.versionInfo; }
        }

        public ArrayList LatestInfo
        {
            set { this.latestInfo = value; }
            get { return this.latestInfo; }
        }


    }

    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
