module("Gankx.Config", package.seeall)

Network = {
    reconnect = {
        { reconnectMaxTimes = 5, waitingTimeout = 1, connectingTimeout = 2, },
        { reconnectMaxTimes = 3, waitingTimeout = 1, connectingTimeout = 4, },
        { reconnectMaxTimes = 2, waitingTimeout = 1, connectingTimeout = 7, },
    },
}