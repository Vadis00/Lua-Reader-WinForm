using System.Data;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization.Json;

namespace Lua_Reader_WinForm.Service
{
    public class LuaService
    {
        private Script script;

        public LuaService()
        {
            script = new Script();
        }

        public string LoadLuaCode(string luaCode)
        {
            try
            {
                var tableName = GetLuaTableName(luaCode);
                script.DoString(luaCode);

                return tableName;
            }
            catch (ScriptRuntimeException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve a Lua table from a file.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
        }

        private string GetLuaTableName(string luaScript)
        {
            // Use a regular expression to find the table name
            Regex regex = new Regex(@"\b(\w+)\s*=\s*\{");
            Match match = regex.Match(luaScript);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        public DynValue GetLuaTable(string tableName)
        {
            try
            {
                var table = script.Globals.Get(tableName);
                return table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DynValue ParseLuaTable(string luaCode)
        {
            return script.DoString(luaCode);
        }

        public DynValue ConvertDataTableToLuaTable(DataTable dataTable)
        {
            Table luaTable = new Table(script);

            foreach (DataColumn column in dataTable.Columns)
            {
                luaTable[column.ColumnName] = column.DataType == typeof(string)
                    ? dataTable.AsEnumerable().Select(r => DynValue.NewString(r[column].ToString())).ToArray()
                    : dataTable.AsEnumerable().Select(r => DynValue.NewNumber(Convert.ToInt32(r[column]))).ToArray();
            }

            return DynValue.NewTable(luaTable);
        }

        public DataTable GetDataTableFromDataGridView(DataGridView dataGridView)
        {
            if (dataGridView != null)
            {
                if (dataGridView.DataSource is DataView dataView)
                {
                    return dataView.ToTable();
                }
                else if (dataGridView.DataSource is DataTable dataTable)
                {
                    return dataTable;
                }
            }

            return null;
        }
    }
}