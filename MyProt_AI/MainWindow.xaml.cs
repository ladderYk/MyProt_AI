using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyProt_AI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //    List<string> template = new List<string>() {
            //        "{SlaveAddress:X2}",
            //  "03",
            //"{StartAddress:X4}",
            //"{RegisterCount:X4}",
            //"{CRC16Modbus:X4}"
            //    };
            //string md5 = GetMd5(1779528618993 + "zlzk.117#qms");
            //Console.WriteLine(md5);
            getDataAsync();
        }
        private void getDataAsync()
        {
            try
            {
                var gateway = new ProtocolGateway("./Protocols", "./tags.json");
                TagValue temp = gateway.ReadTagAsync("DB1_Real0");
                Console.WriteLine($"温度: {temp.Value}");
            }
            catch (Exception e)
            {
            }
        }
        public string GetMd5(object text)
        {
            using(MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(text.ToString());
                return BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", "").ToLower();
            }
        }
        public string BytesToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
