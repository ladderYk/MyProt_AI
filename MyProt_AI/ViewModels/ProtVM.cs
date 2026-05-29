using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace MyProt_AI.ViewModels
{
    public class ProtVM
    {
        private static readonly ProtVM _Instance = new ProtVM();
        public static ProtVM Instance => _Instance;

        public JArray getProtocolList(string sbody)
        {
            return JArray.FromObject(MainWindow.gateway.getList());
        }
        public bool editProtocol(string sbody)
        {
            var path = "./Protocols/S7.json";
            var pathB = "./Protocols/S7_o.json";
            if (File.Exists(path))
            {
                File.Copy(path, pathB, true);
                //FileStream fs = new FileStream(path, FileMode.Truncate);
               // StreamWriter sr = new StreamWriter(fs);
                //sr.Flush(); // 清空缓冲区
                //sr.BaseStream.SetLength(0); // 将文件长度截断为 0
            }
            return true;
        }
    }

}
