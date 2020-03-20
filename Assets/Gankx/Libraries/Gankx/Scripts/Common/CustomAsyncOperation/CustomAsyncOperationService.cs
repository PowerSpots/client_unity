using System.Collections.Generic;
using UnityEngine;

namespace Gankx
{
    public class CustomAsyncOperationService : Singleton<CustomAsyncOperationService>
    {
        public const uint InvalidId = 0xffffffff;

        private static uint BaseId;

        private Dictionary<uint, CustomAsyncOperation> myOperationMap;

        private List<uint> myOperationList;

        protected override void OnInit()
        {
            myOperationMap = new Dictionary<uint, CustomAsyncOperation>();
            myOperationList = new List<uint>();
        }

        public uint Add(AsyncOperation operation)
        {
            return Add(new UnityAsyncOperation(operation));
        }

        public uint Add(CustomAsyncOperation operation)
        {
            if (null == operation)
            {
                return InvalidId;
            }

            BaseId++;

            myOperationMap[BaseId] = operation;
            myOperationList.Add(BaseId);

            return BaseId;
        }

        public void Remove(uint id)
        {
            myOperationMap.Remove(id);
            myOperationList.Remove(id);
        }

        private void LateUpdate()
        {
            for (int index = myOperationList.Count - 1; index >= 0; --index)
            {
                uint id = myOperationList[index];
                CustomAsyncOperation operation;
                myOperationMap.TryGetValue(id, out operation);

                if (null == operation)
                {
                    myOperationList.RemoveAt(index);
                }
                else if(operation.isDone)
                {
                    myOperationMap.Remove(id);
                    myOperationList.RemoveAt(index);
                }
            }
        }

        public bool GetIsDone(uint id)
        {
            CustomAsyncOperation operation;
            if (myOperationMap.TryGetValue(id, out operation))
            {
                return operation.isDone;
            }

            return true;
        }

        public float GetProgress(uint id)
        {
            CustomAsyncOperation operation;
            if (myOperationMap.TryGetValue(id, out operation))
            {
                return operation.progress;
            }
            
            return 1.0f;
        }

    }
}
