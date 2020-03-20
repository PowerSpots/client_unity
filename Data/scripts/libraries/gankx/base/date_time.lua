module("DateTime", package.seeall)

local function getNow()
    local now = gs.DateTime_Now()
    return
    {
        year= now:GetYear(),
        month= now:GetMonth(),
        day= now:GetDay(),
        hour= now:GetHour(),
        min= now:GetMinute(),
        sec= now:GetSecond(),
        wday= now:DayOfWeek(),
        yday= now:DayOfYear(),
        wdayBaseMonday= now:DayOfWeekBaseMonday(),
    }
end

function match(obj)
    if type(obj)~= "table" then
        return false
    end

    local now= getNow()
    for k, v in pairs(now) do
        if obj[k]~= nil and obj[k]~= v then
            return false
        end
    end
    return true
end

function equalDay(sec)
    if type(sec)~= "number" then
        return false
    end

    local tm = gs.DateTime_FromBeijingTime(sec)
    local obj =
    {
        year= tm:GetYear(),
        month= tm:GetMonth(),
        day= tm:GetDay(),
    }

    return match(obj)
end
