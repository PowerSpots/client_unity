Set = {}
Set.mt = {}

function Set.new (t)
    local set = {}
    setmetatable(set,Set.mt)
    if t ~= nil then
        for _, l in ipairs(t) do set[l] = true end
    end
    return set
end

function Set.union(a,b)
    local res = Set.new{}
    for i in pairs(a) do res[i] = true end
    for i in pairs(b) do res[i] = true end
    return res
end

function Set.intersection(a,b)
    local res = Set.new{}
    for i in pairs(a) do
        res[i] = b[i]
    end
    return res
end

function Set.tostring(set)
    local s = "{"
    local sep = ""
    for i in pairs(set) do
        s = s..sep..i
        sep = ","
    end
    return s.."}"
end

function Set.print(set)
    print(Set.tostring(set))
end

function Set.tolist(set)
    local ret = {}
    for i, v in pairs(set) do
        table.insert(ret,i)
    end
    return ret
end

Set.mt.__add = Set.union
Set.mt.__mul = Set.intersection
