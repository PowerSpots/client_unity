module("table", package.seeall)

function removeItem(list, item, removeAll)
    local rmCount = 0
    local listCount = #list
    for i = 1, listCount do
        if list[i - rmCount] == item then
            table.remove(list, i - rmCount)
            if removeAll then
                rmCount = rmCount + 1
            else
                break
            end
        end
    end
end

function removeAll(list, match)
    local rmCount = 0
    local listCount = #list
    for i = 1, listCount do
        if match(list[i - rmCount]) then
            table.remove(list, i - rmCount)
            rmCount = rmCount + 1
        end
    end
end

function tryGetValue(t, key, defaultValue)
    if nil == t or nil == key then
        return defaultValue
    end

    if nil == t[key] then
        return defaultValue
    end

    return t[key]
end

function findIndex(list, value)
    local listCount = #list
    for i = 1, listCount do
        if list[i] == value then
            return i
        end
    end

    return -1
end

function filter(list, filter, ...)
    if nil == filter then
        return list
    end

    local result = {}
    for i = 1, #list do
        local item = list[i]
        if filter(item, ...) then
            table.insert(result, item)
        end
    end

    return result
end

function addRange(list, rangeList)
    for i = 1, #rangeList do
        table.insert(list, rangeList[i])
    end
end

function getRandomItem(list)
    local num = math.random(1, #list)
    return list[num]
end

function getRandomIndex(list)
    local randArray = {}
    for i, v in pairs(list) do
        Array.add(randArray, i)
    end
    if #randArray > 0 then
        return randArray[math.random(1, #randArray)]
    end
    return nil
end

function oper(list, item)
    for i = 1, #list do
        if list[i] == item then
            table.remove(list, i)
            return false
        end
    end
    table.insert(list, item)
    return true
end

function getRange(list, fromIndex, endIndex)
    local tarList = {}
    for i = 1, #list do
        if i >= fromIndex and i <= endIndex then
            table.insert(tarList, list[i])
        end
    end
    return tarList
end

function insertSort(t, compare, ...)
    local i
    for i = 2, #t do
        local j = i - 1
        local temp = t[i]
        while j >= 1 and compare(temp, t[j], ...) do
            t[j + 1] = t[j]
            j = j - 1
        end
        t[j + 1] = temp
    end
end


function unique(t)
    local check = {}
    local n = {}
    for _, v in ipairs(t) do
        if not check[v] then
            table.insert(n, v)
            check[v] = true
        end
    end
    return n
end
function rep(item, times)
    local t = {}
    if item == nil then
        return nil
    end
    if type(times) ~= "number" then
        return nil
    end
    if times < 0 then
        return nil
    end
    for i = 1, times do
        t[i] = item
    end
    return t
end

function shuffle(t)
    if not t then
        return
    end

    local length = #t
    for i = 1, length do
        local j = math.random(i, length)
        t[i], t[j] = t[j], t[i]
    end
end

function getMax(t)
    local index = 1
    local max = t[1]
    for i, v in ipairs(t) do
        if v > max then
            max = v
            index = i
        end
    end
    return max,index
end

function reverse(tbl)
    for i = 1, math.floor(#tbl / 2) do
        tbl[i], tbl[#tbl - i + 1] = tbl[#tbl - i + 1], tbl[i]
    end
end

function range(a, b)
    if b == nil then
        b = a
        a = 1
    end
    local ret = {}
    for i = a, b do
        Array.add(ret, i)
    end
    return ret
end

local function returnSelf(...)
    return ...
end

function accumulate(t, init, max, getf)
    local index
    getf = getf or returnSelf
    max = max or math.huge

    for i, v in ipairs(t) do
        local value = getf(v)
        if init + value > max then
            break
        end
        init = init + value
        index = i
    end
    return init, index
end

function make_array(size, val)
    local t = new_sized_table(size, 0)
    for index = 1, size do
        t[index] = val
    end
end

function make_increment_array(iStart, iEnd)
    if type(iStart) ~= "number" or type(iEnd) ~= "number" then
        return nil
    end

    if iEnd < iStart then
        return nil
    end

    local size = iEnd - iStart + 1
    local t = new_sized_table(size, 0)
    for index = 1, size do
        t[index] = iStart + index - 1
    end

    return t
end

function slice(tbl, first, last, step)
    local sliced = {}

    for i = first or 1, last or #tbl, step or 1 do
        sliced[#sliced+1] = tbl[i]
    end

    return sliced
end