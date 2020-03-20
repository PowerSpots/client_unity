local UIPanelServiceExport = CS.UIPanelServiceExport

module("Gankx.UI.Screen", package.seeall)

function getHeight(self)
    return UIPanelServiceExport.GetScreenHeight()
end

function getWidth(self)
    return UIPanelServiceExport.GetScreenWidth()
end

function getCanvasHeight(self)
    return UIPanelServiceExport.GetCanvasHeight()
end

function getCanvasWidth(self)
    return UIPanelServiceExport.GetCanvasWidth()
end

function getSafeWidth(self)
    return UIPanelServiceExport.GetScreenSafeWidth()
end

function getCanvasSafeWidth(self)
    return UIPanelServiceExport.GetCanvasSafeWidth()
end
