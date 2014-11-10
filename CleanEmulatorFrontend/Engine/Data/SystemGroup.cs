using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesData.DatData;

namespace CleanEmulatorFrontend.Engine.Data
{
    public class SystemGroup
    {
        public string Name { get; set; }
        public IEnumerable<EmulatedSystem> Systems { get; set; }
    }
}
