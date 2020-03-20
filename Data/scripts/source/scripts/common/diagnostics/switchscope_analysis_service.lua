local PerformanceCounterExport = CS.PerformanceCounterExport

service("SwitchScopeAnalysisService")

function startup(self)
    self:listenEvent("OnApplicationEnterStart", onApplicationEnterStart)
    self:listenEvent("OnApplicationEnterStop", onApplicationEnterStop)

    self._tagTimeMap = {}
end

function startCollect(self)
    self._tagTimeMap = {}
    PerformanceCounterExport.BeginRecord()
end

function _addNode(self, parentNode, splits, time)
    local split = splits[1]
    Array.removeAt(splits, 1)
    local curChildNode
    for _, childNode in ipairs(parentNode.children) do
        if childNode.name == split then
            curChildNode = childNode
            break
        end
    end

    if not curChildNode then
        curChildNode = {
            name = split,
            parent = parentNode,
            time = nil,
            children = {},
        }

        curChildNode.parent = parentNode
        Array.add(parentNode.children, curChildNode)
    end

    if #splits <= 0 then
        curChildNode.time = time
    else
        self:_addNode(curChildNode, splits, time)
    end
end

local function getNodeLayer(node)
    local layer = 1
    while node.parent ~= nil do
        layer = layer + 1
        node = node.Parent
    end

    return layer
end

local layerPrefixMap = {
    [1] = "",
    [2] = "  --",
    [3] = "    ===",
    [4] = "      >>>>",
    [5] = "        +++++",
    [6] = "          ++++++",
    [7] = "            +++++++",
    [8] = "               ========",
}

local reportStr = ""
local function reportTimeTree(self, node, filterFunc)
    local layer = getNodeLayer(node)
    local prefix = layerPrefixMap[layer]
    if (layer ~= 1 and nil == filterFunc) or (layer ~= 1 and filterFunc(node)) then
        local time = node.time or "unknown"
        local percent = "unknown"
        if node.time then
            percent = string.format("%.1f", time * 100 / self._totalTime) .. "%"
        end

        reportStr = reportStr .. "\n" .. string.format("%s %s (%sms / %s)", prefix, node.name, time, percent)
    end

    for _, childNode in ipairs(node.children) do
        reportTimeTree(self, childNode, filterFunc)
    end
end

function getTimeTreeStr(self, filterFunc)
    reportStr = string.format("the process takes total of %d ms", self._totalTime)
    reportTimeTree(self, self._timeTreeRoot, filterFunc)
    return reportStr
end

local function rebuildTimeTree(node)
    for _, childNode in ipairs(node.children) do
        rebuildTimeTree(childNode)
    end

    table.sort(node.children, function(lhs, rhs)
        if lhs.time == rhs.time then
            return lhs.name < rhs.name
        end

        return lhs.time > rhs.time
    end)

    if nil == node.time then
        local time = 0
        for _, childNode in ipairs(node.children) do
            time = time + childNode.time
        end

        node.time = time
    end
end

function stopCollect(self)
    PerformanceCounterExport.EndRecord()
    self._tagTimeMap = PerformanceCounterExport.GetTimeDict()
    self._totalTime = PerformanceCounterExport.GetRecordedDuration()
    self._timeTreeRoot = self._tagTimeMap
end

function onApplicationEnterStart(self, newScope, oldScope)
    self:startCollect()
end

function onApplicationEnterStop(self, newScope, oldScope)
    self:stopCollect()
end

