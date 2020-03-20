local NetworkExport = CS.NetworkExport
local NetworkState = Gankx.NetworkState

module("Gankx.NetworkUtility", package.seeall)

--- => OnNetStateChanged(state)
function getNetworkState()
    return NetworkExport.GetNetworkState()
end

function getNetworkStateString()
    local state = getNetworkState()
    local strTable =
    {
        [NetworkState.NotReachable] = "",
        [NetworkState.ReachableViaWWAN] = "WWAN",
        [NetworkState.ReachableViaWiFi] = "WIFI",
    }
    return strTable[state]
end
