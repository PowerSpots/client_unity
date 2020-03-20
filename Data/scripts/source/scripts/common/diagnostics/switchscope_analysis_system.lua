local PerformanceCounterExport = CS.PerformanceCounterExport
local SwitchScopeAnalysisService = SwitchScopeAnalysisService
local File = Gankx.File

system("SwitchScopeAnalysisSystem", SystemScope.Global)

function onInit(self)

end

function showAnalysis(self)
    self:showPanel()
end

function onExitButtonClick(self)
    self:hidePanel()
end

local function minorFilter(node)
    if node.time > 0.1 then
        return true
    end

    return false
end

function onHideMinorClick(self)
    self.timeTreeText.Text = SwitchScopeAnalysisService.instance:getTimeTreeStr(minorFilter)
end

function onShowAllClick(self)
    self.timeTreeText.Text = SwitchScopeAnalysisService.instance:getTimeTreeStr()
end

function onShow(self)
    self.timeTreeText.Text = SwitchScopeAnalysisService.instance:getTimeTreeStr()
end

function onSaveButtonClick(self)
    File.writeAppFile("switch_scope_analysis.txt", self.timeTreeText.Text)
end

function startCollect(self)
    self:setDetailRecordState(true)
end

function stopCollect(self)
    self:setDetailRecordState(false)
end

function getCollectState(self)
    return self ~= nil and self._recordDetailInfo or false
end

function setDetailRecordState(self, state)
    self._recordDetailInfo = state
end

function beginAccumulate(self, name)
    if not self._recordDetailInfo then
        return
    end
    PerformanceCounterExport.BeginAccumulate(string.format("%s", name))
end

function endAccumulate(self, name)
    if not self._recordDetailInfo then
        return
    end
    PerformanceCounterExport.EndAccumulate(string.format("%s", name))
end

panelDefine = {
    file = "common/page_switchscope_analysis",

    tags = {
        lazyLoad = true,
    },

    widgets = {
        {
            name = "ScrollView/Viewport/Text",
            type = "Text",
            var = "timeTreeText",
        },

        {
            name = "HideMinor",
            type = "Button",
            handles = {
                ["OnClick"] = "onHideMinorClick",
            }
        },

        {
            name = "ShowAll",
            type = "Button",
            handles = {
                ["OnClick"] = "onShowAllClick",
            }
        },

        {
            name = "ExitButton",
            type = "Button",
            handles = {
                ["OnClick"] = "onExitButtonClick",
            }
        },

        {
            name = "SaveButton",
            type = "Button",
            handles = {
                ["OnClick"] = "onSaveButtonClick",
            }
        }
    }
}