system("GameSystem", SystemScope.Game)

function onInit()
    Console.info("GameSystem onInit")
    navigateTo(HallPage)
    setNavigateHome(HallPage)
end