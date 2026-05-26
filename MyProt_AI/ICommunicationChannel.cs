using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProt_AI
{
    public interface ICommunicationChannel : IDisposable
    {
        Task ConnectAsync(string portOrIp, object settings);
        Task<byte[]> SendReceiveAsync(byte[] request, int timeoutMs);
    }
}
