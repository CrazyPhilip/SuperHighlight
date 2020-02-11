using SuperHighlight.Exporters;
using SuperHighlight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ProgressViewModel(string SelectedLanguage, string SelectedFont, string SelectedFontSize, string SelectedTheme, string OutputFolderPath, List<FileInformation> FileList)
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

            Thread thread = new Thread(new ThreadStart(() =>
            {
                //for (int i = 0; i <= 100; i++)
                //{
                //    //this.progressBar1.Dispatcher.BeginInvoke((ThreadStart)delegate { this.progressBar1.Value = i; });
                //    Rate = i;
                //    Thread.Sleep(100);
                //}
                PythonExporter pythonExporter = new PythonExporter();
                int index = 0;

                foreach (var file in FileList)
                {
                    if (file.Selected)
                    {
                        dic["title"] = file.FileName;
                        pythonExporter.PythonToHtml(file.FullPath + "\\" + file.FileName, OutputFolderPath, file.FileName + ".html", dic);
                    }

                    index++;
                    Rate = index * 100 / FileList.Count();
                }
            }));
            thread.Start();
        }
    }
}
