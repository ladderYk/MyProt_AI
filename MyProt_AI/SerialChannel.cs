using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProt_AI
{
    public class SerialChannel : ICommunicationChannel
    {
        private SerialPort _sp;
        public async Task ConnectAsync(string portName, object settings) { /* 打开串口 */ }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> SendReceiveAsync(byte[] request, int timeoutMs) { /* 发送、读取完整帧 */
            return null;
        }
    }
}
