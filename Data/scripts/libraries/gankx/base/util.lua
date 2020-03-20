module("Util", package.seeall)

local tableConcat = table.concat
local tableInsert = table.insert
local stringRep = string.rep
local stringFormat = string.format
local type = type
local pairs = pairs
local tostring = tostring

function formatTable(root)
    local cache = { [root] = "." }
    local space, deep = string.rep(' ', 2), 0
    local function _dump(t, name)
        local temp = {}
        for k, v in pairs(t) do
            local key = stringFormat("[%s]", tostring(k))
            if cache[v] then
                tableInsert(temp, stringFormat("%s%s = [%s]\n", stringRep(space, deep + 1), key, cache[v]))
            elseif type(v) == "table" then
                deep = deep + 2
                local new_key = name .. "." .. key
                cache[v] = new_key
                if "[.data]" ~= key then
                    tableInsert(temp, stringFormat("%s%s = %s\n%s", stringRep(space, deep - 1), key, stringRep(space, deep - 1), _dump(v, new_key), stringRep(space, deep - 1)))
                else
                    tableInsert(temp, stringFormat("%s\n", _dump(v, new_key)))
                end

                deep = deep - 2
            else
                tableInsert(temp, stringFormat("%s%s = [%s]\n", stringRep(space, deep + 1), key, tostring(v)))
            end
        end
        return tableConcat(temp)
    end

    return stringFormat("[%s] = {\n%s}", tostring(root), _dump(root, ""))
end


function isEmpty(data)
    return data == nil or data == 0
end

local function table2str_recur(tab, seen, prevBlankSpace, blankSpace)
    if nil == tab then
        return "nil"
    end

    if type(tab) ~= 'table' then
        return tostring(tab)
    end

    if seen[tab] then
        return "self"
    else
        seen[tab] = tab
    end

    local result = "{\n"
    for k, v in pairs(tab) do
        local kStr
        if type(k) == 'number' then
            kStr = blankSpace .. "[" .. table2str_recur(k, seen, blankSpace, blankSpace .. "    ") .. "]"
        else
            kStr = blankSpace .. table2str_recur(k, seen, blankSpace, blankSpace .. "    ")
        end

        local vStr = table2str_recur(v, seen, blankSpace, blankSpace .. "    ")
        if type(v) == 'string' then
            result = result .. kStr .. " = \"" .. vStr .. "\",\n"
        else
            result = result .. kStr .. " = " .. vStr .. ",\n"
        end
    end

    return result .. prevBlankSpace .. "}"
end

function table2Str(mTable)
    local seen = {}
    return table2str_recur(mTable, seen, "", "")
end

function deepClone(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local new_table = {}
        lookup_table[object] = new_table
        for index, value in pairs(object) do
            new_table[_copy(index)] = _copy(value)
        end
        return setmetatable(new_table, getmetatable(object))
    end

    return _copy(object)
end

function clone(origin)
    local dest = {}
    for key, value in pairs(origin) do
        dest[key] = value
    end
    return dest
end

function bool2Int(boolVal)
    if boolVal == true then
        return 1
    end

    return 0
end

function formatObject(luaObject)
    print(formatTable(luaObject));
end

function consoleError(errorInfo)
    Console.error(stringFormat("%s\n%s", errorInfo, debug.traceback()))
end

function clearItemTable(itemTable)
    for i = #itemTable, 1, -1 do
        if 0 == itemTable[i].ID then
            table.remove(itemTable, i)
        end
    end
end

function getTableLen(tab)
    local count = 0
    for _, item in pairs(tab) do
        count = count + 1
    end
    return count
end

_G.Str = table2Str

function hash(v, ext)
    local ch = 0
    local val = 0

    if (v) then
        for i = 1, #v do
            ch = v:byte(i)
            if ( ch >= 65 and ch <= 90 ) then
                ch = ch + 32
            end
            val = val * 0.7 + ch
        end
    end
    val = val .. ''
    val = val:gsub("+", "")
    val = val:gsub("%.", "")


    if (ext) then
        return string.format('%s.%s', val, ext)
    else
        return string.format('%s', val)
    end
end

math.Clamp = function(var, down, up)
    if nil == var or nil == down or nil == up then
        return var
    end

    if var < down then
        var = down
    end

    if var > up then
        var = up
    end

    return var
end

AlwaysLog = true

function printf(str, ...)
    if not AlwaysLog and not CS.PlatformExport.IsInStandaloneOrEditor() then
        return
    end

    print(debug.traceback(string.format(str, ...)))
end


function printt(mTable, str)
    if not AlwaysLog and not CS.PlatformExport.IsInStandaloneOrEditor() then
        return
    end

    if str then
        print(debug.traceback(str .. ":" .. table2Str(mTable)))
    else
        print(debug.traceback(table2Str(mTable)))
    end
end

function LogNetworkMsg(msg)
    print(msg)
    CS.DebugExport.LogNetwork(msg)
end

_G.printf = printf

_G.printt = printt

function parseTime(s)
    if nil == s then
        Console.error("ParseTime Error: input is nil ")
        return nil
    end

    local y, m, d, h, mm, s = string.match(s, "(%d+)[^%d](%d+)[^%d](%d+)%s(%d+)[^%d](%d+)[^%d](%d+)");
    local t = { year = y, month = m, day = d, hour = h, min = mm, sec = s };
    local ok, result = pcall(os.time, t)
    if not ok then
        return nil
    end

    return result
end

_G.D = parseTime

local _CheckMsgData = function(msg, str)
    return msg[str]
end

function checkMsgData(msg, str)
    local bOK, err = pcall(_CheckMsgData, msg, str)
    return bOK and err
end

_G.CheckMsgData = checkMsgData

local _checkCSharpObject = function(obj)
    return not CS.UnityEngineObjectExtention.IsNull(obj)
end
function checkCSharpObject(obj)
    local bOK, err = pcall(_checkCSharpObject, obj)
    return bOK and err
end
_G.CheckCSharpObject = checkCSharpObject

function replaceSpace(msg)
    if nil == msg or "" == msg then
        return ""
    end

    return string.gsub(msg, ' ', _replaceSpaceMethod)
end

function _replaceSpaceMethod(subStr)
    return "\194\160"
end

function formatMsgString(msg)
    msg = replaceSpace(msg)
    msg = msg:gsub("\\n", "\n")
    msg = msg:gsub("\\t", "\t")
    return msg
end

function formatColorStr(str, color)
    return string.format("<color=#%s>%s</color>", color, str)
end

_G.FormatColorStr = formatColorStr

function randByRatio(array)
    local total_ratio = 0

    for _, value in ipairs(array) do
        total_ratio = total_ratio + value
    end

    local randomNum = math.random(0, total_ratio)
    for i, value in ipairs(array) do
        if randomNum < value then
            return i
        else
            randomNum = randomNum - value
        end
    end

    return #array
end

function isNanNumber(x)
    return x ~= x
end

function getGoldenCount(num)
    local text = tostring(num)
    if num > 99999 then
        num = num / 10000
        if num > 9999 then
            text = Gankx.Mathf.GetIntPart(num / 10000) .. "亿"
        else
            text = Gankx.Mathf.GetIntPart(num) .. "万"
        end
    end

    return text
end

function getScrollValue(showRowCount, allItemCount, itemIndex, perRowItemCount)
    perRowItemCount = perRowItemCount or 1
    local allRowCount = math.ceil(allItemCount / perRowItemCount)
    local itemRow = math.ceil(itemIndex / perRowItemCount)

    local scrollValue = 1
    if itemRow <= showRowCount then
        scrollValue = 1
    else
        if itemRow > allRowCount - showRowCount then
            scrollValue = 0
        else
            scrollValue = 1 - (itemRow - 1) / (allRowCount - showRowCount)
        end
    end

    return scrollValue
end

function getScrollValueHorizontal(showRowCount, allItemCount, itemIndex, perRowItemCount)
    perRowItemCount = perRowItemCount or 1
    local allRowCount = math.ceil(allItemCount / perRowItemCount)
    local itemRow = math.ceil(itemIndex / perRowItemCount)

    local scrollValue = 0
    if itemRow <= 1 then
        scrollValue = 0
    else
        if itemRow > allRowCount - showRowCount then
            scrollValue = 1
        else
            scrollValue = (itemRow - 1) / (allRowCount - showRowCount)
        end
    end

    return scrollValue
end

function formatColorText(color, text)
    if not string.isValid(color) then
        return text
    end
    if string.match(color , "#") then
        return string.format("<color=%s>%s</color>", color, text)
    end
    return string.format("<color=#%s>%s</color>", color, text)
end

_G.FormatColorText = formatColorText

function errorHandle(err)
    return debug.traceback(err)
end

_G.ErrorHandle = errorHandle
function safeCallFunction(f, ...)
    local bOK, err = xpcall(f, errorHandle, ...)
    if bOK then
        return err
    else
        Console.error(err)
    end
end
_G.SafeCallFunction = safeCallFunction

function genArrayIndexTable(c, b, e, trueValue, falseValue)
    local ret = {}
    for i = b, e do
        Array.add(ret,(i <= c) and trueValue or falseValue)
    end
    return ret
end

function formatSkillString(value, percent)
    if percent then
        if value % 100 == 0 then
            return math.floor(value/100)
        else
            return string.format("%.2f",value/100)
        end

    end
    return math.floor(value)
end

function safeSetSystemVisible(system, visible)
    local instance = system.instance
    if instance == nil then
        return
    end
    if visible then
        instance:showPanel()
    else
        instance:hidePanel()
    end
end

function urlEncode(s)
    s = string.substitute("{1}?{2}",s,TimeService.instance:GetCurTime())
    s = string.gsub(s, "([^%w%.%- ])", function(c) return string.format("%%%02X", string.byte(c)) end)
    return string.gsub(s, " ", "+")
end

_G.SafeSetSystemVisible = safeSetSystemVisible

function getUIWindowIdByPath(path)
    local strs = string.split(path, "/")
    local _PanelFileName = strs[1]
    local _RelPath
    if nil ~= _PanelFileName then
        _RelPath = string.sub(path, string.len(_PanelFileName) + 2)
    end

    local system,windowId = Gankx.SystemService.instance:GetSystemAndWindowIdByUIPath(_PanelFileName,_RelPath)

    return windowId
end