using System.Collections.Generic;
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
            STATUS_RETRY
        }

        public int id;
        public string message;
        public string status;

        public CrowdResponse(int id, Status status = Status.STATUS_SUCCESS, string message = "")
        {
            this.id = id;
            this.message = message;
            this.status = ((int)status).ToString();
        }

        public void Send(Socket socket)
        {
            byte[] tmpData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this));
            byte[] outData = new byte[tmpData.Length + 1];
            outData[tmpData.Length] = 0;
            socket.Send(outData);
        }
    }
}
