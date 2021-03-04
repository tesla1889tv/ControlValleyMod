using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewBoots = StardewValley.Objects.Boots;

namespace ControlValley
{
    public class Boots
    {
        private static readonly List<IBootTier> boots = new List<IBootTier>
        {
            new MultiBootTier() { 504, 505 },
            new MultiBootTier() { 506, 507 },
            new MultiBootTier() { 508, 509, 510, 806 },
            new MultiBootTier() { 511, 512 },
            new MultiBootTier() { 513, 855 },
            new MultiBootTier() { 514, 804, 878 },
            new SingleBootTier(853),
            new SingleBootTier(854)
        };

        public static StardewBoots GetDowngrade(int index)
        {
            for (int i = boots.Count - 1; i > 0; --i)
            {
                if (boots[i].Contains(index))
                    return boots[i - 1].GetBoots();
            }

            return null;
        }

        public static StardewBoots GetUpgrade(int index)
        {
            for (int i = 0; i < boots.Count - 1; ++i)
            {
                if (boots[i].Contains(index))
                    return boots[i + 1].GetBoots();
            }

            return null;
        }

        public interface IBootTier
        {
            bool Contains(int index);
            StardewBoots GetBoots();
        }

        class MultiBootTier : List<int>, IBootTier
        {
            private Random Random { get; set; }

            public MultiBootTier() : base() {
                Random = new Random();
            }

            public StardewBoots GetBoots()
            {
                return new StardewBoots(this[Random.Next(this.Count)]);
            }
        }

        class SingleBootTier : IBootTier
        {
            private readonly int index;

            public SingleBootTier(int index)
            {
                this.index = index;
            }

            public bool Contains(int index)
            {
                return index == this.index;
            }

            public StardewBoots GetBoots()
            {
                return new StardewBoots(index);
            }
        }
    }
}
