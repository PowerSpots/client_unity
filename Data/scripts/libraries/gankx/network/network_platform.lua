module("NetworkPlatform")

None = 0
Wechat = 1
QQ = 2
WTLogin = 3
Guest = 5
AutoLogin = 6
QR = 0x100
QRWechat = QR + 1
QRQQ = QRWechat + 1


local platformDesc =
{
    [None] = "None",
    [Wechat] = "Wechat",
    [QQ] = "QQ",
    [WTLogin] = "WTLogin",
    [Guest] = "Guest",
    [AutoLogin] = "AutoLogin",
    [QR] = "QR",
    [QRWechat] = "QRWechat",
    [QRQQ] = "QRQQ",
}

function toString(platform)
    if nil == platform then
        return "unknown"
    end

    return platformDesc[scope]
end
