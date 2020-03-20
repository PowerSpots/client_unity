local UICanvasGroupExport = CS.UICanvasGroupExport

widget("CanvasGroup","Window")

function setAlpha(self,value)
    UICanvasGroupExport.setAlpha(self.id, value)
end

function getAlpha(self)
    return UICanvasGroupExport.getAlpha(self.id)
end