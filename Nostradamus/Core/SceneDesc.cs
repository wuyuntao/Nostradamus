using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostradamus
{
    public enum SceneMode : byte
    {
        Client,
        Server,
    }

    public class SceneDesc
    {
        public SceneMode Mode;

        public ClientId ClientId;

        public int SimulationDeltaTime;

        public int ReconciliationDeltaTime;

        public virtual void Validate()
        {
        }
    }
}
