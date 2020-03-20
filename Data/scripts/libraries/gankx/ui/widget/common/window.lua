local UIWindowExportWrapper = Gankx.UI.ExportWrapper(CS.UIWindowExport)

widget("Window")

function setVisible(window, visible)
    local isChangeVisible = window.Visible ~= visible
    UIWindowExportWrapper:setBool(window, "Visible", visible)
    if visible == true then
        window:checkOnVisible()
    end
    if isChangeVisible then
        fireEvent("OnSetWindowVisible", window)
    end
end

function getVisible(window)
    return UIWindowExportWrapper:getBool(window, "Visible")
end

function setScaleVisible(window, visible)
    local oldScaleVisible = window:getScaleVisible()
    UIWindowExportWrapper:setBool(window, "ScaleVisible", visible)

    if visible ~= oldScaleVisible then
        fireEvent("OnScaleVisibleChanged", window, visible)
    end
end

function getScaleVisible(window)
    return UIWindowExportWrapper:getBool(window, "ScaleVisible")
end

function getVisibleInHierarchy(window)
    return UIWindowExportWrapper.interface.GetVisibleInHierarchy(window.id)
end

function getVisibleAndRendered(window, xScale, yScale)
    return UIWindowExportWrapper.interface.GetVisibleAndRendered(window.id, xScale, yScale)
end

function setWidth(window, width)
    UIWindowExportWrapper:setNumber(window, "Width", width)
end

function getWidth(window)
    return UIWindowExportWrapper:getNumber(window, "Width")
end

function setHeight(window, height)
    UIWindowExportWrapper:setNumber(window, "Height", height)
end

function getHeight(window)
    return UIWindowExportWrapper:getNumber(window, "Height")
end

function getColorR(window)
    return UIWindowExportWrapper:getValue(window, "ColorR", 1)
end

function getColorG(window)
    return UIWindowExportWrapper:getValue(window, "ColorG", 1)
end

function getColorB(window)
    return UIWindowExportWrapper:getValue(window, "ColorB", 1)
end

function setAlpha(window, alpha)
    UIWindowExportWrapper:setNumber(window, "Alpha", alpha)
end

function getAlpha(window)
    return UIWindowExportWrapper:getValue(window, "Alpha", 1)
end

function setPivot(window, pivotx, pivoty)
    UIWindowExportWrapper:setVector(window, "Pivot", pivotx, pivoty)
end
function setPivotX(window, pivotx)
    UIWindowExportWrapper:setNumber(window, "PivotX", pivotx)
end
function setPivotY(window, pivoty)
    UIWindowExportWrapper:setNumber(window, "PivotY", pivoty)
end
function getPivotX(window)
    return UIWindowExportWrapper:getNumber(window, "PivotX", 0.5)
end
function getPivotY(window)
    return UIWindowExportWrapper:getNumber(window, "PivotY", 0.5)
end

function getScreenPosX(window)
    return UIWindowExportWrapper:getNumber(window, "ScreenPosX")
end

function getScreenPosY(window)
    return UIWindowExportWrapper:getNumber(window, "ScreenPosY")
end

function getCanvasPos(window)
    return UIWindowExportWrapper.interface.getCanvasPos(window.id)
end

function setWorldPosX(window, x)
    return UIWindowExportWrapper:setNumber(window, "WorldPosX", x)
end

function getWorldPosX(window)
    return UIWindowExportWrapper:getNumber(window, "WorldPosX")
end

function setWorldPosY(window, y)
    UIWindowExportWrapper:setNumber(window, "WorldPosY", y)
end

function getWorldPosY(window)
    return UIWindowExportWrapper:getNumber(window, "WorldPosY")
end

function setWorldPosZ(window, z)
    UIWindowExportWrapper:setNumber(window, "WorldPosZ", z)
end

function setLocalPosX(window, x)
    UIWindowExportWrapper:setNumber(window, "LocalPosX", x)
end

function getLocalPosX(window)
    return UIWindowExportWrapper:getNumber(window, "LocalPosX")
end

function setLocalPosY(window, y)
    UIWindowExportWrapper:setNumber(window, "LocalPosY", y)
end

function getLocalPosY(window)
    return UIWindowExportWrapper:getNumber(window, "LocalPosY")
end

function setLocalScale(window, x, y, z)
    UIWindowExportWrapper:setVector(window, "LocalScale", x, y, z)
end

function setLocalRotation(window, anglex, angleY, angleZ)
    UIWindowExportWrapper:setVector(window, "LocalRotation", anglex, angleY, angleZ)
end

function setColor(window, red, green, blue, alpha)
    alpha = alpha or 1

    UIWindowExportWrapper:setVector(window, "Color", red, green, blue, alpha)
end

function setColorTable(self, color)
    local red = color.red / 255
    local green = color.green / 255
    local blue = color.blue / 255
    local alpha = (color.alpha or 255) / 255

    UIWindowExportWrapper:setVector(self, "Color", red, green, blue, alpha)
end

function setColorHex(window, hex)
    UIWindowExportWrapper:setVector(window, "Color", ColorDefine.ParseRGBAFromHex(hex))
end

function readdChild(window, childWindow)
    UIWindowExportWrapper.interface.readdChild(window.id, childWindow.id)
end

function insertChild(window, childWindow, refChildWindow)
    UIWindowExportWrapper.interface.insertChild(window.id, childWindow.id, refChildWindow.id)
end

function setSelected(window, selected)
    UIWindowExportWrapper:setBool(window, "Selected", selected)
end

function getSelected(window)
    return UIWindowExportWrapper:getBool(window, "Selected")
end

function setAnchorPosition(self, x, y)
    self:setAnchorPositionX(x)
    self:setAnchorPositionY(y)
end

function getAnchorPosition(self)
    return self:getAnchorPositionX(), self:getAnchorPositionY()
end

function setAnchorPositionX(self, x)
    UIWindowExportWrapper:setNumber(self, "AnchorPositionX", x)
end

function setAnchorPositionY(self, y)
    UIWindowExportWrapper:setNumber(self, "AnchorPositionY", y)
end

function getAnchorPositionX(self)
    return UIWindowExportWrapper:getNumber(self, "AnchorPositionX", 0)
end

function getAnchorPositionY(self)
    return UIWindowExportWrapper:getNumber(self, "AnchorPositionY", 0)
end

function getSiblingIndex(self)
    return UIWindowExportWrapper.interface.getSiblingIndex(self.id)
end

function getTransform(self)
    return UIWindowExportWrapper.interface.getTransform(self.id)
end

function copyTransform(self, windowId, xScale, yScale)
    return UIWindowExportWrapper.interface.copyTransform(self.id, windowId, xScale, yScale)
end

function setAnchor(window, minx, minY, maxX, maxY)
    minx = minx or 0.5
    minY = minY or 0.5
    maxX = maxX or 0.5
    maxY = maxY or 0.5
    UIWindowExportWrapper:setVector(window, "Anchor", minx, minY, maxX, maxY)
end

function getName(self)
    return UIWindowExportWrapper:getString(self, "ObjectName")
end

function setName(self, name)
    UIWindowExportWrapper:setString(self, "ObjectName", name)
end

function getPanelId(self)
    return UIWindowExportWrapper.interface.getPanelId(self.id)
end

function forceRebuildLayoutImmediate(self)
    UIWindowExportWrapper.interface.forceRebuildLayoutImmediate(self.id)
end

function attachPointer(self, data)
    UIWindowExportWrapper.interface.attachPointer(self.id, data)
end

function attachTransform(self, target, camera, mode)
    UIWindowExportWrapper.interface.attachTransform(self.id, target, camera, mode)
end

function setInScreen(self)
    UIWindowExportWrapper.interface.setInScreen(self.id)
end

function setGray(self, state)
    UIWindowExportWrapper:setBool(self, "Gray", state)
end

function setGrayRecursively(self, state)
    UIWindowExportWrapper:setBool(self, "GrayRecursively", state)
end

function setRayCastTargetRecursively(self, state)
    UIWindowExportWrapper.interface.setRayCastTargetRecursively(self.id, state)
end
