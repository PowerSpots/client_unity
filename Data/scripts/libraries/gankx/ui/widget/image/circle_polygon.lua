local UICirclePolygonExport = CS.UICirclePolygonExport

widget("CirclePolygon", "Window")

function setBeginFillAmount(self, fillAmount)
    UICirclePolygonExport.SetBeginFillAmount(self.id, fillAmount)
end

function setEndFillAmount(self, fillAmount)
    UICirclePolygonExport.SetEndFillAmount(self.id, fillAmount)
end

function setSegments(self, segments)
    UICirclePolygonExport.SetSegments(self.id, segments)
end

function setSegmentFillAmount(self, index, fillAmount)
    UICirclePolygonExport.SetSegmentFillAmount(self.id, index, fillAmount)
end

function setStartAngle(self, starAngle)
    UICirclePolygonExport.SetStartAngle(self.id, starAngle)
end

function setUVOffset(self, uvOffsetX, uvOffsetY)
    UICirclePolygonExport.SetUVOffset(self.id, uvOffsetX, uvOffsetY)
end

function setVerticesDirty(self)
    UICirclePolygonExport.SetVerticesDirty(self.id)
end


