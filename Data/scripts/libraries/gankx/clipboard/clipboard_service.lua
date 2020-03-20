module("Gankx", package.seeall)

local ClipboardServiceExport = CS.ClipboardExport

service("ClipboardService")

function getClipboard(self)
    return ClipboardServiceExport.GetClipboard()
end

function setClipboard(self, value)
    ClipboardServiceExport.SetClipboard(value)
end

