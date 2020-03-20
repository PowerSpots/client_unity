using UnityEngine;

namespace Gankx
{
    public abstract class CustomAsyncOperation : CustomYieldInstruction
    {
        public abstract float progress { get; }
        public abstract int priority { get; set; }
        public abstract bool allowSceneActivation { get; set; }

        public virtual bool isDone { get; private set; }

        public override bool keepWaiting
        {
            get { return !isDone; }
        }

        public bool Execute()
        {
            if (!isDone)
            {
                isDone = OnExecute();
            }

            return isDone;
        }

        protected abstract bool OnExecute();
    }

    public class UnityAsyncOperation : CustomAsyncOperation
    {
        private readonly AsyncOperation myOperation;

        public UnityAsyncOperation(AsyncOperation operation)
        {
            myOperation = operation;
        }

        public override bool keepWaiting
        {
            get
            {
                return !Execute();
            }
        }

        public override bool isDone
        {
            get
            {
                return OnExecute();
            }
        }

        public override float progress
        {
            get
            {
                if (null == myOperation)
                {
                    return 0.0f;
                }

                return myOperation.progress;
            }
        }

        public override int priority
        {
            get
            {
                if (null == myOperation)
                {
                    return 0;
                }

                return myOperation.priority;
            }
            set
            {
                if (null == myOperation)
                {
                    return;
                }

                myOperation.priority = value;
            }
        }

        public override bool allowSceneActivation
        {
            get
            {
                if (null == myOperation)
                {
                    return false;
                }

                return myOperation.allowSceneActivation;
            }
            set
            {
                if (null == myOperation)
                {
                    return;
                }

                myOperation.allowSceneActivation = value;
            }
        }

        protected override bool OnExecute()
        {
            return myOperation == null || myOperation.isDone;
        }
    }
}