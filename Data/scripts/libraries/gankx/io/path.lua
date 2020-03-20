module("Gankx.Path", package.seeall)

sperator = "/"

function normalize(filePath)

    filePath = string.lower(filePath)
    filePath = table.concat(string.split(filePath, "\\/"), "/")
    if string.len(filePath) > 0 then
        return filePath .. "/"
    else
        return ""
    end
end

function shortName(filePath)
    if nil == filePath then
        return ""
    end

    local token = ""
    for _token in string.gmatch(filePath, "[^\\/]+") do
        token = _token
    end

    return token
end

function combine(filepath1, filepath2)
    if nil == filepath1 or nil == filepath2 then
        return
    end

    return filepath1 .. "/" .. filepath2
end

function split(filepath)
    if nil == filepath then
        return
    end

    local dir, file, post = string.match(filepath, "(.-)([^\\/]-([^\\/%.]+))$")

    if dir ~= nil then
        dir = normalize(dir)
    end

    return dir, file, post
end
