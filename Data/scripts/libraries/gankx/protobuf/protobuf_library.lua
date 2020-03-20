local ProtobufPath = Booter.paths.libraries .. "gankx/protobuf/"
local File = Gankx.File
local Directory = Gankx.Directory

module("Gankx.Protobuf", package.seeall)

local pbLibrary

local function loadSource(path, modname)
    local func = Booter.loadfile(ProtobufPath .. path)
    local status, lib = pcall(func, modname)
    if not status then
        Console.error("Load Protobuf SourceFile(" .. tostring(path) .. ") Error: " .. lib)
        return nil
    end
    return lib
end

local workingPath = ""

local function loadProtoFile(filePath, backend)
    return File.readAllBytes(workingPath .. filePath .. ".proto")
end

function loadAll(filePath)
    local protos = {}

    if nil == filePath then
        return protos
    end

    workingPath = filePath

    local files = Directory.getFiles(workingPath, "proto", true, false)
    local extLength = string.len(".proto") + 1

    for _, file in ipairs(files) do
        local fileWithoutExt = string.sub(file, 0, -extLength)
        local status, proto = pcall(pbLibrary.require, fileWithoutExt)
        if not status then
            Console.error("Load proto file(" .. file .. ") occured error: " .. proto)
        else
            protos[fileWithoutExt] = proto
        end
    end

    workingPath = ""
    return protos
end

local function printProto(msg)
    Console.info(msg:SerializePartial('text'))
end

local function loadLibrary()
    loadSource("pb/proto/util.lua", "pb.proto.util")
    loadSource("pb/proto/scanner.lua", "pb.proto.scanner")
    loadSource("pb/proto/grammar.lua", "pb.proto.grammar")
    loadSource("pb/proto/parser.lua", "pb.proto.parser")

    loadSource("pb/utils.lua", "pb.utils")
    loadSource("pb/handlers.lua", "pb.handlers")

    loadSource("pb/standard/tools.lua", "pb.standard.tools")
    loadSource("pb/standard/buffer.lua", "pb.standard.buffer")
    loadSource("pb/standard/dump.lua", "pb.standard.dump")
    loadSource("pb/standard/repeated.lua", "pb.standard.repeated")
    loadSource("pb/standard/unknown.lua", "pb.standard.unknown")

    package.loaded["pb.standard.message"] = loadSource("pb/standard/message.lua", "pb.standard.message")
    loadSource("pb/standard/zigzag.lua", "pb.standard.zigzag")
    loadSource("pb/standard/pack.lua", "pb.standard.pack")
    loadSource("pb/standard/unpack.lua", "pb.standard.unpack")

    loadSource("pb/standard.lua", "pb.standard")

    loadSource("pb.lua", "pb")
    loadSource("utils.lua", "utils")

    pbLibrary = pb
    pbLibrary.load_proto_file = loadProtoFile
    pbLibrary.print = printProto
end

loadLibrary()

