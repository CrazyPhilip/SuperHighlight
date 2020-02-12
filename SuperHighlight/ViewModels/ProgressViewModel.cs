using SuperHighlight.Exporters;
using SuperHighlight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuperHighlight.ViewModels
{
    public class ProgressViewModel : BaseViewModel
    {
        private float rate;   //进度
        public float Rate
        {
            get { return rate; }
            set { SetProperty(ref rate, value); }
        }

        private Thread thread;

        public ProgressViewModel(string SelectedLanguage, string SelectedFont, string SelectedFontSize, string SelectedTheme, string OutputFolderPath, List<FileInformation> FileList)
        {
            Dictionary<string, string> exporters = new Dictionary<string, string>
            {
                { "Python", "PythonExporter"},
                { "Java", "JavaExporter"},
                { "C#", "CsharpExporter"},
                { "PHP", "PhpExporter"},
                { "C++", "CppExporter"}
            };

            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "language", SelectedLanguage},
                { "title", ""},
                { "font", SelectedFont},
                { "fontsize", SelectedFontSize.ToString()},
                { "theme", SelectedTheme},
                { "content", ""}
            };

            //新建一个线程
            thread = new Thread(new ThreadStart(() =>
            {
                //动态实例化exporter，根据选择的语言实例化对应的exporter
                Type type = Type.GetType("SuperHighlight.Exporters." + exporters[SelectedLanguage]);
                var exporter = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod("Export", new Type[] { typeof(string), typeof(string), typeof(string), typeof(Dictionary<string, string>)});
                int index = 0;

                foreach (var file in FileList)
                {
                    if (file.Selected)
                    {
                        dic["title"] = file.FileName;
                        method.Invoke(exporter, new object[] { file.FullPath + "\\" + file.FileName, OutputFolderPath, file.FileName + ".html", dic });
                    }

                    index++;
                    Rate = index * 100 / FileList.Count();
                }
            }));
            thread.Start();
        }

        ~ProgressViewModel()
        {
            thread.Abort();
        }
    }
}
