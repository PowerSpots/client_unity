local SteadyList = class("SteadyList")

function SteadyList:constructor(...)
    self._first = nil
    self._last = nil
    self._length = 0

    for _, v in ipairs({ ... }) do
        self:push(v)
    end
end

function SteadyList:length()
    return self._length
end

function SteadyList:push(t)
    local newNode = { _data = t }
    if self._last then
        self._last._next = newNode
        newNode._prev = self._last
        self._last = newNode
    else
        -- this is the first node
        self._first = newNode
        self._last = newNode
    end

    self._length = self._length + 1
end

function SteadyList:pop()
    if not self._last then
        return
    end

    local ret = self._last

    if ret._prev then
        ret._prev._next = nil
        self._last = ret._prev
        ret._prev = nil
    else
        self._first = nil
        self._last = nil
    end

    self._length = self._length - 1

    return ret._data
end

function SteadyList:unshift(t)
    local newNode = { _data = t }
    if self._first then
        self._first._prev = newNode
        newNode._next = self._first
        self._first = newNode
    else
        self._first = newNode
        self._last = newNode
    end

    self._length = self._length + 1
end

function SteadyList:shift()
    if not self._first then
        return
    end

    local ret = self._first

    if ret._next then
        ret._next.prev = nil
        self._first = ret._next
        ret._next = nil
    else
        self._first = nil
        self._last = nil
    end

    self._length = self._length - 1

    return ret._data
end

function SteadyList:insert(t, iter)
    local newNode = { _data = t }
    if iter then
        if iter._next then
            iter._next._prev = newNode
            newNode._next = iter._next
        else
            self._last = newNode
        end

        newNode._prev = iter
        iter._next = newNode
        self._length = self._length + 1
    elseif not self._first then
        self._first = newNode
        self._last = newNode
    end
end

function SteadyList:remove(iter)
    if iter._next then
        if iter._prev then
            iter._next._prev = iter._prev
            iter._prev._next = iter._next
        else
            iter._next._prev = nil
            self._first = iter._next
        end
    elseif iter._prev then
        iter._prev._next = nil
        self._last = iter._prev
    else
        self._first = nil
        self._last = nil
    end

    iter._next = nil
    iter._prev = nil

    self._length = self._length - 1
end

local function iterate(self, curIter)
    if not curIter then
        curIter = self._first
    elseif curIter then
        curIter = curIter._next
    end

    if curIter ~= nil then
        return curIter, curIter._data
    else
        return nil
    end
end

function SteadyList:iterate()
    return iterate, self, nil
end

function SteadyList:foreach(action, ...)
    local curIter = self._first
    while curIter ~= nil do
        action(curIter._data, ...)

        curIter = curIter._next
    end
end

return SteadyList
