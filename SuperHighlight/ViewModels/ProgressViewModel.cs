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
        private int rate;   //进度
        public int Rate
        {
            get { return rate; }
            set { SetProperty(ref rate, value); }
        }

        public ProgressViewModel()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    //this.progressBar1.Dispatcher.BeginInvoke((ThreadStart)delegate { this.progressBar1.Value = i; });
                    Thread.Sleep(100);
                }

            }));
            thread.Start();
        }

    }
}
