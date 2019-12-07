using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperHighlight.Exporters
{
    public class ToHtml
    {
        /// <summary>
        /// 导出Html
        /// </summary>
        /// <param name="templatepath">模板文件路径</param>
        /// <param name="htmlpath">导出html路径</param>
        /// <param name="dic">参数字典</param>
        /// <param name="message">异常信息</param>
        /// <returns></returns>
        public bool Create(string templatepath, string htmlpath, Dictionary<string, string> dic, ref string message)
        {
            bool result = false;
            //string htmlnamepath = Path.Combine(htmlpath, htmlname);
            Encoding encode = Encoding.UTF8;
            StringBuilder html = new StringBuilder();

            try
            {
                //读取模版
                html.Append(File.ReadAllText(templatepath, encode));
            }
            catch (FileNotFoundException ex)
            {
                message = ex.Message;
                return false;
            }

            foreach (KeyValuePair<string, string> d in dic)
            {
                //替换数据
                html.Replace(
                    string.Format("${0}$", d.Key),
                    d.Value);
            }

            try
            {
                //写入html文件
                if (!Directory.Exists(htmlpath))
                    Directory.CreateDirectory(htmlpath);
                File.WriteAllText(htmlpath, html.ToString(), encode);
                result = true;
            }
            catch (IOException ex)
            {
                message = ex.Message;
                return false;
            }

            return result;
        }
    }
}
