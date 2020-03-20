module("Gankx.Mathf", package.seeall)

function clamp01(value)
    if value < 0.0 then
        return 0.0
    end
    if value > 1.0 then
        return 1.0
    end
    return value
end

function sqrt(f)
    return math.sqrt(f)
end

function min(a, b)
    return math.min(a, b)
end

function max(a, b)
    return math.max(a, b)
end

function getIntPart(x)
    if x <= 0 then
        return math.ceil(x);
    end

    if math.ceil(x) == x then
        x = math.ceil(x);
    else
        x = math.ceil(x) - 1;
    end
    return x;
end

function round(num)
    if num >= 0 then
        return math.floor(num + .5)
    else return math.ceil(num - .5)
    end
end