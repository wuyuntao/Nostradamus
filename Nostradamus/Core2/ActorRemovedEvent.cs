using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostradamus.Core2
{
    class ActorRemovedEvent : IEventArgs
    {
        public ActorId ActorId;
    }
}
