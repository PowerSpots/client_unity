local UIRawImageExport = Gankx.UI.ExportWrapper(CS.UIRawimageExport)

widget("RawImage", "Window")

function loadLocal(self, path, sync)
    if nil == path or "" == path then
        Console.error("RawImage.loadLocalTexture Error: input is invalid")
        return
    end

    if sync == nil then
        UIRawImageExport.interface.LoadLocalTexture(self.id, path, false)
    else
        UIRawImageExport.interface.LoadLocalTexture(self.id, path, sync)
    end
end

function loadAsset(self, path)
    if nil == path or "" == path then
        Console.error("RawImage.loadAssetTexture Error: input is invalid")
        return
    end

    local index1 = string.find(path, "ui/rawimage")

    if index1 == nil then
        Console.error("RawImage.loadAssetTexture Error :  Texture Path : " .. path .. " need start with ui/rawimage")
        return
    end

    UIRawImageExport.interface.LoadAssetTexture(self.id , path)
end

function setUrl(self, url, defaultAssetPath)
    if url == nil then
        return
    end

    defaultAssetPath = defaultAssetPath or ""
    UIRawImageExport:setVector(self, "Url", url, defaultAssetPath)
end

function clearTexture(self)
    UIRawImageExport.interface.ClearTextureRef(self.id)
end

function destroyTexture(self)
    UIRawImageExport.interface.DestroyTexture(self.id)
end

function setMaterialFloat(self, property, value)
    UIRawImageExport.interface.SetMaterialFloat(self.id, property, value)
end

function setGray(self, state)
    UIRawImageExport.interface.SetGray(self.id, state)
end

function setNativeSize(self)
    UIRawImageExport.interface.SetNativeSize(self.id)
end
