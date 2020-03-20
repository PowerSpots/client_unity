local Coroutine = Gankx.Coroutine

system("LoadingSystem", SystemScope.Global)

local DurationFactor = 2

function onInit(self)
end

function yieldIncrease(self, from, to)
    local duration = DurationFactor * (to - from)
    self.tweenSlider.Duration = duration
    self.tweenSlider:setFromTo(from, to)
    self.tweenSlider:play(true, true)

    Coroutine.yieldWaitForSeconds(duration)
end

function hide(self)
    self:hidePanel()
end

function start(self, bgPath)
    if self.panel.visible then
        return
    end

    self:showPanel()

    self.simpleLoading.Visible = true

    local index = math.random(1, 5)
    local path = bgPath or "ui/rawimage/loading_0" .. index
    self.bg:loadAsset(path)
    self.tweenSlider:reset(0, 0, 0)
end

function stop(self)
    self:hidePanel()
end

function onHide(self)
    fireEvent("OnLoadingEnd")
    fireEvent("OnLoadingSystemHide")
    self.bg:clearTexture()
end

function increaseTo(self, to)
    self.tweenSlider:setTo(to)
    self.tweenSlider:playFromCurrentValue(true, true)
end

function getAmount(self)
    return self.tweenSlider:getValue()
end

panelDefine = {

    file = "common/page_loading",

    tags = {
        lazyLoad = true,
        loadSync = true,
        noCache = true,
    },

    widgets = {
        {
            path = "LoadingBG/Bar",
            type = "TweenSlider",
            var = "tweenSlider",
        },

        {
            path = "LoadingBG/Bg",
            type = "RawImage",
            var = "bg",
        },

        {
            path = "LoadingBG",
            type = "Window",
            var = "simpleLoading",
        },
    }
}
