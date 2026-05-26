using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProt_AI
{
    public class ProtocolConfig
    {
        // 协议名称
        public string ProtocolName { get; set; }
        // 通讯类型
        public TransportConfig Transport { get; set; }
        // 连接配置
        public ConnectionConfig Connection { get; set; }
        // 协议列表
        public Dictionary<string, OperationConfig> Operations { get; set; }
        // 构建方法
        public List<string> BuiltInFunctions { get; set; }
        // 握手协议
        public List<HandshakeConfig> Handshake { get; set; }
        // 返回帧结构
        public FramingConfig Framing { get; set; }
    }

    public class TransportConfig
    {
        public string Type { get; set; }   // "Tcp", "Serial"
        public int DefaultPort { get; set; }
    }
    public class ConnectionConfig
    {
        public int ResponseTimeoutMs { get; set; } = 1000;
        public int InterFrameDelayMs { get; set; } = 0;
    }

    public class OperationConfig
    {
        public List<string> RequestTemplate { get; set; }
        public ResponseParserConfig ResponseParser { get; set; }
    }

    public class ResponseParserConfig
    {
        public string ValidCondition { get; set; }
        public int DataStartIndex { get; set; }
        public string DataLengthExpr { get; set; }
        public string ValueType { get; set; }   // "ByteArray", "UInt16", "Empty"
    }

    public class DeviceConfig
    {
        public string Id { get; set; }
        public string Protocol { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class TagDefinition
    {
        public string TagName { get; set; }
        public string DeviceId { get; set; }
        public string Operation { get; set; }
        public Dictionary<string, object> Variables { get; set; }
        //public string FinalType { get; set; }   // "Float", "Bool", "UInt16", "String" ...
        public int ScanRateMs { get; set; }
    }
    public class HandshakeConfig
    {
        public string Name { get; set; }
        public List<string> RequestTemplate { get; set; }
        public FramingConfig Framing { get; set; }
        public string ValidCondition { get; set; }
    }
    public class FramingConfig
    {
        public string Type { get; set; }                         // "LengthField", "Fixed"
        public int? LengthFieldOffset { get; set; }
        public int? LengthFieldLength { get; set; }
        public bool? LengthIncludesHeader { get; set; }
        public string ByteOrder { get; set; }                    // "BigEndian", "LittleEndian"
        public int? HeaderLength { get; set; }                   // 仅当 !LengthIncludesHeader
        public int? FixedLength { get; set; }                    // Fixed 模式
    }
}
