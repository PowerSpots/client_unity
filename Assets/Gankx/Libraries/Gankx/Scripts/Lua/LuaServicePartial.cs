using XLua;

namespace Gankx
{
    internal partial class LuaService
    {
        public void FireEvent(params object[] eventParams)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Call(eventParams);
        }

        public void FireEvent<T1>(T1 arg1)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Action(arg1);
        }

        public void FireEvent<T1, T2>(T1 arg1, T2 arg2)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Action(arg1, arg2);
        }

        public void FireEvent<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Action(arg1, arg2, arg3);
        }

        public void FireEvent<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Action(arg1, arg2, arg3, arg4);
        }

        public void FireEvent<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Action(arg1, arg2, arg3, arg4, arg5);
        }

        public void FireEvent<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Action(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void FireEvent<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public void FireEvent<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            T7 arg7, T8 arg8)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public void FireDataEvent(string eventName, int id, int msgType, DataBoat databoat)
        {
            if (null == myFireEvent)
            {
                return;
            }

            myFireEvent.Action(eventName, id, msgType, databoat);
        }
    }
}