using Gankx;
using System.Collections.Generic;
using EmojText;
using XLua;

public static class InlineTextManagerExport
{
    /// <summary>
    /// 表情atlas 路径
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    public static LuaTable GetAllEmojiNames(string assetPath)
    {
        LuaTable namesTable = LuaService.instance.NewTable();
        List<string> names = InlineSpriteAssetManager.instance.GetEmojNameList(assetPath);
        for (int i = 0; i < names.Count; i++)
        {
            namesTable.Set(i + 1, names[i]);
        }
        return namesTable;
    }
}