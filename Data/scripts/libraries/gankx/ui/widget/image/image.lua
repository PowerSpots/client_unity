local UIImageExport = Gankx.UI.ExportWrapper(CS.UIImageExport)

widget("Image", "Window")

function setAtlasPath(self, path)
    UIImageExport:setString(self, "AtlasPath", path or "")
end

function setPath(self, path)
    UIImageExport:setString(self, "Path", path or "")
end

function setPathAsync(self, path)
    UIImageExport:setString(self, "PathAsync", path or "")
end

function setFillAmount(self, fillAmount)
    UIImageExport:setNumber(self, "FillAmount", fillAmount)
end

function getFillAmount(self)
    return UIImageExport:getNumber(self, "FillAmount", 0)
end

function setNativeSize(self)
    UIImageExport.interface.SetNativeSize(self.id)
end

function setGray(self, state)
    UIImageExport:setBool(self, "Gray", state)
end

function setEnabled(self, enabled)
    UIImageExport:setBool(self, "Enabled", enabled)
end

function getImageWidth(self)
    return UIImageExport:getNumber(self, "ImageWidth", 0)
end

function getImageHeight(self)
    return UIImageExport:getNumber(self, "ImageHeight", 0)
end

function setMaterialFloat(self, property, value)
    UIImageExport:setVector(self, "MaterialFloat", property, value)
end