module("BitUtil", package.seeall)

function setBit(num, bit)
    local lShiftValue = (1 << bit)
    return num | lShiftValue
end

function clearBit(num, bit)
    local lShiftValue = (1 << bit)
    local notValue = ~lShiftValue
    return num & notValue
end

function testBit(num, bit)
    local lShiftValue = (1 << bit)
    return (num & lShiftValue) ~= 0
end

function numToBinary(num)
    local bitArray = {}

    for i = 0, 31 do
        table.insert(bitArray, 1, (num & 1))

        num = num >> 1
    end

    return table.concat(bitArray)
end

function numToBinaryArray(num)
    local bitArray = {}

    for i = 0, 31 do
        table.insert(bitArray, 1, (num & 1))

        num = num >> 1
    end

    return bitArray
end

function numToBoolArray(num)
    local bitArray = {}

    for i = 0, 31 do
        table.insert(bitArray, 1, (num & 1) == 1)

        num = num >> 1
    end

    return bitArray
end

BitsMask = 0xFFFFFFFF

function getLow32Bits(num)
    return num & BitsMask
end

function getHigh32Bits(num)
    return (num >> 32) & BitsMask
end

local bitsPerBytes = 8

function testBitByBytes(str, bit)
    if bit < 0 then
        return
    end
    if str == nil then
        return
    end
    local a,b = math.modf(bit/bitsPerBytes) + 1,math.fmod(bit,bitsPerBytes)

    if a > string.len(str) then
        return false
    end

    local curNumber = string.byte(str,a)

    local lShiftValue = (0x80 >> b)
    return (curNumber & lShiftValue) ~= 0
end

local bitsPerFixed32 = 32

function testBitByArray(array, bit)
    if bit < 0 then
        return
    end
    if array == nil then
        return
    end
    local a,b = math.modf(bit/bitsPerFixed32) + 1,math.fmod(bit,bitsPerFixed32)

    if a > #array then
        return false
    end

    local curNumber = array[a]

    local lShiftValue = (0x80000000 >> b)
    return (curNumber & lShiftValue) ~= 0
end

local bitsPerInt32 = 8

function testBitByIntArray(array, bit)
    if bit < 0 then
        return
    end
    if array == nil then
        return
    end
    local a,b = math.modf(bit/bitsPerInt32) + 1,math.fmod(bit,bitsPerInt32)

    if a > #array then
        return false
    end

    local curNumber = array[a]

    return testBit(curNumber,b)
end