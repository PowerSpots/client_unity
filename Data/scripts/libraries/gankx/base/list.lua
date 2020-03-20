module("List", package.seeall)

local ListNode = {}
ListNode.__index = ListNode

function ListNode:new()
	local listNode = {}
	setmetatable(listNode, self)

	listNode.value = nil
	listNode.next = nil
	listNode.prev = nil

	return listNode
end

local ListIterator = {}
ListIterator.__index = ListIterator


function ListIterator:new(_owner)
	local listIterator = {}
	setmetatable(listIterator, self)

	listIterator.node = nil
	listIterator.owner = _owner

	return listIterator
end

function ListIterator:next(count)
	count = count or 1

	for i = 1, count do
		if self.node ~= nil then
			self.node = self.node.next
		else
			return false
		end
	end

	return self.node ~= nil
end

function ListIterator:prev(count)
	count = count or 1

	for i = 1, count do
		if self.node ~= nil then
			self.node = self.node.prev
		else
			return	false
		end
	end

	return self.node ~= nil
end

function ListIterator:value()
	if self.node ~= nil then
		return self.node.value
	end

	return nil
end

function ListIterator:erase()
	if self.owner ~= nil then
		return self.owner:erase(self)
	end
end

function ListIterator:Valid()
	return self.owner ~= nil and self.node ~= nil
end

local ListMeta = {}
ListMeta.__index = ListMeta

function new()
	local list = {}
	setmetatable(list, ListMeta)

	list._first = nil
	list._last = nil

	return list
end

function ListMeta:pushBack(value)
	if nil == self._last or
		nil == self._first then
		self._first = ListNode:new()
		self._last = self.first
		self._last.value = value
		return
	end

	local newNode = ListNode:new()
	newNode.prev = self._last
	newNode.value = value
	self._last.next = newNode
	self._last = newNode
end

function ListMeta:popBack()
	if nil == self.last then
		return nil
	end

	local back = self:back()

	local prevNode = self.last.prev
	if prevNode == nil then
		self.first = nil
		self.last = nil
	else
		prevNode.next = nil
		self.last = prevNode
	end

	return back
end

function ListMeta:pushFront(value)
	if nil == self.first or
		nil == self.last then
		self.first = ListNode:new()
		self.last = self.first
		self.last.value = value
		return
	end

	local newNode = ListNode:new()
	newNode.value = value
	newNode.next = self.first
	self.first.prev = newNode
	self.first = newNode
end

function ListMeta:popFront()
	if nil == self.first then
		return nil
	end

	local front = self:front()

	local nextNode = self.first.next
	if nextNode == nil then
		self.first = nil
		self.last = nil
	else
		nextNode.prev = nil
		self.first = nextNode
	end

	return front
end

function ListMeta:front()
	if self.first ~= nil then
		return self.first.value
	end

	return nil
end

function ListMeta:back()
	if self.last ~= nil then
		return self.last.value
	end

	return nil
end

function ListMeta:empty()
	return nil == self.first or nil == self.last
end

function ListMeta:clear()
	self.first = nil
	self.last = nil
end

function ListMeta:ibegin()
	local itr = ListIterator:new(self)
	itr.node = self.first
	return itr
end

function ListMeta:iend()
	local itr = ListIterator:new(self)
	itr.node = self.last

	return itr
end

function ListMeta:find(v, start)
	if start == nil then
		start = self:ibegin()
	end

	repeat
		if v == start:Value() then
			return start
		end
	until start:next() == false

	return nil
end

function ListMeta:rfind(v, start)
	if start == nil then
		start = self:iend()
	end

	repeat
		if v == start:value() then
			return start
		end
	until start:prev() == false

	return nil
end

function ListMeta:erase(itr)

	if nil == itr or nil == itr.node or
		itr.owner ~= self then
		return itr
	end

	local nextItr = ListIterator:new(self)
	nextItr.node = itr.node
	nextItr:next()

	if itr.node == self.first then
		self:pop_front()
	elseif itr.node == self.last then
		self:pop_back()
	else
		local prevNode = itr.node.prev
		local nextNode = itr.node.next

		if prevNode ~= nil then
			prevNode.next = nextNode
		end

		if nextNode ~= nil then
			nextNode.prev = prevNode
		end
	end

	itr.owner = nil
	itr.node = nil

	return nextItr

end

function ListMeta:eraseValue(value)
	local itr = self:find(value)
	self:erase(itr)
end

function ListMeta:eraseAll(value)
	local itr = self:find(value)
	while itr ~= nil and itr:valid() do
		itr = self:erase(itr)
		itr = self:find(value, itr)
	end
end

function ListMeta:insert(itr, value)
	if nil == itr or nil == itr.node or
		itr.owner ~= self then
		return
	end

	local result_itr = ListIterator:new(self)

	if itr.node == self.last then
		self:push_back(value)
		result_itr.node = self.last
	else
		local prevNode = itr.node
		local nextNode = itr.node.next
		local newNode = ListNode:new()
		newNode.value = value
		prevNode.next = newNode
		nextNode.prev = newNode
		newNode.next = nextNode
		newNode.prev = prevNode

		result_itr.node = newNode
	end

	return result_itr
end

function ListMeta:insertBefore(itr, value)
	if nil == itr or nil == itr.node or
		itr.owner ~= self then
		return
	end

	local result_itr = ListIterator:new(self)

	if itr.node == self.first then
		self:push_front(value)
		result_itr.node = self.first
	else
		local prevNode = itr.node.prev
		local nextNode = itr.node
		local newNode = ListNode:new()
		newNode.value = value
		prevNode.next = newNode
		nextNode.prev = newNode
		newNode.next = nextNode
		newNode.prev = prevNode

		result_itr.node = newNode
	end

	return result_itr
end

function _G.ilist(l)

	local itr_first = ListIterator:new(l)
	itr_first.node = ListNode:new()
	itr_first.node.next = l.first

	local function ilist_it(itr)

		itr:next()
		local v = itr:value()

		if v ~= nil then
			return v, itr
		else
			return nil
		end

	end

	return ilist_it, itr_first
end

function _G.rilist(l)

	local itr_last = ListIterator:new(l)
	itr_last.node = ListNode:new()
	itr_last.node.prev = l.last

	local function rilist_it(itr)

		itr:prev()
		local v = itr:value()

		if v ~= nil then
			return v, itr
		else
			return nil
		end

	end

	return rilist_it, itr_last
end

function ListMeta:print()
	for v in iList(self) do
		print(tostring(v))
	end
end

function ListMeta:size()
	local count = 0

	for v in ilist(self) do
		count = count + 1
	end

	return count
end

function ListMeta:clone()
	local newList = List:new()
	for v in ilist(self) do
		newList:push_back(v)
	end
	return newList
end

function ListMeta:contains(value)
	if value == nil then
		return
	end
	local first = self.first
	if first == nil then
		return false
	end
	local cur = first
	repeat
		if cur.value == value then
			return true
		end
		cur = cur.next
	until cur == nil or cur == first
	return false
end
