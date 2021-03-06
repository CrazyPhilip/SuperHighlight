﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperHighlight.Exporters
{
    public class PythonExporter
    {
        /// <summary>
        /// Python导出到Html，Dark主题
        /// </summary>
        /// <param name="InputFilePath">Python文件全路径</param>
        /// <param name="OutputFilePath">输出Html文件全路径</param>
        /// <returns></returns>
        public int Export(string InputFilePath, string OutputFilePath, string OutputFileName, Dictionary<string, string> dic)
        {
            string template_path = Application.StartupPath + "\\Themes\\" + dic["language"] + "_" + dic["theme"] + "_template.html";
            ToHtml html = new ToHtml();
            string msg = "";

            //dic.Add("content", Export(InputFilePath));
            dic["content"] = Generate(InputFilePath);

            html.Create(template_path, OutputFilePath, OutputFileName, dic, ref msg);

            return 1;
        }

        /// <summary>
        /// 空格  0，#注释 1，"双引号  2，'单引号  3
        /// 英文字符和下划线  4，数字  5，符号  6
        /// 其他  10
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private int Recognize(char ch)
        {
            char[] operators =
            {
                '+', '-', '*', '/', '%', '=', '!', '>', '<', '=',
                ':', '?', '.', '{', '}', '[', ']', '(', ')', '&',
                '|', '^', '~', ','
            };

            if (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t')
            {
                return 0;
            }

            if (ch == '#')
            {
                return 1;
            }

            if (ch == '\"')
            {
                return 2;
            }

            if (ch == '\'')
            {
                return 3;
            }

            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_')
            {
                return 4;
            }

            if (ch >= '0' && ch <= '9')
            {
                return 5;
            }

            if (operators.Contains(ch))
            {
                return 6;
            }

            return 10;
        }

        /// <summary>
        /// 识别匹配python代码，返回已编码的content
        /// </summary>
        /// <param name="InputFilePath">输入python文件路径</param>
        /// <returns>已编码的content</returns>
        private string Generate(string InputFilePath)
        {
            string[] keywords =
                { "False", "None", "True", "and", "as", "assert", "break", "class",
                "continue", "def", "del", "elif", "else", "except", "finally", "for",
                "from", "global", "if", "import", "in", "is", "lambda", "nonlocal", "not",
                "or", "pass", "raise", "return", "try", "while", "with", "yield" };

            string content = string.Empty;

            char[] char_array;
            int _flag;    //全局flag   状态标志
            int index = 0;

            //读入文件
            using (FileStream reader = new FileStream(InputFilePath, FileMode.Open))
            {
                int len = (int)reader.Length;
                byte[] bytes = new byte[len];

                reader.Read(bytes, 0, len);

                string lines = Encoding.UTF8.GetString(bytes);
                char_array = lines.ToCharArray();
            }

            try
            {
                _flag = 0;
                int flag = 0;
                string temp = string.Empty;

                do
                {
                    flag = Recognize(char_array[index]);

                    if (flag == 0)
                    {
                        //writer.Write(char_array[index]);
                        content += char_array[index];
                        index++;
                        continue;
                    }

                    _flag = flag;

                    switch (_flag)
                    {
                        case 1:
                            {
                                while (char_array[index] != '\n')
                                {
                                    temp += char_array[index];

                                    index++;
                                }

                                content += "<span class=\"sc1\">" + temp + "</span>";
                                temp = string.Empty;
                                _flag = 0;
                                index--;
                            }
                            break;

                        case 2:
                            {
                                int total = Recognize(char_array[index + 1]) == flag && Recognize(char_array[index + 2]) == flag ? 6 : 2;
                                int count = 0;

                                while (count < total)
                                {
                                    temp += char_array[index];

                                    if (char_array[index] == '\"')
                                    {
                                        count++;
                                    }
                                    index++;
                                }

                                //writer.Write("<span class=\"sc3\">" + temp + "</span>");
                                content += "<span class=\"sc3\">" + temp + "</span>";
                                temp = string.Empty;
                                _flag = 0;
                                index--;
                                //continue;
                            }
                            break;

                        case 3:
                            {
                                int total = Recognize(char_array[index + 1]) == flag && Recognize(char_array[index + 2]) == flag ? 6 : 2;
                                int count = 0;

                                while (count < total)
                                {
                                    temp += char_array[index];

                                    if (char_array[index] == '\'')
                                    {
                                        count++;
                                    }
                                    index++;
                                }

                                //writer.Write("<span class=\"sc4\">" + temp + "</span>");
                                content += "<span class=\"sc4\">" + temp + "</span>";
                                temp = string.Empty;
                                _flag = 0;
                                index--;
                                //continue;
                            }
                            break;

                        case 4:
                            {
                                do
                                {
                                    temp += char_array[index];

                                    index++;
                                    flag = Recognize(char_array[index]);

                                } while (flag == 4 || flag == 5);

                                if (keywords.Contains(temp))
                                {
                                    //writer.Write("<span class=\"sc5\">" + temp + "</span>");
                                    content += "<span class=\"sc5\">" + temp + "</span>";
                                }
                                else
                                {
                                    //writer.Write("<span class=\"sc0\">" + temp + "</span>");
                                    content += "<span class=\"sc0\">" + temp + "</span>";
                                }

                                temp = string.Empty;
                                _flag = 0;
                                index--;
                            }
                            break;

                        case 5:
                            {
                                do
                                {
                                    temp += char_array[index];

                                    index++;
                                    flag = Recognize(char_array[index]);

                                } while (flag == 5);

                                //writer.Write("<span class=\"sc2\">" + temp + "</span>");
                                content += "<span class=\"sc2\">" + temp + "</span>";

                                temp = string.Empty;
                                _flag = 0;
                                index--;
                            }
                            break;

                        case 6:
                            {
                                do
                                {
                                    temp += char_array[index];

                                    index++;
                                    flag = Recognize(char_array[index]);

                                } while (flag == 6);

                                //writer.Write("<span class=\"sc10\">" + temp + "</span>");
                                content += "<span class=\"sc10\">" + temp + "</span>";

                                temp = string.Empty;
                                _flag = 0;
                                index--;
                            }
                            break;

                        default:
                            break;
                    }

                    index++;
                } while (index < char_array.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return content;
        }
    }
}

