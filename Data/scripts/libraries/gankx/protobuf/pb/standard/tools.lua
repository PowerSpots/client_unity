--
-- Created by IntelliJ IDEA.
-- User: rubbyzhang
-- Date: 2017/5/4
-- Time: 23:06
-- To change this template use File | Settings | File Templates.
--


module(...,package.seeall)

function IsLua53()
   return _G._VERSION == "Lua 5.3"
end

---------------------------Bit
-- 5.3 fit to luajit bit operation
local bit53 =
{

}

bit53.band = function(a, b)
    return a & b
end

bit53.bor = function(a, b)
    return a | b
end

bit53.bxor = function(a, b)
    return a ~ b
end

bit53.bnot = function(a)
    return ~ a
end

bit53.lshift = function(a, b)
    return a << b
end

bit53.rshift = function(a, b)
    return a >> b
end

bit53.arshift = function(a, b)
    a = a & 0xFFFFFFFF
    if b <= 0 or (a & 0x80000000) == 0 then
        return (a >> b) & 0xFFFFFFFF
    else
        return((a >> b) | ~(0xFFFFFFFF >> b)) & 0xFFFFFFFF
    end
end

--- 5.3 using default bit methond
--- lua5.2 can use "bit32"
--- Lua51 or luaji can use "jit bit operation" (http://bitop.luajit.org/api.html)
local bitlib = nil 
if IsLua53() then
    bitlib = bit53
else
    bitlib = require "bit"
end

bit = {} 
bit.band = bitlib.band
bit.bxor = bitlib.bxor
bit.bor = bitlib.bor
bit.lshift = bitlib.lshift
bit.rshift = bitlib.rshift
bit.arshift = bitlib.arshift

--------------------------struct
-- Lua53 using default string  pack/unpack 
-- other version can use "struct lib'(http://www.inf.puc-rio.br/~roberto/struct/)

struct = {}
struct.pack = nil
struct.unpack = nil
struct.size = nil

if IsLua53() then
    struct.pack = string.pack
    struct.unpack = string.unpack
    struct.size = string.packsize
else
	local structlib = require"struct"
    struct.pack = structlib.pack
    struct.unpack = structlib.unpack
    struct.size = structlib.size
end





