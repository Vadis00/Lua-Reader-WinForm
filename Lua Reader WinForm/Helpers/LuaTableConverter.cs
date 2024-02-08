using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace Lua_Reader_WinForm.Helpers
{
    public class LuaTableConverter
    {
        public List<dynamic> ConvertLuaTableToDynamicList(DynValue luaTable, string luaCode)
        {
            List<dynamic> dynamicList = new List<dynamic>();

            char[] separator = { '\n' };
            string[] luaCodeSplit = luaCode.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            var luaOriginalDictionary = CreateLuaOriginalDictionary(luaCodeSplit);

            foreach (var luaTableRow in luaTable.Table.Pairs)
            {
                var originalRow = GetLuaOriginalLine(luaOriginalDictionary, luaTableRow.Key.ToString());

                if (luaTableRow.Value.Type == DataType.Table)
                {
                    dynamic dynamicObject = new ExpandoObject();

                    foreach (var keyValuePair in luaTableRow.Value.Table.Pairs)
                    {
                        string key = keyValuePair.Key.String;

                        AddProperty(dynamicObject, key, ProcessLuaValue(keyValuePair.Value, originalRow, key, false));
                    }

                    dynamicList.Add(dynamicObject);
                }
            }

            return dynamicList;
        }

        private Dictionary<string, string> CreateLuaOriginalDictionary(string[] lines)
        {
            var luaOriginalDictionary = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                string trimmedLine = line.TrimStart();
                if (trimmedLine.StartsWith("[") && trimmedLine.Contains("]"))
                {
                    string luaKey = trimmedLine.Substring(1, trimmedLine.IndexOf("]") - 1);
                    luaOriginalDictionary[luaKey] = line;
                }
            }
            return luaOriginalDictionary;
        }

        private void AddProperty(dynamic expando, string propertyName, object propertyValue)
        {
            var expandoDict = (IDictionary<string, object>)expando;
            expandoDict[propertyName] = propertyValue;
        }

        private string ProcessLuaValue(DynValue luaValue, string originalRow, string key, bool isNestedTable)
        {
            switch (luaValue.Type)
            {
                case DataType.Table:
                    return ProcessNestedTable(luaValue.Table, originalRow, key);

                case DataType.Number:
                    return luaValue.Number.ToString(numberFormatCulture);

                case DataType.String:
                    if (!isNestedTable)
                        //  return $"'{luaValue.String}'";
                        return $"'{EscapeStringValue(luaValue.String)}'";
                    else
                        //     return $"\"{luaValue.String}\"";
                        return $"'{EscapeStringValue(luaValue.String)}'";

                default:
                    return luaValue.CastToString();
            }
        }

        private string ProcessNestedTable(Table nestedTable, string originalRow, string key)
        {
            StringBuilder result = new StringBuilder();
            result.Append("{");

            bool firstPair = true;

            foreach (var pair in nestedTable.Pairs)
            {
                if (!firstPair)
                {
                    result.Append(",");
                }

                if (pair.Key.Type == DataType.Number)
                {
                    string valueForKey = null;

                    if (pair.Key.Type == DataType.Number)
                    {
                        var keyOriginalValue = GetLuaValue(originalRow, key);
                        if (keyOriginalValue != null)
                        {
                            valueForKey = GetValueForKey(keyOriginalValue, pair.Key.Number);
                        }
                    }

                    if (valueForKey != null)
                    {
                        result.Append($"[{pair.Key.Number}]={ProcessLuaValue(pair.Value, originalRow, key, true)}");
                    }
                    else
                    {
                        result.Append($"{ProcessLuaValue(pair.Value, originalRow, key, true)}");
                    }
                }
                else
                {
                    result.Append($"{pair.Key.String}={ProcessLuaValue(pair.Value, originalRow, key, true)}");
                }

                firstPair = false;
            }

            result.Append("}");

            return result.ToString();
        }

        private readonly CultureInfo numberFormatCulture = new CultureInfo("en-US");

        public string SaveLuaTableToFile(DataTable dataTable, string tableName)
        {
            try
            {
                StringBuilder luaCode = new StringBuilder();
                luaCode.Append(tableName + " = { \n");

                foreach (DataRow row in dataTable.Rows)
                {
                    luaCode.Append("\t[" + row.ItemArray[0].ToString() + "] = {");
                    bool firstPair = true;

                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        var value = ProcessSavedLuaValue(row.ItemArray[i]);

                        if (value != " ")
                        {
                            if (!firstPair)
                            {
                                luaCode.Append(", ");
                            }

                            luaCode.Append(dataTable.Columns[i].ColumnName + " = ");
                            luaCode.Append(ProcessSavedLuaValue(row.ItemArray[i]).Replace("\r\n", "\\n").Replace("\n", "\\n"));
                        }
                        firstPair = false;
                    }

                    luaCode.Append("},\n");
                }

                luaCode.Append("}\nreturn " + tableName + "\n");

                return luaCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving Lua Table: {ex.Message}", "Error");
                return null;
            }
        }

        private string ProcessSavedLuaValue(object value)
        {
            string stringValue = value.ToString();

            //  string escapedValue = EscapeStringValue(stringValue);

            //   if (!IsNumber(escapedValue) && !IsEnclosedInBraces(escapedValue))
            //   {
            //     return $"'{escapedValue}'";
            //}

            if (IsEmptyEnclosedInBraces(value.ToString()))
                return "{}";

            return stringValue;
        }

        private string EscapeStringValue(string stringValue)
        {
            string innerEscapedValue = stringValue
                     .Replace("'", "\\'")
                     .Replace("\"", "\\\"");

            return innerEscapedValue;
        }

        private bool IsNumber(string value)
        {
            return double.TryParse(value, NumberStyles.Float, numberFormatCulture, out _);
        }

        private bool IsEnclosedInBraces(string value)
        {
            return value.StartsWith("{") && value.EndsWith("}");
        }

        private bool IsEmptyEnclosedInBraces(string value)
        {
            if (value == "{ }")
                return true;

            return false;
        }

        private string? GetLuaOriginalLine(Dictionary<string, string> luaOriginalDictionary, string luaKey)
        {
            if (luaOriginalDictionary.TryGetValue(luaKey, out string originalLine))
            {
                return originalLine;
            }
            return null;
        }

        private dynamic GetLuaValue(string originalRow, string key)
        {
            return ParseLuaTable(originalRow, key);
        }

        private string ParseLuaTable(string luaTable, string key)
        {
            string pattern = $"{key}\\s*=\\s*{{([^}}]*(?:{{[^}}]*}}[^}}]*)*)}}";
            Match match = Regex.Match(luaTable, pattern);

            if (match.Success)
            {
                return "{" + match.Groups[1].Value + "}";
            }

            return null;
        }

        private string GetValueForKey(string luaTable, double key)
        {
            // Паттерн для поиска ключа и соответствующего ему значения
            string pattern = $@"\[{key}\]\s*=\s*(\d+)";

            Match match = Regex.Match(luaTable, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }
    }
}