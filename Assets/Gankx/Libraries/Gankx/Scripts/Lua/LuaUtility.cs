using UnityEngine;
using XLua;

namespace Gankx
{
    public class LuaUtility
    {
        public static object TableFieldToColor(LuaTable luaTable, object field)
        {
            if (null == luaTable || null == field)
            {
                return null;
            }

            var fieldValue = luaTable[field] as LuaTable;
            if (null == fieldValue)
            {
                return null;
            }

            var result = Color.white;

            for (var index = 0; index < 4; ++index)
            {
                var value = fieldValue[index + 1];
                if (value is double)
                {
                    result[index] = (float) (double) value;
                }
            }

            return result;
        }

        public static object TableFieldToInt(LuaTable luaTable, object field)
        {
            if (null == luaTable || null == field)
            {
                return null;
            }

            var fieldValue = luaTable[field];
            if (!(fieldValue is double))
            {
                return null;
            }

            return (int) (double) fieldValue;
        }
    }
}