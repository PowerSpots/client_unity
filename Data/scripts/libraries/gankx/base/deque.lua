Deque = {}

function Deque.new()

	return
	{
		first = 0,
		last = -1
	}

end

function Deque.clear(DequeObject )

	while true do

		local value = Deque.popFront( DequeObject )
		if value == nil then
			DequeObject.first = 0
			DequeObject.last = -1
			break
		end

	end

end

local function _iterator(d, var)
    local first = d.first + var
    if first > d.last then
        return
    end
    var = var + 1
    return var, d[first]
end

function Deque.getFromFront(DequeObject, index)
    local first  = DequeObject.first + index
    if first > DequeObject.last then
        return
    end
    
    return DequeObject[first]
end

function Deque.isExist(DequeObject, value)
    local count = Deque.getLength(DequeObject)
    local found = false
    for i=0, count-1 do
        if DequeObject[DequeObject.first + i] == value then
            found = true
            return found
        end
    end
    
    return found
end

function Deque.isExistFunc(DequeObject, compare)
    local count = Deque.getLength(DequeObject)
    local found = false
    for i=0, count-1 do
        if compare(DequeObject[DequeObject.first + i]) == true then
            found = true
            return found
        end
    end

    return found
end

function Deque.delValue(DequeObject, value)
    local count = Deque.getLength(DequeObject)
    local index = -1
    for i=0, count-1 do
        if DequeObject[DequeObject.first + i] == value then
            index = i
            break
        end
    end
    
    if index ~= -1 then
        for i=DequeObject.first + index, DequeObject.last-1 do
            DequeObject[i] = DequeObject[i + 1]
        end
        DequeObject[DequeObject.last] = nil
        DequeObject.last = DequeObject.last - 1    
    end
    
end

function Deque.iterator(DequeObject, index )
    return _iterator, DequeObject, index or 0
end

local function _invIterator(d, var)
    local last = d.last - var
    if d.first > last then
        return
    end
    var = var + 1
    return var, d[last]
end

function Deque.invIterator(DequeObject, index )
    return _invIterator, DequeObject, index or 0
end

function Deque.isEmtpy(DequeObject )
	return DequeObject.first > DequeObject.last
end

function Deque.getLength(DequeObject)
	return ( DequeObject.last - DequeObject.first ) +1
end

function Deque.pushFront(DequeObject, value)
	local first = DequeObject.first- 1
	DequeObject.first = first
	DequeObject[first] = value
end

function Deque.pushBack(DequeObject, value)
	local last = DequeObject.last + 1
	DequeObject.last = last
	DequeObject[last]= value
end

function Deque.popFront(DequeObject)
	local first = DequeObject.first
	if first > DequeObject.last then
		return
	end

	local value = DequeObject[first]
	DequeObject[first] = nil
	DequeObject.first = first +1
	return value
end

function Deque.popBack(DequeObject)
	local last = DequeObject.last
	if DequeObject.first > last then
		return
	end
    
    local value = DequeObject[last]
	DequeObject[last] = nil
	DequeObject.last = last - 1
	return value
end

function Deque.getBack(DequeObject)
    if true == Deque.isEmtpy(DequeObject) then
        return
    end
    return DequeObject[DequeObject.last]
end

function Deque.getFront(DequeObject)
    if true == Deque.isEmtpy(DequeObject) then
        return
    end
    return DequeObject[DequeObject.first]
end
