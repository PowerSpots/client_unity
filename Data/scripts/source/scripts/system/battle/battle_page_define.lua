system("BattleSystem", classPartial)

panelDefine = {
    file = "page_battle",
    tags = {
        lazyLoad = false,
        unloadOnHide = false,
    },
    widgets = {
        -- {
        --     path = "Windows/BattleRoot",
        --     type = "Window",
        --     var = "BattleRoot",
        -- },
        {
            path = "Windows/ButtonBattle",
            type = "Button",
            var = "ButtonBattle",
            handles = {
                ["OnClick"] = "onButtonBattle",
            }
        },
        {
            path = "Windows/ButtonReturnHall",
            type = "Button",
            var = "ButtonReturnHall",
            handles = {
                ["OnClick"] = "onButtonReturnHall",
            }
        },
    },
}