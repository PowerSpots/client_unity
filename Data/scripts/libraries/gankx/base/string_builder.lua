local tableconcat = table.concat
local stringformat = string.format
local stringSubstitute = string.substitute

StringBuilder = {}

function StringBuilder:new(sep)
    local object = {}
	setmetatable(object, self)
	self.__index = self

    object.sep = sep
    object.buffer = {}

	return object
end

function StringBuilder:append(str)
    self.buffer[#self.buffer + 1] = str
end

function StringBuilder:appendFormat(format, ...)
    self:append(stringformat(format, ...))
end

function StringBuilder:appendSubstitute(format, ...)
    self:append(stringSubstitute(format, ...))
end

function StringBuilder:appendLine(str)
    self:append(str)
    self:append("\n")
end

function StringBuilder:toString()
    return tableconcat(self.buffer, self.sep)
end

function StringBuilder:clear()
    local count = #self.buffer
    for i = 1, count do
        self.buffer[i] = nil
    end
end
