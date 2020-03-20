local ResourcesExport = CS.ResourcesExport
local AsyncOperation = UnityEngine.AsyncOperation

module("UnityEngine.Resources", package.seeall)

function unloadUnusedAssets()
    local asyncId = ResourcesExport.UnloadUnusedAssets()
    if asyncId == AsyncOperation.INVALID_ID then
        return nil
    end

    local asyncOperation = AsyncOperation:new(asyncId)
    return asyncOperation
end
