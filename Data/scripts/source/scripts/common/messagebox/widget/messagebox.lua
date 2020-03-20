local ButtonStyle = Gankx.UI.MessageBox.ButtonStyle
local TimeLimitOperation = Gankx.UI.MessageBox.TimeLimitOperation

widget("MessageBox", "Window")

local timeLimitPrefix
local timeLimitText
local lastText = "defaulttxt"

function onInit(self)
    self.context.data = nil
    self.context.style = ButtonStyle.OkCancel
end

function setCellData(self, data)
    self:setData(data)
end

function setData(self, data)
    if nil == data then
        self:__error("setData", "invalid parameter")
        return
    end

    self.context.data = data

    self.children.bg.Visible = true
    self.children.msgText.Visible = true

    if nil ~= data.msgText then
        self:setContent(data.msgText, self.children.msgText)
    end

    self:setStyle(ButtonStyle.OkCancel)
    self:setMaskVisible(true)
    self:setOkLabel("OK")
    self:setCancelLabel("cacel")
    self:setTimeLimit(0)
end

function setContent(self, content, widget)
    if nil == content then
        return
    end

    widget.Text = Util.formatMsgString(content)
end

function setStyle(self, style)
    if nil == style then
        return
    end

    self.context.style = style

    if style == ButtonStyle.Ok then
        self.children.okBtn.Visible = true
        self.children.cancelBtn.Visible = false
    elseif style == ButtonStyle.OkCancel then
        self.children.okBtn.Visible = true
        self.children.cancelBtn.Visible = true
    end
end

function setMaskVisible(self, isVisible)
    self.children.maskBg.Visible = isVisible
end

function setOkLabel(self, desc)
    self.children.okBtn.Text = desc
end

function setCancelLabel(self, desc)
    self.children.cancelBtn.Text = desc
end

function SetOkImage(self, atlasPath)
    if atlasPath ~= nil and atlasPath ~= "" then
        self.children.okBtnImage.AtlasPath = atlasPath
    end
end

function onOkBtnClick(self)
    self:_onClose(true)
end

function onCancelBtnClick(self)
    self:_onClose(false)
end

function _onClose(self, result)
    self:postMessage("OnBtnClick", self.context.data, result)
    self.Visible = false
end

function setTimeLimit(self, timeLimit)
    if timeLimit == 0 or timeLimit == nil then
        self.context._destroyWithTime = false
    else
        self.context._destroyWithTime = true
    end
    self.context.timeLimit = timeLimit
end

function setTimeLimitOperation(self, operation)
    self.context.timeLimitOperation = operation
    if self.context.timeLimitOperation == TimeLimitOperation.Ok then
        if self.context.style == ButtonStyle.Ok then
            timeLimitPrefix = self.children.okBtn.Text
            timeLimitText = self.children.okBtn
        else
            timeLimitPrefix = self.children.okBtn.Text
            timeLimitText = self.children.okBtn
        end
    else
        timeLimitPrefix = self.children.cancelBtn.Text
        timeLimitText = self.children.cancelBtn
    end
    lastText = nil
end

function update(self, deltaTime)
    if self.context._destroyWithTime then
        local txt = string.format("%s(%d)", timeLimitPrefix, math.floor(self.context.timeLimit))
        if lastText ~= txt then
            lastText = txt
            timeLimitText.Text = lastText
        end
        self.context.timeLimit = self.context.timeLimit - deltaTime
        if self.context.timeLimit <= 0 then
            if self.context.timeLimitOperation == TimeLimitOperation.Ok then
                self:_onClose(true)
            elseif self.context.timeLimitOperation == TimeLimitOperation.Ok then
                self:_onClose(false)
            else
                self:postMessage("OnDestroyWithTime", self.context.data)
                self.Visible = false
            end
        end
    end
end

WidgetDefine = {
    widgets = {

        {
            path = "MsgText",
            type = "Text",
            var = "msgText",
        },

        {
            path = "BG",
            type = "Window",
            var = "bg",
        },

        {
            path = "Buttons",
            type = "Window",
            var = "buttons",
        },

        {
            path = "Buttons/Ok",
            type = "Image",
            var = "okBtnImage",
        },

        {
            path = "Buttons/Cancel",
            type = "Image",
            var = "cancelBtnImage",
        },

        {
            path = "Buttons/Ok",
            type = "Button",
            var = "okBtn",
            handles = {
                ["OnClick"] = "onOkBtnClick",
            },
        },

        {
            path = "Buttons/Cancel",
            type = "Button",
            var = "cancelBtn",
            handles = {
                ["OnClick"] = "onCancelBtnClick",
            },
        },

        {
            path = "MaskBg",
            type = "Button",
            var = "maskBg",
        },
    },
}