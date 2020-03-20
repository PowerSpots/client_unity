module("Array", package.seeall)

function add(array, item)
    if nil == array or nil == item then
        return 0
    end

    local insertIndex = #array + 1
    array[insertIndex] = item
    return insertIndex
end

function insertAdd(array, item, compare,...)

    local j = add(array,item)
    local temp = array[j]
    while j > 1 and compare(temp, array[j - 1], ...) do
        array[j] = array[j-1]
        j = j - 1
    end
    array[j] = temp
end

function contains(array, item, match)
    if nil == array or nil == item then
        return false
    end

    for i, v in ipairs(array) do
        if v == item or (match and match(v,item)) then
            return true
        end
    end

    return false
end

function clear(array)
    if nil == array then
        return
    end

    for i, v in ipairs(array) do
        array[i] = nil
    end
end

function indexOf(array, item, match)
    if nil == array or nil == item then
        return 0
    end

    for i, v in ipairs(array) do
        if item == v or (match and match(item, v))then
            return i
        end
    end

    return 0
end

function remove(array, item)
    if nil == array or nil == item then
        return
    end

    for i, v in ipairs(array) do
        if v == item then
            table.remove(array, i)
            return
        end
    end
end

function removeAt(array, index)
    if nil == array or nil == index then
        return
    end

    table.remove(array, index)
end

function removeAll(array, match, ...)
    if nil == array then
        return
    end

    local i = 1
    local v
    while true do
        v = array[i]
        if nil == v then
            break
        end

        if nil == match or match(v, ...) then
            table.remove(array, i)
        else
            i = i + 1
        end
    end
end

function foreach(array, action, ...)
    if nil == array or nil == action then
        return
    end

    local i = 1
    local v
    while true do
        v = array[i]
        if nil == v then
            break
        end

        if action(v, ...) == true then
            table.remove(array, i)
        else
            i = i + 1
        end
    end
end

function reverse(array)
    if nil == array then
        return
    end

    local tmp
    local count = #array

    for i = 1, count / 2, 1 do
        tmp = array[i]
        array[i] = array[count - i + 1]
        array[count - i + 1] = tmp
    end
end

function arrayForeach(array, action, ...)
    if nil == array or nil == action then
        return
    end

    for i, v in ipairs(array) do
        if not action(v,...) then
            break
        end
    end
end

function conditionCheck(array, action, ...)
    if nil == array or nil == action then
        return
    end

    for i, v in ipairs(array) do
        if not action(v,...) then
            return false
        end
    end

    return true
end

function bubbleSort(array, cmpFunc)
    local itemCount = #array
    local hasChanged
    repeat
        hasChanged = false
        itemCount = itemCount - 1
        for i = 1, itemCount do
            if cmpFunc(array[i + 1], array[i]) then
                array[i], array[i + 1] = array[i + 1], array[i]
                hasChanged = true
            end
        end
    until hasChanged == false
end

