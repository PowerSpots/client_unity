using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public abstract class MomentSingletonComponent : SingletonComponent
    {
        public int Frame { get; set; }

        public bool IsChange { get; set; }

        public abstract MomentSingletonComponent DeepCopy();
    }
}
