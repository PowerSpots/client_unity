using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public abstract class PlayerCommandBase : ComponentBase
    {
        public int id;
        public int frame;
        public int time;

        public abstract PlayerCommandBase DeepCopy();

        public abstract bool EqualsCmd(PlayerCommandBase cmd);
    }
}
