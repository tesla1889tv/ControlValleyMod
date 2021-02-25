using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ControlValley
{
    public class CrowdRequest
    {
        public static readonly int RECV_BUF = 4096;

        public string code;
        public int id;
        public string type;
        public string viewer;

        public static CrowdRequest Recieve(Socket socket)
        {
            byte[] buf = new byte[RECV_BUF];
            string content = "";
            int read;
            do
            {
                read = socket.Receive(buf);
                if (read < 0) return null;

                content += Encoding.ASCII.GetString(buf);
            } while (read == 0 || (read == RECV_BUF && buf[RECV_BUF - 1] != 0));

            return JsonConvert.DeserializeObject<CrowdRequest>(content);
        }

        public enum Type
        {
            REQUEST_TEST,
            REQUEST_START,
            REQUEST_STOP
        }

        public string GetReqCode()
        {
            return this.code;
        }

        public int GetReqID()
        {
            return this.id;
        }

        public Type GetReqType()
        {
            string value = this.type;
            if (value == "1")
                return Type.REQUEST_START;
            else if (value == "2")
                return Type.REQUEST_STOP;
            return Type.REQUEST_TEST;
        }

        public string GetReqViewer()
        {
            return this.viewer;
        }
    }
}
