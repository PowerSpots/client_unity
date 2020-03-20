Vector3 = {}
Vector3.__index = Vector3

function Vector3.__add(a, b)
    if type(a) == "number" then
        return Vector3.new(b.x + a, b.y + a, b.z + a)
    elseif type(b) == "number" then
        return Vector3.new(a.x + b, a.y + b, a.z + b)
    else
        return Vector3.new(a.x + b.x, a.y + b.y, a.z + b.z)
    end
end

function Vector3.__sub(a, b)
    if type(a) == "number" then
        return Vector3.new(a - b.x, a - b.y, a - b.z)
    elseif type(b) == "number" then
        return Vector3.new(a.x - b, a.y - b, a.z - b)
    else
        return Vector3.new(a.x - b.x, a.y - b.y, a.z - b.z)
    end
end

function Vector3.__mul(a, b)
    if type(a) == "number" then
        return Vector3.new(b.x * a, b.y * a, b.z * a)
    elseif type(b) == "number" then
        return Vector3.new(a.x * b, a.y * b, a.z * b)
    else
        return Vector3.new(a.x * b.x, a.y * b.y, a.z * b.z)
    end
end

function Vector3.__div(a, b)
    if type(a) == "number" then
        return Vector3.new(a / b.x, a / b.y, a / b.z)
    elseif type(b) == "number" then
        return Vector3.new(a.x / b, a.y / b, a.z / b)
    else
        return Vector3.new(a.x / b.x, a.y / b.y, a.z / b.z)
    end
end

function Vector3.__eq(a, b)
    return a.x == b.x and a.y == b.y and a.z == b.z
end

function Vector3.__lt(a, b)
    return a.x < b.x or (a.x == b.x and a.y < b.y) or (a.x == b.x and a.y == b.y and a.x < b.x)
end

function Vector3.__le(a, b)
    return a.x <= b.x and a.y <= b.y and a.z <= b.z
end

function Vector3.__tostring(a)
    return "(" .. a.x .. ", " .. a.y .. ", " .. a.z .. ")"
end

function Vector3.new(posx, posy, posz)
    if type(posx) == "table" or type(posx) == "userdata" then
        return setmetatable({ x = posx.x or 0, y = posx.y or 0, z = posx.z or 0}, Vector3)
    end
    return setmetatable({ x = posx or 0, y = posy or 0, z = posz or 0}, Vector3)
end

function Vector3.distance(a, b, ignoreY)
    if getmetatable(a) ~= Vector3 then a = Vector3.new(a) end
    if getmetatable(b) ~= Vector3 then b = Vector3.new(b) end
    local x = a.x - b.x
    local y = 0
    if not ignoreY then y = a.y - b.y end
    local z = a.z - b.z
    return math.sqrt(x * x + y * y + z * z)
end

function Vector3.unsafeadd(a, b)
    if type(a) == "number" then
        b.x = b.x + a
        b.y = b.y + a
        b.z = b.z + a
        return b
    elseif type(b) == "number" then
        a.x = a.x + b
        a.y = a.y + b
        a.z = a.z + b
        return a
    else
        a.x = a.x + b.x
        a.y = a.y + b.y
        a.z = a.z + b.z
        return a
    end
end

function Vector3.unsafesub(a, b)
    if type(a) == "number" then
        b.x = a - b.x
        b.y = a - b.y
        b.z = a - b.z
        return b
    elseif type(b) == "number" then
        a.x = a.x - b
        a.y = a.y - b
        a.z = a.z - b
        return a
    else
        a.x = a.x - b.x
        a.y = a.y - b.y
        a.z = a.z - b.z
        return a
    end
end

function Vector3.unsafemul(a, b)
    if type(a) == "number" then
        b.x = a * b.x
        b.y = a * b.y
        b.z = a * b.z
        return b
    elseif type(b) == "number" then
        a.x = a.x * b
        a.y = a.y * b
        a.z = a.z * b
        return a
    else
        a.x = a.x * b.x
        a.y = a.y * b.y
        a.z = a.z * b.z
        return a
    end
end

function Vector3.unsafediv(a, b)
    if type(a) == "number" then
        b.x = a / b.x
        b.y = a / b.y
        b.z = a / b.z
        return b
    elseif type(b) == "number" then
        a.x = a.x / b
        a.y = a.y / b
        a.z = a.z / b
        return a
    else
        a.x = a.x / b.x
        a.y = a.y / b.y
        a.z = a.z / b.z
        return a
    end
end

function Vector3.direction(a, b)
    if getmetatable(a) ~= Vector3 then a = Vector3.new(a) end
    if getmetatable(b) ~= Vector3 then b = Vector3.new(b) end
    return (b - a):normalize()
end

function Vector3:clone()
    return Vector3.new(self.x, self.y, self.z)
end

function Vector3:unpack()
    return self.x, self.y, self.z
end

function Vector3:len()
    return math.sqrt(self.x * self.x + self.y * self.y + self.z * self.z)
end

function Vector3:lenSq()
    return self.x * self.x + self.y * self.y + self.z * self.z
end

function Vector3:normalize()
    local len = self:len()
    self.x = self.x / len
    self.y = self.y / len
    self.z = self.z / len
    return self
end

function Vector3:normalized()
    return self / self:len()
end

setmetatable(Vector3, { __call = function(_, ...) return Vector3.new(...) end })