system("LoginPage", classPartial)

panelDefine = {
    file = "page_tutorial_home",
    tags = {
        lazyLoad = false,
        unloadOnHide = false,
    },
    widgets = {
        {
            path = "Windows/ButtonLogin",
            type = "Button",
            var = "BtnLogin",
            handles = {
                ["OnClick"] = "onBtnLoginClick",
            }
        },
    },
}