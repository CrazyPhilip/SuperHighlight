using System;
using System.Collections.Generic;
using SuperHighlight.Models;
using SuperHighlight.Commands;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

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

        private string selectedFontSize;   //选择的字号
        public string SelectedFontSize
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
            LanguageList = new List<string>
            {
                "Python", "Java", "C#", "PHP"
            };

            FontSizeList = new List<int> { 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28 };

            FontList = new List<string> { "Consolas", "Courier New" };

            ThemeList = new List<string>
            {
                "Dark", "Light"
            };

            FileList = new List<FileInformation>
            {
                new FileInformation{Selected=false, FileName="graph.py", Size="15kb"},
                new FileInformation{Selected=false, FileName="graph1.py", Size="15kb"},
                new FileInformation{Selected=false, FileName="graph2.py", Size="15kb"}
            };

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
                        };

                        list.Add(temp);
                    }

                    FileList = list;
                    /*  暂时不包含子文件夹
                    //遍历子文件夹
                    DirectoryInfo[] dirInfo = theFolder.GetDirectories();
                    foreach (DirectoryInfo NextFolder in dirInfo)
                    {
                        //list.Add(NextFolder.ToString());
                        FileInfo[] fileInfo = NextFolder.GetFiles("*.py", SearchOption.AllDirectories);
                        foreach (FileInfo NextFile in fileInfo) //遍历文件
                        {
                            list.Add(NextFile.FullName);
                        }
                    }*/

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
                Console.WriteLine(SelectedFont);
                Console.WriteLine(SelectedFontSize);
                Console.WriteLine(SelectedTheme);
            }, () => { return true; });

            SelectThemeCommand = new Command(() =>
            {
                ThemeImage = "/Themes/" + SelectedLanguage + "_" + SelectedTheme + ".PNG";
                //Console.WriteLine(ThemeImage);
            }, () => { return true; });
        }
    }
}
