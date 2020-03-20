using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RTS
{
    public class ComponentTypeBase
    {
        public virtual int Count()
        {
            return 0;
        }

        public virtual int GetComponentIndex(string name)
        {
            return -1;
        }
    }

}