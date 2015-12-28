using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;

namespace ServerData
{
    [Serializable]
    public class Packet
    {
        public List<string> Gdata;
        public int packetInt;
        public bool packetBool;
        public string senderID;
        public PacketType PacketType;

        public Packet(PacketType type, string senderID) {
            Gdata = new List<string>();
            this.senderID = senderID;
            this.PacketType = type;
        }

        public Packet(byte[] packetbytes) {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetbytes);
            Packet p = (Packet)bf.Deserialize(ms);
            ms.Close();
            this.Gdata = p.Gdata;
            this.packetInt = p.packetInt;
            this.packetBool = p.packetBool;
            this.senderID = p.senderID;
            this.PacketType = p.PacketType;



        }

        public byte[] ToBytes() {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            byte[] bytes = ms.ToArray();
            ms.Close();
            return bytes;



        }

        public static string GetIP4Address()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress item in ips)
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork )
                {
                    return item.ToString();
                }
                
            }
            return "127.0.0.1";
        }

    }

    public enum PacketType {
        Registration,
        Chat
    }
}
