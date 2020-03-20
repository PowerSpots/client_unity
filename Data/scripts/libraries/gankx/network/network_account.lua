local NetworkExport =  CS.NetworkExport

module("Gankx.NetworkAccount", package.seeall)

function isPlatformInstalled(self, platform)
    return NetworkExport.IsPlatformInstalled(platform)
end

function login(self, platform)
    fireEvent("NetWorkLogin")
    return NetworkExport.Login(platform)
end

-- => onNetGetRecord(platform, openId)
function getRecord(self)
    return NetworkExport.GetRecord()
end

function logout(self)
    NetworkExport.Logout()
end

function getAccountId(self)
    return NetworkExport.GetAccountId()
end
