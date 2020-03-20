local SceneManagerExport = CS.SceneManagerExport
local AsyncOperation = UnityEngine.AsyncOperation
local LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode

module("UnityEngine.SceneManagement.SceneManager", package.seeall)


local unloadSceneOperation = {}

function loadSceneAsync(sceneName, mode)
    local asyncId = AsyncOperation.INVALID_ID
    mode = mode or LoadSceneMode.Single

    if type(sceneName) == "string" then
        asyncId = SceneManagerExport.LoadSceneAsyncByName(sceneName, mode)
    elseif type(sceneName) == "number" then
        asyncId = SceneManagerExport.LoadSceneAsyncByIndex(sceneName, mode)
    end

    if asyncId == AsyncOperation.INVALID_ID then
        return nil
    end

    local asyncOperation = AsyncOperation:new(asyncId)

    return asyncOperation
end

function unloadSceneAssetbundle(sceneName)
    if type(sceneName) == "string" then
        asyncId = SceneManagerExport.UnloadSceneAssetbundleByName(sceneName)
    elseif type(sceneName) == "number" then
        asyncId = SceneManagerExport.UnloadSceneAssetbundleIndex(sceneName)
    end
end

function unLoadSceneAsync(sceneName)
    local asyncId = AsyncOperation.INVALID_ID

    if type(sceneName) == "string" then
        asyncId = SceneManagerExport.UnloadSceneAsyncByName(sceneName)
    elseif type(sceneName) == "number" then
        asyncId = SceneManagerExport.UnloadSceneAsyncByIndex(sceneName)
    end

    if asyncId == AsyncOperation.INVALID_ID then
        return nil
    end

    local asyncOperation = AsyncOperation:new(asyncId)

    table.insert(unloadSceneOperation, sceneName)

    return asyncOperation
end

function setSceneGOActiveByNameAndActivatePreScene(sceneName,isActive)
    SceneManagerExport.SetSceneGOActiveByNameAndActivatePreScene(sceneName, isActive)
end

function onSceneUnloaded(sceneName)
    for i = #unloadSceneOperation,1,-1 do
        if unloadSceneOperation[i]==sceneName then
            table.remove(unloadSceneOperation,i)
        end
    end
end

function hasUnloadOp()
    return #unloadSceneOperation >0
end

function loadScene(sceneName, mode)
    mode = mode or LoadSceneMode.Single

    if type(sceneName) == "string" then
        SceneManagerExport.LoadSceneByName(sceneName, mode)
    elseif type(sceneName) == "number" then
        SceneManagerExport.LoadSceneByIndex(sceneName, mode)
    end
end

function unloadScene(sceneName)
    if type(sceneName) == "string" then
        return SceneManagerExport.UnloadSceneByName(sceneName)
    elseif type(sceneName) == "number" then
        return SceneManagerExport.UnloadSceneByIndex(sceneName)
    end
end

function getActiveSceneName()
    return SceneManagerExport.GetActiveSceneName()
end

function setActiveScene(sceneName)
    SceneManagerExport.SetActiveScene(sceneName)
end

function isLoadedScene(sceneName)
    return SceneManagerExport.IsLoadedScene(sceneName)
end


