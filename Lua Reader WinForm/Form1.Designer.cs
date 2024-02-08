using Lua_Reader_WinForm.Helpers;
using Lua_Reader_WinForm.Models;
using Lua_Reader_WinForm.Service;
using System.Data;
using System.Reflection;

namespace Lua_Reader_WinForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl;

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem resizeColumnsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem increaseFontSizeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decreaseFontSizeMenuItem;

        private PagedDataViewModel pagedDataViewModel;

        private ToolStripControlHost openNewTabButtonHost;
        private System.Windows.Forms.Button openNewTabButton;

        private  LuaService luaService;
        private  FileService fileService;
        private  LuaTableConverter luaTableConverter;

        private bool isFullScreen = false;
        private int clickCount = 0;

        private Dictionary<int, TabPage> tabPages = new Dictionary<int, TabPage>();

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            luaService = new LuaService();
            fileService = new FileService();
            luaTableConverter = new LuaTableConverter();

            tabControl = new TabControl();
            progressBar = new ProgressBar();
            menuStrip = new MenuStrip();
            fileMenu = new ToolStripMenuItem();
            viewMenu = new ToolStripMenuItem();
            openNewTabButtonHost = new ToolStripControlHost(new Button());
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 24);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(800, 403);
            tabControl.TabIndex = 0;
 
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Bottom;
            progressBar.Location = new Point(0, 427);
            progressBar.MarqueeAnimationSpeed = 30;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(800, 23);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 1;
            progressBar.Visible = false;
            // 
            // menuStrip
            // 
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(800, 24);
            menuStrip.TabIndex = 2;
            // 
            // fileMenu
            // 
            this.fileMenu = new ToolStripMenuItem("File");
            this.fileMenu.DropDownItems.AddRange(new ToolStripItem[] {
                openMenuItem = new ToolStripMenuItem("Open", null, OpenLuaFile_Click),
                saveAsMenuItem = new ToolStripMenuItem("Save as", null, SaveLuaFile_Click)
            });
            this.fileMenu.Name = "fileMenu";
            // 
            // viewMenu
            // 
            this.viewMenu = new ToolStripMenuItem("View");
            this.viewMenu.DropDownItems.AddRange(new ToolStripItem[] {
                resizeColumnsMenuItem = new ToolStripMenuItem("Resize columns", null, ResizeColumns_Click),
                increaseFontSizeMenuItem = new ToolStripMenuItem("Size +", null, IncreaseFontSize_Click),
                decreaseFontSizeMenuItem = new ToolStripMenuItem("Size -", null, DecreaseFontSize_Click)
            });
            this.viewMenu.Name = "viewMenu";
            // 
            // openNewTabButtonHost 
            //
            openNewTabButton = new Button();
            openNewTabButton.AccessibleName = "openNewTabButtonHost";
            openNewTabButton.Dock = DockStyle.Top;
            openNewTabButton.Location = new Point(43, 2);
            openNewTabButton.Margin = new Padding(5);
            openNewTabButton.Name = "openNewTabButtonHost";
            openNewTabButton.Size = new Size(91, 25);
            openNewTabButton.TabIndex = 2;
            openNewTabButton.Text = "Open new tab";
            openNewTabButton.Click += OpenNewTab_Click;

            openNewTabButtonHost = new ToolStripControlHost(openNewTabButton);
            // 

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, viewMenu, openNewTabButtonHost });


            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl);
            Controls.Add(progressBar);
            Controls.Add(menuStrip);
            Name = "Lua Reader";
            Text = "Lua Reader";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenNewTab_Click(sender, e);
            tabControl.MouseDown += tabControl_MouseDown;

        }
        private void OpenNewTab_Click(object sender, EventArgs e)
        {
            TabPage newTabPage = new TabPage();
            newTabPage.Text = "New Tab      ";

            DataGridView newDataGridView = new DataGridView();
            newDataGridView.Name = "dataGrid";
            newDataGridView.Dock = DockStyle.Fill;
            newDataGridView.AutoGenerateColumns = false;
            newDataGridView.ScrollBars = ScrollBars.Both;
            newDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            newDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            newDataGridView.AllowUserToAddRows = false;
            newDataGridView.AllowUserToDeleteRows = false;
            newDataGridView.AllowUserToResizeRows = false;
            newDataGridView.MultiSelect = false;
            newDataGridView.CellDoubleClick += DataGrid_CellDoubleClick;
            newDataGridView.VirtualMode = true;
            newDataGridView.CellValueNeeded += DataGridView_CellValueNeeded;
            newDataGridView.Scroll += DataGridView_Scroll;
            newDataGridView.ReadOnly = false;
            newDataGridView.BackgroundColor = Color.White;

            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(newDataGridView, true);

            newTabPage.Controls.Add(newDataGridView);

            AddSearchControls(newDataGridView, newTabPage);
            tabControl.TabPages.Add(newTabPage);

            newTabPage.BackColor = Color.White;
            newTabPage.ForeColor = Color.Black;

            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += tabControl_DrawItem;

            int tabIdentifier = tabControl.TabPages.Count - 1;

            if (!tabPages.ContainsKey(tabIdentifier))
            {
                tabPages.Add(tabIdentifier, newTabPage);
            }
        }
        private void tabControl_MouseDown(object sender, MouseEventArgs e)
        {
            tabControl.MouseDown -= tabControl_MouseDown;

            for (int i = tabControl.TabPages.Count - 1; i >= 0; i--)
            {
                Rectangle rect = tabControl.GetTabRect(i);
                rect.Offset(rect.Width - 16, 4);
                rect.Width = 16;
                rect.Height = 16;

                if (rect.Contains(e.Location))
                {
                    int tabIdentifier = i;

                    tabControl.TabPages.RemoveAt(i);

                    if (tabControl.TabPages.Count == 0)
                    {
                        OpenNewTab_Click(sender, e);
                    }

                    break;
                }
            }

            tabControl.MouseDown += tabControl_MouseDown;
        }
        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
           e.Graphics.DrawString("✕", new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular), Brushes.Black, e.Bounds.Right - 20, e.Bounds.Top + 4);
           e.Graphics.DrawString(this.tabControl.TabPages[e.Index].Text, this.Font, Brushes.Black, e.Bounds.Left, e.Bounds.Top + 4);
           e.DrawFocusRectangle();
        }
        private void AddSearchControls(DataGridView dataGridView, TabPage tabPage)
        {
            TableLayoutPanel searchPanel = new TableLayoutPanel();
            searchPanel.Dock = DockStyle.Bottom;
            searchPanel.ColumnCount = 3;
            searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            searchPanel.Height = 30;
  

            TextBox searchTextBox = new TextBox();
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Height = 25;
            searchTextBox.Width = 300;
        //  searchTextBox.TextChanged += SearchTextBox_TextChanged;

            Label searchLabel = new Label();
            searchLabel.Text = "Search in the table: ";
            searchLabel.AutoSize = true;
            searchLabel.Height = 25;
            searchLabel.Anchor = AnchorStyles.None;



            Button searchButton = new Button();
            searchButton.AccessibleName = "Search";
            searchButton.Width = 100;
            searchButton.Text = "Search";
            searchButton.Click += (sender, e) => SearchTextBox_TextChanged(searchTextBox, EventArgs.Empty);

            // searchButton.Click += OpenNewTab_Click; 

            searchPanel.Controls.Add(searchLabel, 0, 0);
            searchPanel.Controls.Add(searchTextBox, 1, 0);
            searchPanel.Controls.Add(searchButton, 2, 0);

            tabPage.Controls.Add(searchPanel);
        }
        private void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
        }
        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
        }
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                TextBox textBox = (TextBox)sender;
                string searchTerm = textBox.Text.ToLower();
                var dataGridView1 = GetCurrentDataGrid().Item2;

                if (dataGridView1.DataSource is DataTable dataTable)
                {
                    string filterExpression = string.Join(" OR ",
                        dataTable.Columns.OfType<DataColumn>().Select(column =>
                            $"Convert({column.ColumnName}, 'System.String') LIKE '%{searchTerm}%'"));

                    dataTable.DefaultView.RowFilter = filterExpression;

                    if (dataGridView1.Rows.Count > 0)
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = 0;
                    }
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void DataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private async void OpenLuaFile_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                await LoadLuaTableFromFile();
            }
            finally
            {
                await Task.Delay(700);
                progressBar.Visible = false;
                Cursor.Current = Cursors.Default;
            }
        }
        private void SaveLuaFile_Click(object sender, EventArgs e)
        {
            SaveLuaTableToFile();
        }
        private void SaveLuaTableToFile()
        {
            Cursor.Current = Cursors.WaitCursor;
            var datagrid = GetCurrentDataGrid();

            if(datagrid == null || datagrid.Item2 == null)
            {
                return;
            }

            DataGridView dataGrid = datagrid.Item2;

            DataTable dataTable = luaService.GetDataTableFromDataGridView(dataGrid);
            Cursor.Current = Cursors.Default;

            if (dataTable == null)
            {
                return;
            }

            var luaCore = luaTableConverter.SaveLuaTableToFile(dataTable, datagrid.Item1);

            fileService.SaveFile(luaCore);
        }
        private void ResizeColumns_Click(object sender, EventArgs e)
        {
            clickCount++;
            var datagrid = GetCurrentDataGrid();
            DataGridView currentDataGrid = datagrid.Item2;

            int columnCount = currentDataGrid.Columns.Count;
            double maxWidthPercent = 1.0 / columnCount;

            double maxWidthPixels = currentDataGrid.Width * maxWidthPercent;

            switch (clickCount)
            {
                case 1:
                    foreach (DataGridViewColumn column in currentDataGrid.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    break;

                case 2:
                    foreach (DataGridViewColumn column in currentDataGrid.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        column.Width = 150;
                    }
                    clickCount = 0;
                    break;

                case 3:
                    foreach (DataGridViewColumn column in currentDataGrid.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        column.Width = 150;
                    }
                    clickCount = 0;
                    break;
            }
        }
        private void IncreaseFontSize_Click(object sender, EventArgs e)
        {
            var datagrid = GetCurrentDataGrid();
            DataGridView dataGridView = datagrid.Item2;

            dataGridView.Font = new Font(dataGridView.Font.FontFamily, dataGridView.Font.Size + 2, dataGridView.Font.Style);
        }
        private void DecreaseFontSize_Click(object sender, EventArgs e)
        {
            var datagrid = GetCurrentDataGrid();
            DataGridView dataGridView = datagrid.Item2;

            float newSize = Math.Max(2, dataGridView.Font.Size - 2);
            dataGridView.Font = new Font(dataGridView.Font.FontFamily, newSize, dataGridView.Font.Style);
        }
        private async Task LoadLuaTableFromFile()
        {
            Tuple<string, FileInfo> fileTuple = await fileService.OpenFileAsync();

            if (fileTuple != null)
            {
                string luaCode = fileTuple.Item1;
                FileInfo fileInfo = fileTuple.Item2;

                var tableName = luaService.LoadLuaCode(luaCode);
                dynamic luaTable = luaService.GetLuaTable(tableName);

                if (luaTable != null)
                {
                    var datagrid = GetCurrentDataGrid();
                    await DisplayLuaTable(luaTable, datagrid.Item2, luaCode);
                    SetTabHeaderText(tableName);
                }
                else
                {
                    MessageBox.Show("Failed to retrieve a Lua table from a file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private Tuple<string, DataGridView> GetCurrentDataGrid()
        {
            DataGridView resultGrid = null;
            string selectedTabName = null;

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    if (tabControl.SelectedTab != null)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        var selectedTab = tabControl.SelectedTab;
                        resultGrid = selectedTab.Controls.OfType<DataGridView>().FirstOrDefault();

                        Cursor.Current = Cursors.Default;

                        if (selectedTab.Text != null)
                        {
                            selectedTabName = selectedTab.Text;
                        }
                    }
                });
            }
            else
            {
                if (tabControl.SelectedTab != null)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    var selectedTab = tabControl.SelectedTab;
                    resultGrid = selectedTab.Controls.OfType<DataGridView>().FirstOrDefault();

                    Cursor.Current = Cursors.Default;

                    if (selectedTab.Text != null)
                    {
                        selectedTabName = selectedTab.Text;
                    }
                }
            }

            return new Tuple<string, DataGridView>(selectedTabName, resultGrid);
        }
        private async Task DisplayLuaTable(dynamic luaTable, DataGridView dataGridView, string luaCode)
        {
            try
            {
                var dynamicList = await ConvertLuaTableToDynamicListAsync(luaTable, luaCode);
                pagedDataViewModel = new PagedDataViewModel(dynamicList);
                await pagedDataViewModel.InitializeAsync();

                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        dataGridView.AutoGenerateColumns = true;

                        dataGridView.DataSource = pagedDataViewModel.DataTable;

                        dataGridView.EnableHeadersVisualStyles = false;
                        DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
                        columnHeaderStyle.BackColor = Color.LightBlue;
                        columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
                        dataGridView.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
                    });
                }
                else
                {
                    dataGridView.AutoGenerateColumns = true;

                    dataGridView.DataSource = pagedDataViewModel.DataTable;

                    dataGridView.EnableHeadersVisualStyles = false;
                    DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
                    columnHeaderStyle.BackColor = Color.LightBlue;
                    columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
                    dataGridView.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying Lua Table: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task<List<dynamic>> ConvertLuaTableToDynamicListAsync(dynamic luaTable, string luaCode)
        {
            return await Task.Run(() => luaTableConverter.ConvertLuaTableToDynamicList(luaTable, luaCode));
        }
        private void SetTabHeaderText(string name)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    if (tabControl.SelectedTab != null)
                    {
                        var selectedTab = tabControl.SelectedTab;
                        selectedTab.Text = name;
                    }
                });
            }
            else
            {
                if (tabControl.SelectedTab != null)
                {
                    var selectedTab = tabControl.SelectedTab;
                    if (tabControl.SelectedTab != null)
                    {
                        var selectedTab2 = tabControl.SelectedTab;
                        selectedTab2.Text = name;
                    }
                }
            }
        }

        #endregion
    }
}
