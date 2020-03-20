local UIImageExport = Gankx.UI.ExportWrapper(CS.UIImageExport)

widget("CircleImage", "Image")

function setBeginFillAmount(self, fillAmount)
    UIImageExport:setNumber(self, "CircleImageBeginFillAmount", fillAmount)
end

function setEndFillAmount(self, fillAmount)
    UIImageExport:setNumber(self, "CircleImageEndFillAmount", fillAmount)
end

function setSegements(self, segements)
    UIImageExport:setNumber(self, "CircleImageSegements", segements)
end

function setStartAngle(self, starAngle)
    UIImageExport:setNumber(self, "CircleImageStartAngle", starAngle)
end

function setUVOffset(self, uvOffsetX, uvOffsetY)
    UIImageExport.interface.setUVOffset(self.id, uvOffsetX, uvOffsetY)
end

function setVerticesDirty(self)
    UIImageExport.interface.setVerticesDirty(self.id)
end



