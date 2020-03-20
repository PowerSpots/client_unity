using Gankx;
using System.Collections.Generic;


public class LuaStringService : Singleton<LuaStringService>
{
    private List<string> luaStringList = new List<string>();

    public int GetStringId(string luastring)
    {
        for (int i = 0; i < luaStringList.Count; i++)
        {
            if (luaStringList[i] == luastring)
            {
                return i;
            }
        }
        luaStringList.Add(luastring);
        return luaStringList.Count - 1;
    }

    public string GetString(int id)
    {
        string result = string.Empty;
        if (id < luaStringList.Count)
        {
            if (luaStringList[id] != null)
            {
                result = luaStringList[id];
            }
        }
        return result;
    }

    public void ClearString()
    {
        luaStringList.Clear();
    }
}