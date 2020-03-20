public sealed class LuaStringServiceExport
{
    public static int GetStringId(string luastring)
    {
        return LuaStringService.instance.GetStringId(luastring);
    }

    public static string GetString(int id)
    {
        return LuaStringService.instance.GetString(id);
    }

    public static void ClearString()
    {
        LuaStringService.instance.ClearString();
    }
}

