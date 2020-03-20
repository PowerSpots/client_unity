local Coroutine = Gankx.Coroutine

module("Gankx", package.seeall)

class("CompositedAsyncOperation", "AsyncOperation")

function constructor(self)
    __super.constructor(self)

    self._passedWeight = 0
    self._totalWeight = 0

    self._currentOperation = nil
    self._currentOperationWeight = 0

    self._isGrouping = false
    self._groupWeight = 0
end

function getProgress(self)
    local currentOperation = self._currentOperation
    local progressWeight = 0
    if currentOperation ~= nil then
        progressWeight = currentOperation:getProgress() * self._currentOperationWeight
    end

    local totalWeight = self._totalWeight
    if totalWeight > 0 then
        self._progress = (self._passedWeight + progressWeight) / totalWeight
    else
        self._progress = 1
    end

    return self._progress
end

function setTotalWeight(self, value)
    self._totalWeight = value or 0
end

function getTotalWeight(self)
    return self._totalWeight
end

function beginGroup(self, weight)
    if nil == self then
        return
    end

    if self._isGrouping then
        return
    end

    self._passedWeight = self._passedWeight + self._currentOperationWeight
    self._currentOperationWeight = 0
    self._currentOperation = nil

    weight = weight or 0
    if weight < 0 then
        weight = 0
    end

    self._isGrouping = true
    self._groupWeight = weight
end

function endGroup(self)
    if nil == self then
        return
    end

    if not self._isGrouping then
        return
    end

    self._passedWeight = self._passedWeight + self._currentOperationWeight
    self._currentOperationWeight = 0
    self._currentOperation = nil

    local groupWeight = self._groupWeight
    if groupWeight > 0 then
        self._passedWeight = self._passedWeight + groupWeight
    end

    self._isGrouping = false
    self._groupWeight = 0
end

function yield(self, operation, weight)
    weight = weight or 0
    if weight < 0 then
        weight = 0
    end

    if self._isGrouping then
        local groupWeight = self._groupWeight
        if groupWeight > weight then
            self._groupWeight = groupWeight - weight
        else
            self._groupWeight = 0
            weight = groupWeight
        end
    end

    self._passedWeight = self._passedWeight + self._currentOperationWeight
    self._currentOperationWeight = weight
    self._currentOperation = operation

    if operation and not operation:getIsDone() then
        Coroutine.yieldAsyncOperation(operation)
    end
end

