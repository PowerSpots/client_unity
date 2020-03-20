system("HallPage", SystemScope.Hall)

function onInit(self)
    navigateTo(HallPage)
    setNavigateHome(HallPage)
end

function onButtonBattle(self)
    Console.info("HallPage onButtonBattle Click")
    GalaxyGame.instance:enter(SystemScope.Battle)
end

