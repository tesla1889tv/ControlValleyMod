using System.Threading;
using StardewValley;

namespace ControlValley
{
    public class BuffThread
    {
        private readonly Buff buff;
        private readonly int duration;

        public BuffThread(int buff, int duration)
        {
            this.buff = new Buff(buff);
            this.duration = duration;
        }

        public void Run()
        {
            buff.addBuff();
            Thread.Sleep(duration);
            buff.removeBuff();
        }
    }
}
