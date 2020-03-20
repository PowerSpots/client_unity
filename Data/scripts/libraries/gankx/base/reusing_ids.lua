local ReusingIds = class("ReusingIds")

function ReusingIds:constructor(startIndex)
    self._curId = startIndex or 1

    self._freeIdList = SteadyList:new()
end

function ReusingIds:create()
    if self._freeIdList:length() > 0 then
        return self._freeIdList:pop()
    end

    self._curId = self._curId + 1
    return self._curId
end

function ReusingIds:release(id)
    self._freeIdList:push(id)
end

return ReusingIds

