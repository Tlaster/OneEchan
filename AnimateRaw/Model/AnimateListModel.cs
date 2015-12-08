using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimateRaw.Model
{
    public class AnimateListModel
    {
        public double ID { get; internal set; }
        public TimeSpan LastUpdate { get; internal set; }
        public string Name { get; internal set; }
    }
}
