using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ControlValley
{
    public class CrowdResponse
    {
        public enum Status
        {
            STATUS_SUCCESS,
            STATUS_FAILURE,
            STATUS_UNAVAIL,
            STATUS_RETRY,
            STATUS_KEEPALIVE=255
        }

        public int id;
        public string message;
        public int status;

        public CrowdResponse(int id, Status status = Status.STATUS_SUCCESS, string message = "")
        {
            this.id = id;
            this.message = message;
            this.status = (int)status;
        }

        public static void KeepAlive(Socket socket)
        {
            new CrowdResponse(0, Status.STATUS_KEEPALIVE).Send(socket);
        }

        public void Send(Socket socket)
        {
            byte[] tmpData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this));
            byte[] outData = new byte[tmpData.Length + 1];
            Buffer.BlockCopy(tmpData, 0, outData, 0, tmpData.Length);
            outData[tmpData.Length] = 0;
            socket.Send(outData);
        }
    }
}
