system("MessageBoxSystem", SystemScope.Global)

local MaxMessageCount = 5

function onInit(self)
    if self.messageBoxList ~= nil then
        self.messageBoxList:setMaxCount(MaxMessageCount)
    end
end

function onLogOut(self)
    if self.panel:isLoaded() then
        self:removeAll()
    end
end

function onShow(self)
    fireEvent("OnMessageBoxSystemShow")
end

function onHide(self)
    fireEvent("OnMessageBoxSystemHide")
end

function getCount(self)
    return self.messageBoxList:getCount()
end

function getTopMessageBox(self)
    local count = self:getCount()
    return self.messageBoxList:getCell(count)
end

function show(self, msgText, callBackFunc, callBackContext, uiType, ...)
    local count = self:getCount()
    local newIndex = count + 1
    local cellData = {
        msgText = Util.replaceSpace(msgText),
        param = { ... },
        callBackFunc = callBackFunc,
        callBackContext = callBackContext,
        index = newIndex,
        uiType = uiType,
    }

    if newIndex > MaxMessageCount then
        self:error("show", "MessageBoxSystem " .. newIndex .. " exceed max count '" .. MaxMessageCount .. "'")
        return nil
    end

    self.messageBoxList:add(cellData)

    local cellItem = self.messageBoxList:getCell(newIndex)
    if nil == cellItem then
        return nil
    end

    cellItem:setData(cellData)
    cellItem.Visible = true

    self:_isShowPanel()

    return cellItem
end

function onPopup(self, ...)
    self:show(...)
end

function onExitGameSucceedHandler(self)
    if self.panel:isLoaded() then
        self:hidePanel()
        self:removeAll()
    end
end

function _isShowPanel(self)
    local count = self:getCount()
    if count > 0 then
        self:showPanel()
    else
        self:hidePanel()
    end
end

function onBtnClick(self, sender, data, isOk)
    if nil == data then
        self:error("onBtnClick", "invalid parameter")
        return
    end

    local callBackFunc = data.callBackFunc
    local callBackContext = data.callBackContext

    if nil ~= callBackFunc then
        if callBackContext ~= nil then
            callBackFunc(callBackContext, isOk, data.param)
        else
            callBackFunc(isOk, data.Param)
        end
    end

    self.messageBoxList:remove(sender)
    self:_isShowPanel()
end

function onDestroyWithTime(self, sender)
    self.messageBoxList:remove(sender)
    self:_isShowPanel()
end

function remove(self, messageBox)
    self.messageBoxList:remove(messageBox)
    self:_isShowPanel()
end

function removeAll(self)
    self.messageBoxList:removeAll()
    self:_isShowPanel()
end

function update(self, deltaTime)
    if self.messageBoxList ~= nil then
        for _, cell in pairs(self.messageBoxList:getCellList()) do
            cell:update(deltaTime)
        end
    end
end

panelDefine = {
    file = "common/page_messagebox",

    tags = {
        lazyLoad = true,
        loadSync = true,
    },

    widgets = {
        {
            path = "Windows/Content/MaskBg",
        },
        {
            path = "Windows",
            type = "GridList",
            var = "messageBoxList",
            cacheSetting = {
                type = "MessageBox",
                handles = {
                    ["OnBtnClick"] = "onBtnClick",
                    ["OnDestroyWithTime"] = "onDestroyWithTime",
                }
            }
        },
    },
}

