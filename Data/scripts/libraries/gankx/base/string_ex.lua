module("string", package.seeall)

Empty = ""

local UIInputValidateExport = CS.UITextFieldValidateExport

local function chsize(char)
    if not char then
        print("not char")
        return 0
    elseif char > 240 then
        return 4
    elseif char > 225 then
        return 3
    elseif char > 192 then
        return 2
    else
        return 1
    end
end

function trim(s)
    return s:gsub("^%s*(.-)%s*$", "%1", 1)
end

function split(s, sep)
    local sep, fields = sep or ":", {}
    local pattern = string.format("([^%s]+)", sep)
    s:gsub(pattern, function(c) fields[#fields + 1] = c end)
    return fields
end

function splitEx( _str, seperator)
    local pos, arr = 0, {}
    for st, sp in function() return string.find( _str, seperator, pos, true ) end do
        table.insert(arr, string.sub(_str, pos, st-1 ))
        pos = sp + 1
    end
    table.insert(arr, string.sub( _str, pos))
    return arr
end

function substitute(str, ...)
    local args = { ... }
    return str:gsub("%{(%d+)%}", function(str)
        return args[tonumber(str)]
    end)
end

function substitueByList(str, list)
    if nil == list then
        return str
    end

    local args = list

    for i, v in ipairs(args) do
        str = str:gsub("{" .. i .. "}", tostring(v))
    end

    return str
end


function UTF8len(str)
    local len = 0
    local currentIndex = 1
    while currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        currentIndex = currentIndex + chsize(char)
        len = len + 1
    end
    return len
end

function UTF8Sub(str, beginUTF8Index, endUTF8Index)
    if nil == str then
        return ""
    end

    if nil == beginUTF8Index or beginUTF8Index <= 0 then
        return ""
    end

    if nil == endUTF8Index or endUTF8Index < beginUTF8Index then
        return ""
    end

    local beginCharIndex, endCharIndex

    local currentCharIndex = 1
    local currentUTF8Index = 1
    while currentCharIndex <= #str do
        local char = string.byte(str, currentCharIndex)

        if currentUTF8Index == beginUTF8Index then
            beginCharIndex = currentCharIndex
        end

        if currentUTF8Index == endUTF8Index then
            endCharIndex = currentCharIndex + chsize(char) - 1
            break
        end

        currentCharIndex = currentCharIndex + chsize(char)
        currentUTF8Index = currentUTF8Index + 1
    end

    if nil == beginCharIndex or nil == endCharIndex then
        return ""
    end

    return string.sub(str, beginCharIndex, endCharIndex)
end

function widthSingle(inputstr)
    return UIInputValidateExport.GetStringLength(inputstr)
end

function trimLen(str)
    return string.len(string.trim(str))
end

function isValid(str)
    if nil == str then
        return false
    end
    if type(str) ~= "string" then
        return false
    end
    return trimLen(str) > 0
end

function startsWith(str, substr)
    if str == nil or substr == nil then
        return nil, "the string or the sub-stirng parameter is nil"
    end
    if string.find(str, substr) ~= 1 then
        return false
    else
        return true
    end
end

function endsWith(str, substr)
    if str == nil or substr == nil then
        return nil, "the string or the sub-string parameter is nil"
    end
    local str_tmp = string.reverse(str)
    local substr_tmp = string.reverse(substr)
    if string.find(str_tmp, substr_tmp) ~= 1 then
        return false
    else
        return true
    end
end

function splitWithPercent(str)
    local a,b = string.find(str,"%d+%%")
    local s1 = string.sub(str,1,a-1)
    local s2 = string.sub(str,a,b)
    return s1,s2
end

local function kmp_table(pattern)
    local result = {}
    for i = 1, #pattern+1,1 do
        local j = i
        while true do
            if j == 1 then
                result[#result + 1] = 1
                break
            end
            j = j-1
            if string.sub(pattern,result[j], result[j]) == string.sub(pattern,i, i) then
                result[#result + 1] = result[j] + 1
                break
            end
            j = result[j]
        end
    end
    return result
end

function kmp(haystack,needle)
    local fail = kmp_table(needle)
    local index, match = 0,1
    while index + match < #haystack do
        if string.sub(haystack,index + match, index + match) == string.sub(needle,match, match) then
            match = match + 1
            if match-1 == #needle then
                return index + 1
            end
        else
            if match == 1 then index = index + 1
            else
                index = index + match - (fail[match-1])
                match = fail[match-1]
            end
        end
    end
end
