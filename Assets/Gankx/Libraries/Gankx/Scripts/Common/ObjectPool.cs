using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gankx
{
    internal static class ListPool<T>
    {
        private static readonly ObjectPool<List<T>> MyListPool = new ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return MyListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            MyListPool.Release(toRelease);
        }
    }

    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> myStack = new Stack<T>();
        private readonly UnityAction<T> myActionOnGet;
        private readonly UnityAction<T> myActionOnRelease;

        public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            myActionOnGet = actionOnGet;
            myActionOnRelease = actionOnRelease;
        }

        public int countAll { get; private set; }

        public int countActive
        {
            get { return countAll - countInactive; }
        }

        public int countInactive
        {
            get { return myStack.Count; }
        }

        public T Get()
        {
            T element;
            if (myStack.Count == 0)
            {
                element = new T();
                countAll++;
            }
            else
            {
                element = myStack.Pop();
            }

            if (myActionOnGet != null)
            {
                myActionOnGet(element);
            }

            return element;
        }

        public void Release(T element)
        {
            if (myStack.Count > 0 && ReferenceEquals(myStack.Peek(), element))
            {
                Debug.LogError("Trying to release object that is already released to pool.");
            }

            if (myActionOnRelease != null)
            {
                myActionOnRelease(element);
            }

            myStack.Push(element);
        }
    }
}