using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperHighlight.Models
{
    public class FileInformation
    {
        [JsonProperty("Selected")]
        public bool Selected { get; set; }   //是否选择

        [JsonProperty("FileName")]
        public string FileName { get; set; }   //文件名

        [JsonProperty("Size")]
        public string Size { get; set; }   //文件大小

        [JsonProperty("EditTime")]
        public string EditTime { get; set; }   //修改时间


    }
}
