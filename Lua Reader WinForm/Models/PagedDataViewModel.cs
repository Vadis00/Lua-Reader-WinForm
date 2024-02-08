using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace Lua_Reader_WinForm.Models
{
    public class PagedDataViewModel : INotifyPropertyChanged
    {
        private int pageSize = 500000;
        private int currentPage = 1;
        private List<dynamic> allData;
        private DataTable dataTable;

        public PagedDataViewModel(List<dynamic> allData)
        {
            this.allData = allData;
        }

        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, allData.Count);

            var newDataTable = await Task.Run(() => ConvertDynamicListToDataTable(allData.GetRange(startIndex, endIndex - startIndex)));

            if (dataTable == null)
            {
                dataTable = newDataTable;
            }
            else
            {
                DataTable combinedDataTable = new DataTable();

                foreach (DataColumn column in dataTable.Columns)
                {
                    combinedDataTable.Columns.Add(column.ColumnName, column.DataType);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    combinedDataTable.ImportRow(row);
                }

                foreach (DataRow row in newDataTable.Rows)
                {
                    combinedDataTable.ImportRow(row);
                }

                dataTable = combinedDataTable;
            }

            OnPropertyChanged(nameof(DataTable));
        }

        public async Task LoadNextPageAsync()
        {
            if (currentPage * pageSize < allData.Count)
            {
                currentPage++;
                await LoadDataAsync();
            }
        }

        public DataTable DataTable => dataTable;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine($"Property {propertyName} changed.");
        }

        private DataTable ConvertDynamicListToDataTable(List<dynamic> dynamicList)
        {
            try
            {
                DataTable dataTable = new DataTable();

                foreach (var dynamicObject in dynamicList)
                {
                    var currentObject = dynamicObject as IDictionary<string, object>;

                    if (currentObject != null)
                    {
                        var propertyNames = new List<string>(currentObject.Keys);

                        for (int i = 0; i < propertyNames.Count; i++)
                        {
                            var propertyName = propertyNames[i];

                            if (!dataTable.Columns.Contains(propertyName))
                            {
                                DataColumn column = new DataColumn(propertyName, typeof(object));

                                bool added = false;

                                for (int j = i - 1; j >= 0; j--)
                                {
                                    if (propertyNames[j] != null && dataTable.Columns.Contains(propertyNames[j]))
                                    {
                                        int previousIndex = dataTable.Columns.IndexOf(propertyNames[j]);
                                        dataTable.Columns.Add(column);
                                        dataTable.Columns[column.ColumnName].SetOrdinal(previousIndex + 1);
                                        added = true;
                                        break;  // Exit the inner loop once the column is added
                                    }
                                }

                                if (!added)
                                {
                                    dataTable.Columns.Add(column);
                                }
                            }
                        }
                    }
                }

                foreach (var dynamicObject in dynamicList)
                {
                    var dataRow = dataTable.NewRow();

                    var currentObject = dynamicObject as IDictionary<string, object>;

                    foreach (DataColumn column in dataTable.Columns)
                    {
                        if (currentObject != null && currentObject.ContainsKey(column.ColumnName))
                        {
                            var ggf = currentObject[column.ColumnName].ToString();

                            if (ggf.Contains("King Schmitz"))
                            {
                            }
                            dataRow[column.ColumnName] = currentObject[column.ColumnName];
                        }
                        else
                        {
                            dataRow[column.ColumnName] = " ";
                        }
                    }

                    dataTable.Rows.Add(dataRow);
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}