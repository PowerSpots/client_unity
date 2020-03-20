system("HallSystem", SystemScope.Hall)

function onInit()
    Console.info("HallSystem onInit")
    navigateTo(HallPage)
    setNavigateHome(HallPage)
end

function test_api(self, param1)
    Console.info("HallSystem test_api param1:" .. param1)
end