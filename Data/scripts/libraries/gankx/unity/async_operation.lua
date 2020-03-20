local CustomAsyncOperationExport = CS.CustomAsyncOperationExport

module("UnityEngine", package.seeall)

class("AsyncOperation", "Gankx.AsyncOperation")

INVALID_ID = CustomAsyncOperationExport.INVALID_ID

function constructor(self, id)
    __super.constructor(self)

    self._id = id
end

function getIsDone(self)
    self._isDone = CustomAsyncOperationExport.GetIsDone(self._id)
    return self._isDone
end

function getProgress(self)
    self._progress = CustomAsyncOperationExport.GetProgress(self._id)
    return self._progress
end

function onResume(self)
    CustomAsyncOperationExport.Cancel(self._id)
end
