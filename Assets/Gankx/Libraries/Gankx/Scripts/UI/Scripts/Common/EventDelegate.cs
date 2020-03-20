#if UNITY_EDITOR || !UNITY_FLASH
#define REFLECTION_SUPPORT
#endif


using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if REFLECTION_SUPPORT
using System.Reflection;
#endif


namespace Gankx.UI
{
    [Serializable]
    public class EventDelegate
    {
        [Serializable]
        public class Parameter
        {
            public Object obj;
            public string field;

            public Parameter()
            {
            }

            public Parameter(Object obj, string field)
            {
                this.obj = obj;
                this.field = field;
            }

            public Parameter(object val)
            {
                myValue = val;
            }

            [NonSerialized]
            private object myValue;

#if REFLECTION_SUPPORT
            [NonSerialized]
            public Type expectedType = typeof(void);

            [NonSerialized]
            public bool cached;

            [NonSerialized]
            public PropertyInfo propInfo;

            [NonSerialized]
            public FieldInfo fieldInfo;


            public object value
            {
                get
                {
                    if (myValue != null)
                    {
                        return myValue;
                    }

                    if (!cached)
                    {
                        cached = true;
                        fieldInfo = null;
                        propInfo = null;

                        if (obj != null && !string.IsNullOrEmpty(field))
                        {
                            var objType = obj.GetType();

                            propInfo = objType.GetProperty(field);
                            if (propInfo == null)
                            {
                                fieldInfo = objType.GetField(field);
                            }
                        }
                    }

                    if (propInfo != null)
                    {
                        return propInfo.GetValue(obj, null);
                    }

                    if (fieldInfo != null)
                    {
                        return fieldInfo.GetValue(obj);
                    }

                    if (obj != null)
                    {
                        return obj;
                    }

                    if (expectedType != null && expectedType.IsValueType)
                    {
                        return null;
                    }

                    return Convert.ChangeType(null, expectedType);
                }
                set { myValue = value; }
            }

            public Type type
            {
                get
                {
                    if (myValue != null)
                    {
                        return myValue.GetType();
                    }

                    if (obj == null)
                    {
                        return typeof(void);
                    }

                    return obj.GetType();
                }
            }
#else // REFLECTION_SUPPORT
            public object value
            {
                get
                {
                    if (myValue != null)
                    {
                        return myValue;
                    }

                    return obj;
                }
            }
#if UNITY_EDITOR || !UNITY_FLASH
            public Type type
            {
                get
                {
                    if (myValue != null)
                    {
                        return myValue.GetType();
                    }

                    return typeof(void);
                }
            }
#else
            public Type type
            {
                get
                {
                    if (myValue != null)
                    {
                        return myValue.GetType();
                    }

                    return null;
                }
            }
#endif
#endif
        }

        [SerializeField]
        private MonoBehaviour myTarget;

        [SerializeField]
        private string myMethodName;

        [SerializeField]
        private Parameter[] myParameters;

        public bool oneShot;

        public delegate void Callback();

        [NonSerialized]
        private Callback myCachedCallback;

        [NonSerialized]
        private bool myRawDelegate;

        [NonSerialized]
        private bool myCached;
#if REFLECTION_SUPPORT
        [NonSerialized]
        private MethodInfo myMethod;

        [NonSerialized]
        private ParameterInfo[] myParameterInfos;

        [NonSerialized]
        private object[] myArgs;
#endif

        public MonoBehaviour target
        {
            get { return myTarget; }
            set
            {
                myTarget = value;
                myCachedCallback = null;
                myRawDelegate = false;
                myCached = false;
#if REFLECTION_SUPPORT
                myMethod = null;
                myParameterInfos = null;
#endif
                myParameters = null;
            }
        }

        public string methodName
        {
            get { return myMethodName; }
            set
            {
                myMethodName = value;
                myCachedCallback = null;
                myRawDelegate = false;
                myCached = false;
#if REFLECTION_SUPPORT
                myMethod = null;
                myParameterInfos = null;
#endif
                myParameters = null;
            }
        }

        public Parameter[] parameters
        {
            get
            {
#if UNITY_EDITOR
                if (!myCached || !Application.isPlaying)
                {
                    Cache();
                }
#else
                if (!myCached)
                {
                    Cache();
                }
#endif
                return myParameters;
            }
        }

        public bool isValid
        {
            get
            {
#if UNITY_EDITOR
                if (!myCached || !Application.isPlaying)
                {
                    Cache();
                }
#else
                if (!myCached)
                {
                    Cache();
                }
#endif
                return myRawDelegate && myCachedCallback != null ||
                       myTarget != null && !string.IsNullOrEmpty(myMethodName);
            }
        }


        public bool isEnabled
        {
            get
            {
#if UNITY_EDITOR
                if (!myCached || !Application.isPlaying)
                {
                    Cache();
                }
#else
                if (!myCached)
                {
                    Cache();
                }
#endif
                if (myRawDelegate && myCachedCallback != null)
                {
                    return true;
                }

                if (myTarget == null)
                {
                    return false;
                }

                var mb = myTarget;
                return mb == null || mb.enabled;
            }
        }

        public EventDelegate()
        {
        }

        public EventDelegate(Callback call)
        {
            Set(call);
        }

        public EventDelegate(MonoBehaviour target, string methodName)
        {
            Set(target, methodName);
        }

        /// <summary>
        ///     GetMethodName is not supported on some platforms.
        /// </summary>
#if REFLECTION_SUPPORT
        private static string GetMethodName(Callback callback)
        {
            return callback.Method.Name;
        }

        private static bool IsValid(Callback callback)
        {
            return callback != null && callback.Method != null;
        }
#else
        private static bool IsValid(Callback callback)
        {
            return callback != null;
        }
#endif

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return !isValid;
            }

            var callback = obj as Callback;
            if (callback != null)
            {
#if REFLECTION_SUPPORT
                if (callback.Equals(myCachedCallback))
                {
                    return true;
                }

                var mb = callback.Target as MonoBehaviour;
                return myTarget == mb && string.Equals(myMethodName, GetMethodName(callback));
#elif UNITY_FLASH
                return callback == myCachedCallback;
#else
                return callback.Equals(myCachedCallback);
#endif
            }

            var del = obj as EventDelegate;
            if (del != null)
            {
                return myTarget == del.myTarget && string.Equals(myMethodName, del.myMethodName);
            }

            return false;
        }

        private static readonly int HashCode = "EventDelegate".GetHashCode();

        public override int GetHashCode()
        {
            return HashCode;
        }

        private void Set(Callback call)
        {
            Clear();

            if (call != null && IsValid(call))
            {
#if REFLECTION_SUPPORT
                myTarget = call.Target as MonoBehaviour;

                if (myTarget == null)
                {
                    myRawDelegate = true;
                    myCachedCallback = call;
                    myMethodName = null;
                }
                else
                {
                    myMethodName = GetMethodName(call);
                    myRawDelegate = false;
                }
#else
                myRawDelegate = true;
                myCachedCallback = call;
#endif
            }
        }

        public void Set(MonoBehaviour targetValue, string methodNameValue)
        {
            Clear();
            myTarget = targetValue;
            myMethodName = methodNameValue;
        }

        private void Cache()
        {
            myCached = true;
            if (myRawDelegate)
            {
            }

#if REFLECTION_SUPPORT
            if (myCachedCallback == null || myCachedCallback.Target as MonoBehaviour != myTarget ||
                GetMethodName(myCachedCallback) != myMethodName)
            {
                if (myTarget != null && !string.IsNullOrEmpty(myMethodName))
                {
                    var type = myTarget.GetType();
#if NETFX_CORE
				try
				{
					IEnumerable<MethodInfo> methods = type.GetRuntimeMethods();

					foreach (MethodInfo mi in methods)
					{
						if (mi.Name == mMethodName)
						{
							mMethod = mi;
							break;
						}
					}
				}
				catch (System.Exception ex)
				{
					Debug.LogError("Failed to bind " + type + "." + mMethodName + "\n" +  ex.Message);
					return;
				}
#else // NETFX_CORE
                    for (myMethod = null; type != null;)
                    {
                        try
                        {
                            myMethod = type.GetMethod(myMethodName,
                                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                            if (myMethod != null)
                            {
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
#if UNITY_WP8 || UNITY_WP_8_1
					// For some odd reason Type.GetMethod(name, bindingFlags) doesn't seem to work on WP8...
					try
					{
						mMethod = type.GetMethod(mMethodName);
						if (mMethod != null) break;
					}
					catch (System.Exception) { }
#endif
                        type = type.BaseType;
                    }
#endif // NETFX_CORE

                    if (myMethod == null)
                    {
                        Debug.LogError("Could not find method '" + myMethodName + "' on " + myTarget.GetType(),
                            myTarget);
                        return;
                    }

                    if (myMethod.ReturnType != typeof(void))
                    {
                        Debug.LogError(myTarget.GetType() + "." + myMethodName + " must have a 'void' return type.",
                            myTarget);
                        return;
                    }

                    // Get the list of expected parameters
                    myParameterInfos = myMethod.GetParameters();

                    if (myParameterInfos.Length == 0)
                    {
                        // No parameters means we can create a simple delegate for it, optimizing the call
#if NETFX_CORE
					mCachedCallback = (Callback)mMethod.CreateDelegate(typeof(Callback), mTarget);
#else
                        myCachedCallback = (Callback) Delegate.CreateDelegate(typeof(Callback), myTarget, myMethodName);
#endif

                        myArgs = null;
                        myParameters = null;
                        return;
                    }

                    myCachedCallback = null;

                    // Allocate the initial list of parameters
                    if (myParameters == null || myParameters.Length != myParameterInfos.Length)
                    {
                        myParameters = new Parameter[myParameterInfos.Length];
                        for (int i = 0, imax = myParameters.Length; i < imax; ++i)
                        {
                            myParameters[i] = new Parameter();
                        }
                    }

                    // Save the parameter type
                    for (int i = 0, imax = myParameters.Length; i < imax; ++i)
                    {
                        myParameters[i].expectedType = myParameterInfos[i].ParameterType;
                    }
                }
            }
#endif // REFLECTION_SUPPORT
        }

        public bool Execute()
        {
#if !REFLECTION_SUPPORT
            if (isValid)
            {
                if (myRawDelegate)
                {
                    myCachedCallback();
                }
                else
                {
                    myTarget.SendMessage(myMethodName, SendMessageOptions.DontRequireReceiver);
                }

                return true;
            }
#else
#if UNITY_EDITOR
            if (!myCached || !Application.isPlaying)
            {
                Cache();
            }
#else
            if (!myCached)
            {
                Cache();
            }
#endif
            if (myCachedCallback != null)
            {
#if !UNITY_EDITOR
                myCachedCallback();
#else
                if (Application.isPlaying)
                {
                    myCachedCallback();
                }
                else if (myCachedCallback.Target != null)
                {
                    var type = myCachedCallback.Target.GetType();
                    var objs = type.GetCustomAttributes(typeof(ExecuteInEditMode), true);

                    if (objs != null && objs.Length > 0)
                    {
                        myCachedCallback();
                    }
                }
#endif
                return true;
            }

            if (myMethod != null)
            {
#if UNITY_EDITOR
                // There must be an [ExecuteInEditMode] flag on the script for us to call the function at edit time
                if (myTarget != null && !Application.isPlaying)
                {
                    var type = myTarget.GetType();
                    var objs = type.GetCustomAttributes(typeof(ExecuteInEditMode), true);
                    if (objs == null || objs.Length == 0)
                    {
                        return true;
                    }
                }
#endif
                var len = myParameters != null ? myParameters.Length : 0;

                if (len == 0)
                {
                    myMethod.Invoke(myTarget, null);
                }
                else
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (myArgs == null || myArgs.Length != myParameters.Length)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        myArgs = new object[myParameters.Length];
                    }

                    for (int i = 0, imax = myParameters.Length; i < imax; ++i)
                    {
                        myArgs[i] = myParameters[i].value;
                    }

                    try
                    {
                        myMethod.Invoke(myTarget, myArgs);
                    }
                    catch (ArgumentException ex)
                    {
                        var msg = "Error calling ";

                        if (myTarget == null)
                        {
                            msg += myMethod.Name;
                        }
                        else
                        {
                            msg += myTarget.GetType() + "." + myMethod.Name;
                        }

                        msg += ": " + ex.Message;
                        msg += "\n  Expected: ";

                        if (myParameterInfos.Length == 0)
                        {
                            msg += "no arguments";
                        }
                        else
                        {
                            msg += myParameterInfos[0];
                            for (var i = 1; i < myParameterInfos.Length; ++i)
                            {
                                msg += ", " + myParameterInfos[i].ParameterType;
                            }
                        }

                        msg += "\n  Received: ";

                        if (myParameters.Length == 0)
                        {
                            msg += "no arguments";
                        }
                        else
                        {
                            msg += myParameters[0].type;
                            for (var i = 1; i < myParameters.Length; ++i)
                            {
                                msg += ", " + myParameters[i].type;
                            }
                        }

                        msg += "\n";
                        Debug.LogError(msg);
                    }

                    // Clear the parameters so that references are not kept
                    for (int i = 0, imax = myArgs.Length; i < imax; ++i)
                    {
                        if (myParameterInfos[i].IsIn || myParameterInfos[i].IsOut)
                        {
                            myParameters[i].value = myArgs[i];
                        }

                        myArgs[i] = null;
                    }
                }

                return true;
            }
#endif
            return false;
        }

        public void Clear()
        {
            myTarget = null;
            myMethodName = null;
            myRawDelegate = false;
            myCachedCallback = null;
            myParameters = null;
            myCached = false;
#if REFLECTION_SUPPORT
            myMethod = null;
            myParameterInfos = null;
            myArgs = null;
#endif
        }

        public override string ToString()
        {
            if (myTarget != null)
            {
                var typeName = myTarget.GetType().ToString();
                var period = typeName.LastIndexOf('.');
                if (period > 0)
                {
                    typeName = typeName.Substring(period + 1);
                }

                if (!string.IsNullOrEmpty(methodName))
                {
                    return typeName + "/" + methodName;
                }

                return typeName + "/[delegate]";
            }

            return myRawDelegate ? "[delegate]" : null;
        }

        public static void Execute(List<EventDelegate> list)
        {
            if (list != null)
            {
                for (var i = 0; i < list.Count;)
                {
                    var del = list[i];

                    if (del != null)
                    {
#if !UNITY_EDITOR && !UNITY_FLASH
                        try
                        {
                            del.Execute();
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null)
                            {
                                Debug.LogError(ex.InnerException.Message);
                            }
                            else
                            {
                                Debug.LogError(ex.Message);
                            }
                        }
#else
                        del.Execute();
#endif

                        if (i >= list.Count)
                        {
                            break;
                        }

                        if (list[i] != del)
                        {
                            continue;
                        }

                        if (del.oneShot)
                        {
                            list.RemoveAt(i);
                            continue;
                        }
                    }

                    ++i;
                }
            }
        }

        public static bool IsValid(List<EventDelegate> list)
        {
            if (list != null)
            {
                for (int i = 0, imax = list.Count; i < imax; ++i)
                {
                    var del = list[i];
                    if (del != null && del.isValid)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static EventDelegate Set(List<EventDelegate> list, Callback callback)
        {
            if (list != null)
            {
                var del = new EventDelegate(callback);
                list.Clear();
                list.Add(del);
                return del;
            }

            return null;
        }

        public static void Set(List<EventDelegate> list, EventDelegate del)
        {
            if (list != null)
            {
                list.Clear();
                list.Add(del);
            }
        }

        public static EventDelegate Add(List<EventDelegate> list, Callback callback, bool oneShot = false)
        {
            if (list != null)
            {
                for (int i = 0, imax = list.Count; i < imax; ++i)
                {
                    var del = list[i];
                    if (del != null && del.Equals(callback))
                    {
                        return del;
                    }
                }

                var ed = new EventDelegate(callback);
                ed.oneShot = oneShot;
                list.Add(ed);
                return ed;
            }

            Debug.LogWarning("Attempting to add a callback to a list that's null");
            return null;
        }


        public static void Add(List<EventDelegate> list, EventDelegate ev)
        {
            Add(list, ev, ev.oneShot);
        }


        public static void Add(List<EventDelegate> list, EventDelegate ev, bool oneShot)
        {
            if (ev.myRawDelegate || ev.target == null || string.IsNullOrEmpty(ev.methodName))
            {
                Add(list, ev.myCachedCallback, oneShot);
            }
            else if (list != null)
            {
                for (int i = 0, imax = list.Count; i < imax; ++i)
                {
                    var del = list[i];
                    if (del != null && del.Equals(ev))
                    {
                        return;
                    }
                }

                var copy = new EventDelegate(ev.target, ev.methodName);
                copy.oneShot = oneShot;

                if (ev.myParameters != null && ev.myParameters.Length > 0)
                {
                    copy.myParameters = new Parameter[ev.myParameters.Length];
                    for (var i = 0; i < ev.myParameters.Length; ++i)
                    {
                        copy.myParameters[i] = ev.myParameters[i];
                    }
                }

                list.Add(copy);
            }
            else
            {
                Debug.LogWarning("Attempting to add a callback to a list that's null");
            }
        }

        public static bool Remove(List<EventDelegate> list, Callback callback)
        {
            if (list != null)
            {
                for (int i = 0, imax = list.Count; i < imax; ++i)
                {
                    var del = list[i];

                    if (del != null && del.Equals(callback))
                    {
                        list.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool Remove(List<EventDelegate> list, EventDelegate ev)
        {
            if (list != null)
            {
                for (int i = 0, imax = list.Count; i < imax; ++i)
                {
                    var del = list[i];

                    if (del != null && del.Equals(ev))
                    {
                        list.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}