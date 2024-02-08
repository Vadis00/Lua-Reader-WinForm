using MoonSharp.Interpreter;
using System.Text;

namespace Lua_Reader_WinForm.Helpers
{
    public class LuaTableToString
    {
        public string TableToString(DynValue luaTable)
        {
            if (luaTable.Type != DataType.Table)
                return luaTable.ToPrintString();

            Table table = luaTable.Table;
            StringBuilder result = new StringBuilder("{");

            foreach (var pair in table.Pairs)
            {
                result.Append($"{pair.Key} = {TableValueToString(pair.Value)}, ");
            }

            result.Append("}");

            return result.ToString();
        }

        private string TableValueToString(DynValue luaValue)
        {
            switch (luaValue.Type)
            {
                case DataType.Table:
                    return TableToString(luaValue);

                default:
                    return luaValue.ToPrintString();
            }
        }
    }
}