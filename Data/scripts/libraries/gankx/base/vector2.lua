module("Gankx", package.seeall)

class("Vector2")

local Mathf = Gankx.Mathf
local Vector2 = Gankx.Vector2

function constructor(self, x, y)
    self.x = x or 0
    self.y = y or 0
end

function normalized(self)
    local vector2 = self:new(self.x, self.y)
    vector2:normalize()
    return vector2
end

function magnitude(self)
    return Mathf.Sqrt(self.x * self.x + self.y * self.y)
end

function sqrMagnitude(self)
    return self.x * self.x + self.y * self.y
end

function zero()
    return Vector2:new(0.0, 0.0)
end

function one(self)
    return Vector2:new(1.0, 1.0)
end

function up()
    return Vector2:new(0.0, 1.0)
end

function down()
    return Vector2:new(0.0, - 1.0)
end

function left()
    return Vector2:new(- 1.0, 0.0)
end

function right()
    return Vector2:new(1.0, 0.0)
end

function __add(a, b)
    return Vector2:new(a.x + b.x, a.y + b.y)
end

function __sub(a, b)
    return Vector2:new(a.x - b.x, a.y - b.y)
end

function __unm(a)
    return Vector2:new(- a.x, - a.y)
end

function __mul(a, b)
    return Vector2:new(a.x * b, a.y * b)
end

function __div(a, b)
    return Vector2:new(a.x / b, a.y / b)
end

function __eq(a, b)
    return Vector2.SqrMagnitude(a - b) < 9.99999943962493E-11
end

function set(self, new_x, new_y)
    self.x = new_x
    self.y = new_y
end

function lerp(a, b, t)
    t = Mathf.Clamp01(t)
    return Vector2:new(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t)
end

function lerpUnclamped(a, b, t)
    return Vector2:new(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t)
end

function moveTowards(current, target, maxDistanceDelta)
    local vector2 = target - current
    local magnitude = vector2:magnitude()
    if (magnitude <= maxDistanceDelta or magnitude == 0.0) then
        return target
    end
    return current + vector2 / magnitude * maxDistanceDelta
end

function scale(a, b)
    return Vector2:new(a.x * b.x, a.y * b.y)
end

function scaleSelf(self, scale)
    self.x = self.x * scale.x
    self.y = self.y * scale.y
end

function normalize(self)
    local magnitude = self:magnitude()
    if (magnitude > 9.99999974737875E-06) then
        self.x = self.x / magnitude
        self.y = self.y / magnitude
    else
        self.x = 0
        self.y = 0
    end
end

function __tostring(self)
    return string.substitute("({0}, {1})", self.x, self.y)
end

function reflect(inDirection, inNormal)
    return -2.0 * Vector2.Dot(inNormal, inDirection) * inNormal + inDirection
end

function dot(lhs, rhs)
    return (lhs.x * rhs.x + lhs.y * rhs.y)
end

function distance(a, b)
    local c = a - b
    return c:magnitude()
end

function clampMagnitude(vector, maxLength)
    if vector:sqrMagnitude() > maxLength * maxLength then

    end
end

function sqrMagnitude(a)
    return a.x * a.x + a.y * a.y
end

function min(lhs, rhs)
    return Vector2:new(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y))
end

function max(lhs, rhs)
    return Vector2:new(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y))
end