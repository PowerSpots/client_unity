system("HallPage", classPartial)

panelDefine = {
    file = "page_hall",
    tags = {
        lazyLoad = false,
        unloadOnHide = false,
    },
    widgets = {
        {
            path = "Windows/ButtonBattle",
            type = "Button",
            var = "ButtonBattle",
            handles = {
                ["OnClick"] = "onButtonBattle",
            }
        },
    },
}