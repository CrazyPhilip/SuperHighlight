using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperHighlight.Exporters
{
    public class CppExporter
    {
        
        public int CppToHtml(string InputFilePath, string OutputFilePath, string OutputFileName, Dictionary<string, string> dic)
        {
            string template_path = Application.StartupPath + "\\Themes\\" + dic["language"] + "_" + dic["theme"] + "_template.html";
            ToHtml html = new ToHtml();
            string msg = "";

            dic["content"] = Export(InputFilePath);

            html.Create(template_path, OutputFilePath, OutputFileName, dic, ref msg);

            return 1;
        }

        private int Recognize(char ch)
        {
            char[] operators =
            {
                '+', '-', '*', '%', '=', '!', '=',
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

            if (ch == '/')
            {
                return 7;
            }

            if (operators.Contains(ch))
            {
                return 6;
            }

            if (ch== '<')
            {
                return 8;
            }

            if (ch=='>')
            {
                return 9;
            }
            
            return 10;
        }

        private string Export(string InputFilePath)
        {
            string[] keywords =
            {
                "if", "else", "while", "signed", "throw", "union", "this",
                "int", "char", "double", "unsigned", "const", "goto", "virtual",
                "for", "float", "break", "auto", "class", "operator", "case",
                "do", "long", "typeof", "static", "friend", "template", "default",
                "new", "void", "register", "extern", "return", "enum", "inline",
                "try", "short", "continue", "sizeof", "switch", "private", "protected",
                "asm", "catch", "delete", "public", "volatile", "struct"
            };

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
                        case 7:
                            {
                                if (char_array[index+1] == '/')
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
                                else if (char_array[index + 1] == '=')
                                {
                                    index++;
                                    temp += char_array[index];
                                    //writer.Write("<span class=\"sc10\">" + temp + "</span>");
                                    content += "<span class=\"sc10\">" + temp + "</span>";

                                    temp = string.Empty;
                                    _flag = 0;
                                    //index--;
                                }
                                else if (char_array[index + 1] == '*')
                                {
                                    while (char_array[index] != '/')
                                    {
                                        temp += char_array[index];

                                        index++;
                                    }

                                    content += "<span class=\"sc1\">" + temp + "</span>";
                                    temp = string.Empty;
                                    _flag = 0;
                                    index--;
                                }
                            }
                            break;

                        case 2:
                            {
                                //int total = Recognize(char_array[index + 1]) == flag && Recognize(char_array[index + 2]) == flag ? 6 : 2;
                                //int count = 0;

                                while (char_array[index] != '\"')
                                {
                                    temp += char_array[index];

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
                                //int total = Recognize(char_array[index + 1]) == flag && Recognize(char_array[index + 2]) == flag ? 6 : 2;
                                //int count = 0;

                                while (char_array[index] != '\'')
                                {
                                    temp += char_array[index];

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

                        case 8:
                            {

                            }

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
