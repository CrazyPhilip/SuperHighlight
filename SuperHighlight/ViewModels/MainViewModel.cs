using System;
using System.Collections.Generic;
using SuperHighlight.Models;
using SuperHighlight.Commands;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using SuperHighlight.Exporters;
using System.Xml;

namespace SuperHighlight.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private List<string> languageList;   //编程语言列表
        public List<string> LanguageList
        {
            get { return languageList; }
            set { SetProperty(ref languageList, value); }
        }

        private List<string> fontList;   //字体列表
        public List<string> FontList
        {
            get { return fontList; }
            set { SetProperty(ref fontList, value); }
        }

        private List<int> fontSizeList;   //字号列表
        public List<int> FontSizeList
        {
            get { return fontSizeList; }
            set { SetProperty(ref fontSizeList, value); }
        }

        private List<string> themeList;   //主题列表
        public List<string> ThemeList
        {
            get { return themeList; }
            set { SetProperty(ref themeList, value); }
        }

        private List<FileInformation> fileList;   //文件列表
        public List<FileInformation> FileList
        {
            get { return fileList; }
            set { SetProperty(ref fileList, value); }
        }

        private string selectedLanguage;   //选择的语言
        public string SelectedLanguage
        {
            get { return selectedLanguage; }
            set { SetProperty(ref selectedLanguage, value); }
        }

        private string selectedFont;   //选择的字体
        public string SelectedFont
        {
            get { return selectedFont; }
            set { SetProperty(ref selectedFont, value); }
        }

        private int selectedFontSize;   //选择的字号
        public int SelectedFontSize
        {
            get { return selectedFontSize; }
            set { SetProperty(ref selectedFontSize, value); }
        }

        private string selectedTheme;   //选择的主题
        public string SelectedTheme
        {
            get { return selectedTheme; }
            set { SetProperty(ref selectedTheme, value); }
        }

        private string themeIamge;   //Comment
        public string ThemeImage
        {
            get { return themeIamge; }
            set { SetProperty(ref themeIamge, value); }
        }

        private string inputFolderPath;   //输入文件夹路径
        public string InputFolderPath
        {
            get { return inputFolderPath; }
            set { SetProperty(ref inputFolderPath, value); }
        }

        private string outputFolderPath;   //输出文件夹路径
        public string OutputFolderPath
        {
            get { return outputFolderPath; }
            set { SetProperty(ref outputFolderPath, value); }
        }

        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute( IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        public Command InputFolderCommand { get; private set; }

        public Command OutputFolderCommand { get; private set; }

        public Command OpenInputFolderCommand { get; private set; }

        public Command OpenOutputFolderCommand { get; private set; }

        public Command GenerateCommand { get; private set; }

        public Command SelectThemeCommand { get; private set; }

        public MainViewModel()
        {
            LanguageList = new List<string>();
            ThemeList = new List<string>();
            FontSizeList = new List<int>();
            FontList = new List<string>();

            Init(Application.StartupPath + @"\Configuration\Configuration.xml");
            
            SelectedFont = "Consolas";
            SelectedLanguage = "Python";
            SelectedFontSize = 12;
            SelectedTheme = "Dark";

            InputFolderCommand = new Command(() =>
            {
                FolderBrowserDialog openFileDialog = new FolderBrowserDialog();  //选择文件夹

                if (openFileDialog.ShowDialog() == DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
                {
                    InputFolderPath = openFileDialog.SelectedPath;

                    List<FileInformation> list = new List<FileInformation>();
                    FileInformation temp;
                    //遍历文件夹
                    DirectoryInfo theFolder = new DirectoryInfo(InputFolderPath);
                    FileInfo[] thefileInfo = theFolder.GetFiles("*.py", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo NextFile in thefileInfo) //遍历文件
                    {
                        temp = new FileInformation
                        {
                            Selected = true,
                            FileName = NextFile.Name,
                            EditTime = NextFile.LastWriteTime.ToString(),
                            Size = NextFile.Length.ToString(),
                            FullPath = NextFile.DirectoryName
                        };

                        list.Add(temp);
                    }

                    //  暂时不包含子文件夹
                    //遍历子文件夹
                    DirectoryInfo[] dirInfo = theFolder.GetDirectories();
                    foreach (DirectoryInfo NextFolder in dirInfo)
                    {
                        //list.Add(NextFolder.ToString());
                        FileInfo[] fileInfo = NextFolder.GetFiles("*.py", SearchOption.AllDirectories);
                        foreach (FileInfo NextFile in fileInfo) //遍历文件
                        {
                            temp = new FileInformation
                            {
                                Selected = true,
                                FileName = NextFile.Name,
                                EditTime = NextFile.LastWriteTime.ToString(),
                                Size = NextFile.Length.ToString(),
                                FullPath = NextFile.DirectoryName
                            };

                            list.Add(temp);
                        }
                    }

                    FileList = list;
                }
            }, () => { return true; });

            OutputFolderCommand = new Command(() =>
            {
                FolderBrowserDialog openFileDialog = new FolderBrowserDialog();  //选择文件夹

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    OutputFolderPath = openFileDialog.SelectedPath;
                }
            }, () => { return true; });

            OpenInputFolderCommand = new Command(() => 
            {
                ShellExecute(IntPtr.Zero, "open", InputFolderPath, "", "", 4);

            }, () => { return true; });

            OpenOutputFolderCommand = new Command(() =>
            {
                ShellExecute(IntPtr.Zero, "open", OutputFolderPath, "", "", 4);

            }, () => { return true; });

            GenerateCommand = new Command(() =>
            {
                Distribute();
            }, () => { return true; });

            SelectThemeCommand = new Command(() =>
            {
                ThemeImage = "/Themes/" + SelectedLanguage + "_" + SelectedTheme + ".PNG";
                //Console.WriteLine(ThemeImage);
            }, () => { return true; });
        }

        private void Init(string file)
        {
            XmlDocument xmlDocument = new XmlDocument();
            if (File.Exists(file))
            {
                xmlDocument.Load(file);

                XmlNodeList languageList = xmlDocument.SelectNodes("/Configuration/Languages/Language");
                if (languageList.Count>0)
                {
                    foreach (XmlNode item in languageList)
                    {
                        LanguageList.Add(item.Attributes["Name"].Value);
                    }
                }

                XmlNodeList themeList = xmlDocument.SelectNodes("/Configuration/Themes/Theme");
                if (themeList.Count>0)
                {
                    foreach (XmlNode item in themeList)
                    {
                        ThemeList.Add(item.InnerText);
                    }
                }

                XmlNodeList fontSizeList = xmlDocument.SelectNodes("/Configuration/FontSizes/FontSize");
                if (fontSizeList.Count > 0)
                {
                    foreach (XmlNode item in fontSizeList)
                    {
                        FontSizeList.Add(int.Parse(item.InnerText));
                    }
                }

                XmlNodeList fontList = xmlDocument.SelectNodes("/Configuration/Fonts/Font");
                if (fontList.Count>0)
                {
                    foreach (XmlNode item in fontList)
                    {
                        FontList.Add(item.InnerText);
                    }
                }
            }
        }

        private void Distribute()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "language", SelectedLanguage},
                { "title", ""},
                { "font", SelectedFont},
                { "fontsize", SelectedFontSize.ToString()},
                { "theme", SelectedTheme},
                { "content", ""}
            };

            if (SelectedLanguage == "Python")
            {
                PythonExporter pythonExporter = new PythonExporter();

                foreach (var file in FileList)
                {
                    if (file.Selected)
                    {
                        dic["title"] = file.FileName;
                        pythonExporter.PythonToHtml(file.FullPath + "\\" + file.FileName, OutputFolderPath, file.FileName + ".html", dic);
                    }
                }

            }
        }
    }
}
