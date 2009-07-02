namespace SubSonic.SubStage
{
    partial class SubStageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(activeServer != null)
            {
                activeServer.Stop();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubStageForm));
            this.pGrid = new PropertyGridEx.PropertyGridEx();
            this.pgbTestConnection = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileImportProject = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.kryptonManager1 = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNewProject = new System.Windows.Forms.ToolStripButton();
            this.btnNewProvider = new System.Windows.Forms.ToolStripButton();
            this.btnAddConnectionString = new System.Windows.Forms.ToolStripButton();
            this.btnSplitGenerateCode = new System.Windows.Forms.ToolStripSplitButton();
            this.miScriptSchemas = new System.Windows.Forms.ToolStripMenuItem();
            this.miScriptData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miGenerateCSharp = new System.Windows.Forms.ToolStripMenuItem();
            this.miGenerateVB = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDDNodeView = new System.Windows.Forms.ToolStripDropDownButton();
            this.miUseGeneratedNames = new System.Windows.Forms.ToolStripMenuItem();
            this.miUseDatabaseNames = new System.Windows.Forms.ToolStripMenuItem();
            this.btnInvokeProviders = new System.Windows.Forms.ToolStripButton();
            this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.ilNodes = new System.Windows.Forms.ImageList(this.components);
            this.kryptonSplitContainer2 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this.tabMaster = new System.Windows.Forms.TabControl();
            this.tabMasterProperties = new System.Windows.Forms.TabPage();
            this.tabMasterScaffolds = new System.Windows.Forms.TabPage();
            this.webScaffolds = new System.Windows.Forms.WebBrowser();
            this.tabMasterAPIReference = new System.Windows.Forms.TabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.tabMasterForums = new System.Windows.Forms.TabPage();
            this.webBrowser2 = new System.Windows.Forms.WebBrowser();
            this.tabMasterWorkItems = new System.Windows.Forms.TabPage();
            this.webBrowser3 = new System.Windows.Forms.WebBrowser();
            this.ilTabs = new System.Windows.Forms.ImageList(this.components);
            this.tabDetail = new System.Windows.Forms.TabControl();
            this.tabDetailLog = new System.Windows.Forms.TabPage();
            this.tbxLog = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
            this.tsEventLog = new System.Windows.Forms.ToolStrip();
            this.btnClearLog = new System.Windows.Forms.ToolStripButton();
            this.tabDetailConfigOutput = new System.Windows.Forms.TabPage();
            this.tbxConfigOutput = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
            this.tsConfigFile = new System.Windows.Forms.ToolStrip();
            this.btnCopyConfig = new System.Windows.Forms.ToolStripButton();
            this.tabDetailFileBrowser = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeFileSystem = new System.Windows.Forms.TreeView();
            this.fileBrowser = new System.Windows.Forms.WebBrowser();
            this.cmTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiTreeDeleteProject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiTreeAddProvider = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiTreeDeleteProvider = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiTreeAddConnectionString = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiTreeDeleteConnectionString = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiGenerateObjectEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
            this.kryptonSplitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
            this.kryptonSplitContainer1.Panel2.SuspendLayout();
            this.kryptonSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel1)).BeginInit();
            this.kryptonSplitContainer2.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel2)).BeginInit();
            this.kryptonSplitContainer2.Panel2.SuspendLayout();
            this.kryptonSplitContainer2.SuspendLayout();
            this.tabMaster.SuspendLayout();
            this.tabMasterProperties.SuspendLayout();
            this.tabMasterScaffolds.SuspendLayout();
            this.tabMasterAPIReference.SuspendLayout();
            this.tabMasterForums.SuspendLayout();
            this.tabMasterWorkItems.SuspendLayout();
            this.tabDetail.SuspendLayout();
            this.tabDetailLog.SuspendLayout();
            this.tsEventLog.SuspendLayout();
            this.tabDetailConfigOutput.SuspendLayout();
            this.tsConfigFile.SuspendLayout();
            this.tabDetailFileBrowser.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.cmTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // pGrid
            // 
            // 
            // 
            // 
            this.pGrid.DocCommentDescription.AutoEllipsis = true;
            this.pGrid.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.pGrid.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.pGrid.DocCommentDescription.Name = "";
            this.pGrid.DocCommentDescription.Size = new System.Drawing.Size(617, 37);
            this.pGrid.DocCommentDescription.TabIndex = 1;
            this.pGrid.DocCommentImage = null;
            // 
            // 
            // 
            this.pGrid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.pGrid.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.pGrid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.pGrid.DocCommentTitle.Name = "";
            this.pGrid.DocCommentTitle.Size = new System.Drawing.Size(617, 15);
            this.pGrid.DocCommentTitle.TabIndex = 0;
            this.pGrid.DocCommentTitle.UseMnemonic = false;
            this.pGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pGrid.Location = new System.Drawing.Point(0, 0);
            this.pGrid.Margin = new System.Windows.Forms.Padding(0);
            this.pGrid.Name = "pGrid";
            this.pGrid.Size = new System.Drawing.Size(623, 263);
            this.pGrid.TabIndex = 0;
            // 
            // 
            // 
            this.pGrid.ToolStrip.AccessibleName = "ToolBar";
            this.pGrid.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.pGrid.ToolStrip.AllowMerge = false;
            this.pGrid.ToolStrip.AutoSize = false;
            this.pGrid.ToolStrip.CanOverflow = false;
            this.pGrid.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.pGrid.ToolStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pGrid.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pGrid.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pgbTestConnection});
            this.pGrid.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.pGrid.ToolStrip.Name = "";
            this.pGrid.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.pGrid.ToolStrip.Size = new System.Drawing.Size(623, 25);
            this.pGrid.ToolStrip.TabIndex = 1;
            this.pGrid.ToolStrip.TabStop = true;
            this.pGrid.ToolStrip.Text = "PropertyGridToolBar";
            this.pGrid.ToolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.pGrid_ToolStrip_ItemClicked);
            this.pGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridEx1_PropertyValueChanged);
            // 
            // pgbTestConnection
            // 
            this.pgbTestConnection.Image = ((System.Drawing.Image)(resources.GetObject("pgbTestConnection.Image")));
            this.pgbTestConnection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pgbTestConnection.Name = "pgbTestConnection";
            this.pgbTestConnection.Size = new System.Drawing.Size(114, 22);
            this.pgbTestConnection.Text = "Test Connection";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(816, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileImportProject,
            this.miFileExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // miFileImportProject
            // 
            this.miFileImportProject.Name = "miFileImportProject";
            this.miFileImportProject.Size = new System.Drawing.Size(159, 22);
            this.miFileImportProject.Text = "Import Project...";
            this.miFileImportProject.Click += new System.EventHandler(this.miFileImportProject_Click);
            // 
            // miFileExit
            // 
            this.miFileExit.Name = "miFileExit";
            this.miFileExit.Size = new System.Drawing.Size(159, 22);
            this.miFileExit.Text = "E&xit";
            this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHelpAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // miHelpAbout
            // 
            this.miHelpAbout.Name = "miHelpAbout";
            this.miHelpAbout.Size = new System.Drawing.Size(159, 22);
            this.miHelpAbout.Text = "&About SubStage";
            this.miHelpAbout.Click += new System.EventHandler(this.miHelpAbout_Click);
            // 
            // kryptonManager1
            // 
            this.kryptonManager1.GlobalPaletteMode = ComponentFactory.Krypton.Toolkit.PaletteModeManager.Office2007Black;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 513);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(816, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.Stretch = false;
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(801, 17);
            this.tsStatus.Spring = true;
            this.tsStatus.Text = "Ready";
            this.tsStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewProject,
            this.btnNewProvider,
            this.btnAddConnectionString,
            this.btnSplitGenerateCode,
            this.btnDDNodeView,
            this.btnInvokeProviders});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(180, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnNewProject
            // 
            this.btnNewProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNewProject.Image = ((System.Drawing.Image)(resources.GetObject("btnNewProject.Image")));
            this.btnNewProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewProject.Name = "btnNewProject";
            this.btnNewProject.Size = new System.Drawing.Size(23, 22);
            this.btnNewProject.Text = "New Project";
            this.btnNewProject.Click += new System.EventHandler(this.btnNewProject_Click);
            // 
            // btnNewProvider
            // 
            this.btnNewProvider.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNewProvider.Enabled = false;
            this.btnNewProvider.Image = ((System.Drawing.Image)(resources.GetObject("btnNewProvider.Image")));
            this.btnNewProvider.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewProvider.Name = "btnNewProvider";
            this.btnNewProvider.Size = new System.Drawing.Size(23, 22);
            this.btnNewProvider.Text = "New Provider";
            this.btnNewProvider.Click += new System.EventHandler(this.btnNewProvider_Click);
            // 
            // btnAddConnectionString
            // 
            this.btnAddConnectionString.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddConnectionString.Enabled = false;
            this.btnAddConnectionString.Image = ((System.Drawing.Image)(resources.GetObject("btnAddConnectionString.Image")));
            this.btnAddConnectionString.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddConnectionString.Name = "btnAddConnectionString";
            this.btnAddConnectionString.Size = new System.Drawing.Size(23, 22);
            this.btnAddConnectionString.Text = "New Connection String";
            this.btnAddConnectionString.Click += new System.EventHandler(this.btnAddConnectionString_Click);
            // 
            // btnSplitGenerateCode
            // 
            this.btnSplitGenerateCode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSplitGenerateCode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miScriptSchemas,
            this.miScriptData,
            this.toolStripSeparator3,
            this.miGenerateCSharp,
            this.miGenerateVB});
            this.btnSplitGenerateCode.Image = ((System.Drawing.Image)(resources.GetObject("btnSplitGenerateCode.Image")));
            this.btnSplitGenerateCode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSplitGenerateCode.Name = "btnSplitGenerateCode";
            this.btnSplitGenerateCode.Size = new System.Drawing.Size(32, 22);
            this.btnSplitGenerateCode.Text = "Generate Code";
            this.btnSplitGenerateCode.Click += new System.EventHandler(this.btnSplitGenerateCode_Click);
            this.btnSplitGenerateCode.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnSplitGenerateCode_DropDownItemClicked);
            // 
            // miScriptSchemas
            // 
            this.miScriptSchemas.CheckOnClick = true;
            this.miScriptSchemas.Name = "miScriptSchemas";
            this.miScriptSchemas.Size = new System.Drawing.Size(191, 22);
            this.miScriptSchemas.Text = "Script Schemas";
            // 
            // miScriptData
            // 
            this.miScriptData.CheckOnClick = true;
            this.miScriptData.Name = "miScriptData";
            this.miScriptData.Size = new System.Drawing.Size(191, 22);
            this.miScriptData.Text = "Script Data";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(188, 6);
            // 
            // miGenerateCSharp
            // 
            this.miGenerateCSharp.Checked = true;
            this.miGenerateCSharp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miGenerateCSharp.Name = "miGenerateCSharp";
            this.miGenerateCSharp.Size = new System.Drawing.Size(191, 22);
            this.miGenerateCSharp.Text = "Generate C# Code";
            // 
            // miGenerateVB
            // 
            this.miGenerateVB.Name = "miGenerateVB";
            this.miGenerateVB.Size = new System.Drawing.Size(191, 22);
            this.miGenerateVB.Text = "Generate VB.Net Code";
            // 
            // btnDDNodeView
            // 
            this.btnDDNodeView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDDNodeView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miUseGeneratedNames,
            this.miUseDatabaseNames});
            this.btnDDNodeView.Image = ((System.Drawing.Image)(resources.GetObject("btnDDNodeView.Image")));
            this.btnDDNodeView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDDNodeView.Name = "btnDDNodeView";
            this.btnDDNodeView.Size = new System.Drawing.Size(29, 22);
            this.btnDDNodeView.Text = "Node View";
            // 
            // miUseGeneratedNames
            // 
            this.miUseGeneratedNames.Checked = true;
            this.miUseGeneratedNames.CheckOnClick = true;
            this.miUseGeneratedNames.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miUseGeneratedNames.Name = "miUseGeneratedNames";
            this.miUseGeneratedNames.Size = new System.Drawing.Size(190, 22);
            this.miUseGeneratedNames.Text = "Use Generated Names";
            this.miUseGeneratedNames.Click += new System.EventHandler(this.miUseGeneratedNames_Click);
            // 
            // miUseDatabaseNames
            // 
            this.miUseDatabaseNames.CheckOnClick = true;
            this.miUseDatabaseNames.Name = "miUseDatabaseNames";
            this.miUseDatabaseNames.Size = new System.Drawing.Size(190, 22);
            this.miUseDatabaseNames.Text = "Use Database Names";
            this.miUseDatabaseNames.Click += new System.EventHandler(this.miUseDatabaseNames_Click);
            // 
            // btnInvokeProviders
            // 
            this.btnInvokeProviders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnInvokeProviders.Enabled = false;
            this.btnInvokeProviders.Image = ((System.Drawing.Image)(resources.GetObject("btnInvokeProviders.Image")));
            this.btnInvokeProviders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnInvokeProviders.Name = "btnInvokeProviders";
            this.btnInvokeProviders.Size = new System.Drawing.Size(23, 22);
            this.btnInvokeProviders.Text = "Invoke Providers";
            this.btnInvokeProviders.Click += new System.EventHandler(this.btnInvokeProviders_Click);
            // 
            // kryptonSplitContainer1
            // 
            this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.kryptonSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonSplitContainer1.Location = new System.Drawing.Point(0, 24);
            this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
            // 
            // kryptonSplitContainer1.Panel1
            // 
            this.kryptonSplitContainer1.Panel1.Controls.Add(this.treeView1);
            this.kryptonSplitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // kryptonSplitContainer1.Panel2
            // 
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonSplitContainer2);
            this.kryptonSplitContainer1.Size = new System.Drawing.Size(816, 489);
            this.kryptonSplitContainer1.SplitterDistance = 180;
            this.kryptonSplitContainer1.TabIndex = 3;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.Control;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.HotTracking = true;
            this.treeView1.ImageKey = "box.png";
            this.treeView1.ImageList = this.ilNodes;
            this.treeView1.Indent = 10;
            this.treeView1.LineColor = System.Drawing.Color.DarkGray;
            this.treeView1.Location = new System.Drawing.Point(0, 25);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(180, 464);
            this.treeView1.StateImageList = this.ilNodes;
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // ilNodes
            // 
            this.ilNodes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilNodes.ImageStream")));
            this.ilNodes.TransparentColor = System.Drawing.Color.Transparent;
            this.ilNodes.Images.SetKeyName(0, "package.png");
            this.ilNodes.Images.SetKeyName(1, "database.png");
            this.ilNodes.Images.SetKeyName(2, "folder.png");
            this.ilNodes.Images.SetKeyName(3, "table.png");
            this.ilNodes.Images.SetKeyName(4, "database_table.png");
            this.ilNodes.Images.SetKeyName(5, "database_gear.png");
            this.ilNodes.Images.SetKeyName(6, "folder_link.png");
            this.ilNodes.Images.SetKeyName(7, "link.png");
            this.ilNodes.Images.SetKeyName(8, "textfield.png");
            this.ilNodes.Images.SetKeyName(9, "textfield_key.png");
            this.ilNodes.Images.SetKeyName(10, "table_error.png");
            this.ilNodes.Images.SetKeyName(11, "script_gear.png");
            this.ilNodes.Images.SetKeyName(12, "table_delete.png");
            this.ilNodes.Images.SetKeyName(13, "drive.png");
            this.ilNodes.Images.SetKeyName(14, "brick.png");
            // 
            // kryptonSplitContainer2
            // 
            this.kryptonSplitContainer2.Cursor = System.Windows.Forms.Cursors.Default;
            this.kryptonSplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonSplitContainer2.Location = new System.Drawing.Point(0, 0);
            this.kryptonSplitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.kryptonSplitContainer2.Name = "kryptonSplitContainer2";
            this.kryptonSplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // kryptonSplitContainer2.Panel1
            // 
            this.kryptonSplitContainer2.Panel1.Controls.Add(this.tabMaster);
            // 
            // kryptonSplitContainer2.Panel2
            // 
            this.kryptonSplitContainer2.Panel2.Controls.Add(this.tabDetail);
            this.kryptonSplitContainer2.Size = new System.Drawing.Size(631, 489);
            this.kryptonSplitContainer2.SplitterDistance = 290;
            this.kryptonSplitContainer2.TabIndex = 0;
            // 
            // tabMaster
            // 
            this.tabMaster.Controls.Add(this.tabMasterProperties);
            this.tabMaster.Controls.Add(this.tabMasterScaffolds);
            this.tabMaster.Controls.Add(this.tabMasterAPIReference);
            this.tabMaster.Controls.Add(this.tabMasterForums);
            this.tabMaster.Controls.Add(this.tabMasterWorkItems);
            this.tabMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMaster.ImageList = this.ilTabs;
            this.tabMaster.ItemSize = new System.Drawing.Size(81, 19);
            this.tabMaster.Location = new System.Drawing.Point(0, 0);
            this.tabMaster.Margin = new System.Windows.Forms.Padding(0);
            this.tabMaster.Name = "tabMaster";
            this.tabMaster.SelectedIndex = 0;
            this.tabMaster.Size = new System.Drawing.Size(631, 290);
            this.tabMaster.TabIndex = 0;
            this.tabMaster.SelectedIndexChanged += new System.EventHandler(this.tabMaster_SelectedIndexChanged);
            // 
            // tabMasterProperties
            // 
            this.tabMasterProperties.Controls.Add(this.pGrid);
            this.tabMasterProperties.ImageKey = "application_side_list.png";
            this.tabMasterProperties.Location = new System.Drawing.Point(4, 23);
            this.tabMasterProperties.Name = "tabMasterProperties";
            this.tabMasterProperties.Size = new System.Drawing.Size(623, 263);
            this.tabMasterProperties.TabIndex = 0;
            this.tabMasterProperties.Text = "Properties";
            this.tabMasterProperties.UseVisualStyleBackColor = true;
            // 
            // tabMasterScaffolds
            // 
            this.tabMasterScaffolds.Controls.Add(this.webScaffolds);
            this.tabMasterScaffolds.ImageIndex = 8;
            this.tabMasterScaffolds.Location = new System.Drawing.Point(4, 23);
            this.tabMasterScaffolds.Name = "tabMasterScaffolds";
            this.tabMasterScaffolds.Size = new System.Drawing.Size(623, 263);
            this.tabMasterScaffolds.TabIndex = 4;
            this.tabMasterScaffolds.Text = "Scaffolds";
            this.tabMasterScaffolds.UseVisualStyleBackColor = true;
            // 
            // webScaffolds
            // 
            this.webScaffolds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webScaffolds.Location = new System.Drawing.Point(0, 0);
            this.webScaffolds.MinimumSize = new System.Drawing.Size(20, 20);
            this.webScaffolds.Name = "webScaffolds";
            this.webScaffolds.Size = new System.Drawing.Size(623, 263);
            this.webScaffolds.TabIndex = 0;
            // 
            // tabMasterAPIReference
            // 
            this.tabMasterAPIReference.Controls.Add(this.webBrowser1);
            this.tabMasterAPIReference.ImageKey = "book_addresses.png";
            this.tabMasterAPIReference.Location = new System.Drawing.Point(4, 23);
            this.tabMasterAPIReference.Name = "tabMasterAPIReference";
            this.tabMasterAPIReference.Size = new System.Drawing.Size(623, 263);
            this.tabMasterAPIReference.TabIndex = 1;
            this.tabMasterAPIReference.Text = "API Reference";
            this.tabMasterAPIReference.UseVisualStyleBackColor = true;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(623, 263);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Url = new System.Uri("http://subsonichelp.com/", System.UriKind.Absolute);
            // 
            // tabMasterForums
            // 
            this.tabMasterForums.Controls.Add(this.webBrowser2);
            this.tabMasterForums.ImageIndex = 2;
            this.tabMasterForums.Location = new System.Drawing.Point(4, 23);
            this.tabMasterForums.Name = "tabMasterForums";
            this.tabMasterForums.Size = new System.Drawing.Size(623, 263);
            this.tabMasterForums.TabIndex = 2;
            this.tabMasterForums.Text = "Forums";
            this.tabMasterForums.UseVisualStyleBackColor = true;
            // 
            // webBrowser2
            // 
            this.webBrowser2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser2.Location = new System.Drawing.Point(0, 0);
            this.webBrowser2.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser2.Name = "webBrowser2";
            this.webBrowser2.ScriptErrorsSuppressed = true;
            this.webBrowser2.Size = new System.Drawing.Size(623, 263);
            this.webBrowser2.TabIndex = 1;
            this.webBrowser2.Url = new System.Uri("http://forums.subsonicproject.com/forums/TopicsActive.aspx", System.UriKind.Absolute);
            // 
            // tabMasterWorkItems
            // 
            this.tabMasterWorkItems.Controls.Add(this.webBrowser3);
            this.tabMasterWorkItems.ImageIndex = 3;
            this.tabMasterWorkItems.Location = new System.Drawing.Point(4, 23);
            this.tabMasterWorkItems.Margin = new System.Windows.Forms.Padding(0);
            this.tabMasterWorkItems.Name = "tabMasterWorkItems";
            this.tabMasterWorkItems.Size = new System.Drawing.Size(623, 263);
            this.tabMasterWorkItems.TabIndex = 3;
            this.tabMasterWorkItems.Text = "Work Items";
            this.tabMasterWorkItems.UseVisualStyleBackColor = true;
            // 
            // webBrowser3
            // 
            this.webBrowser3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser3.Location = new System.Drawing.Point(0, 0);
            this.webBrowser3.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser3.Name = "webBrowser3";
            this.webBrowser3.ScriptErrorsSuppressed = true;
            this.webBrowser3.Size = new System.Drawing.Size(623, 263);
            this.webBrowser3.TabIndex = 1;
            this.webBrowser3.Url = new System.Uri("http://www.codeplex.com/subsonic/WorkItem/List.aspx", System.UriKind.Absolute);
            // 
            // ilTabs
            // 
            this.ilTabs.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTabs.ImageStream")));
            this.ilTabs.TransparentColor = System.Drawing.Color.Transparent;
            this.ilTabs.Images.SetKeyName(0, "application_side_list.png");
            this.ilTabs.Images.SetKeyName(1, "book_addresses.png");
            this.ilTabs.Images.SetKeyName(2, "group.png");
            this.ilTabs.Images.SetKeyName(3, "bug.png");
            this.ilTabs.Images.SetKeyName(4, "script_gear.png");
            this.ilTabs.Images.SetKeyName(5, "page_white_visualstudio.png");
            this.ilTabs.Images.SetKeyName(6, "page.png");
            this.ilTabs.Images.SetKeyName(7, "page_white_delete.png");
            this.ilTabs.Images.SetKeyName(8, "application_form_edit.png");
            // 
            // tabDetail
            // 
            this.tabDetail.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabDetail.Controls.Add(this.tabDetailLog);
            this.tabDetail.Controls.Add(this.tabDetailConfigOutput);
            this.tabDetail.Controls.Add(this.tabDetailFileBrowser);
            this.tabDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDetail.ImageList = this.ilTabs;
            this.tabDetail.Location = new System.Drawing.Point(0, 0);
            this.tabDetail.Margin = new System.Windows.Forms.Padding(0);
            this.tabDetail.Name = "tabDetail";
            this.tabDetail.SelectedIndex = 0;
            this.tabDetail.Size = new System.Drawing.Size(631, 194);
            this.tabDetail.TabIndex = 0;
            // 
            // tabDetailLog
            // 
            this.tabDetailLog.Controls.Add(this.tbxLog);
            this.tabDetailLog.Controls.Add(this.tsEventLog);
            this.tabDetailLog.ImageKey = "page.png";
            this.tabDetailLog.Location = new System.Drawing.Point(4, 4);
            this.tabDetailLog.Name = "tabDetailLog";
            this.tabDetailLog.Size = new System.Drawing.Size(623, 167);
            this.tabDetailLog.TabIndex = 2;
            this.tabDetailLog.Text = "Event Log";
            this.tabDetailLog.UseVisualStyleBackColor = true;
            // 
            // tbxLog
            // 
            this.tbxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxLog.Location = new System.Drawing.Point(0, 25);
            this.tbxLog.Name = "tbxLog";
            this.tbxLog.ReadOnly = true;
            this.tbxLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.tbxLog.Size = new System.Drawing.Size(623, 142);
            this.tbxLog.StateCommon.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left;
            this.tbxLog.StateCommon.Content.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxLog.TabIndex = 1;
            this.tbxLog.WordWrap = false;
            this.tbxLog.TextChanged += new System.EventHandler(this.tbxLog_TextChanged);
            // 
            // tsEventLog
            // 
            this.tsEventLog.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tsEventLog.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tsEventLog.GripMargin = new System.Windows.Forms.Padding(0);
            this.tsEventLog.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsEventLog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnClearLog});
            this.tsEventLog.Location = new System.Drawing.Point(0, 0);
            this.tsEventLog.Name = "tsEventLog";
            this.tsEventLog.Padding = new System.Windows.Forms.Padding(0);
            this.tsEventLog.Size = new System.Drawing.Size(623, 25);
            this.tsEventLog.Stretch = true;
            this.tsEventLog.TabIndex = 0;
            this.tsEventLog.Text = "toolStrip2";
            // 
            // btnClearLog
            // 
            this.btnClearLog.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnClearLog.Image = ((System.Drawing.Image)(resources.GetObject("btnClearLog.Image")));
            this.btnClearLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(77, 22);
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // tabDetailConfigOutput
            // 
            this.tabDetailConfigOutput.Controls.Add(this.tbxConfigOutput);
            this.tabDetailConfigOutput.Controls.Add(this.tsConfigFile);
            this.tabDetailConfigOutput.ImageKey = "script_gear.png";
            this.tabDetailConfigOutput.Location = new System.Drawing.Point(4, 4);
            this.tabDetailConfigOutput.Name = "tabDetailConfigOutput";
            this.tabDetailConfigOutput.Size = new System.Drawing.Size(623, 167);
            this.tabDetailConfigOutput.TabIndex = 0;
            this.tabDetailConfigOutput.Text = "Configuration Output";
            this.tabDetailConfigOutput.UseVisualStyleBackColor = true;
            // 
            // tbxConfigOutput
            // 
            this.tbxConfigOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxConfigOutput.Location = new System.Drawing.Point(0, 25);
            this.tbxConfigOutput.Margin = new System.Windows.Forms.Padding(0);
            this.tbxConfigOutput.Name = "tbxConfigOutput";
            this.tbxConfigOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.tbxConfigOutput.Size = new System.Drawing.Size(623, 142);
            this.tbxConfigOutput.StateCommon.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left;
            this.tbxConfigOutput.StateCommon.Content.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxConfigOutput.TabIndex = 2;
            this.tbxConfigOutput.WordWrap = false;
            // 
            // tsConfigFile
            // 
            this.tsConfigFile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tsConfigFile.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsConfigFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCopyConfig});
            this.tsConfigFile.Location = new System.Drawing.Point(0, 0);
            this.tsConfigFile.Name = "tsConfigFile";
            this.tsConfigFile.Padding = new System.Windows.Forms.Padding(0);
            this.tsConfigFile.Size = new System.Drawing.Size(623, 25);
            this.tsConfigFile.TabIndex = 1;
            this.tsConfigFile.Text = "toolStrip2";
            // 
            // btnCopyConfig
            // 
            this.btnCopyConfig.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnCopyConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyConfig.Image")));
            this.btnCopyConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopyConfig.Name = "btnCopyConfig";
            this.btnCopyConfig.Size = new System.Drawing.Size(124, 22);
            this.btnCopyConfig.Text = "Copy to Clipboard";
            this.btnCopyConfig.Click += new System.EventHandler(this.btnCopyConfig_Click);
            // 
            // tabDetailFileBrowser
            // 
            this.tabDetailFileBrowser.BackColor = System.Drawing.Color.Transparent;
            this.tabDetailFileBrowser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabDetailFileBrowser.Controls.Add(this.splitContainer1);
            this.tabDetailFileBrowser.ImageKey = "page_white_visualstudio.png";
            this.tabDetailFileBrowser.Location = new System.Drawing.Point(4, 4);
            this.tabDetailFileBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.tabDetailFileBrowser.Name = "tabDetailFileBrowser";
            this.tabDetailFileBrowser.Size = new System.Drawing.Size(623, 167);
            this.tabDetailFileBrowser.TabIndex = 1;
            this.tabDetailFileBrowser.Text = "Generated Files";
            this.tabDetailFileBrowser.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.DarkGray;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeFileSystem);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.fileBrowser);
            this.splitContainer1.Size = new System.Drawing.Size(623, 167);
            this.splitContainer1.SplitterDistance = 121;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeFileSystem
            // 
            this.treeFileSystem.BackColor = System.Drawing.SystemColors.Control;
            this.treeFileSystem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeFileSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFileSystem.HideSelection = false;
            this.treeFileSystem.HotTracking = true;
            this.treeFileSystem.ImageKey = "drive.png";
            this.treeFileSystem.ImageList = this.ilNodes;
            this.treeFileSystem.Indent = 10;
            this.treeFileSystem.LineColor = System.Drawing.Color.LightGray;
            this.treeFileSystem.Location = new System.Drawing.Point(0, 0);
            this.treeFileSystem.Margin = new System.Windows.Forms.Padding(0);
            this.treeFileSystem.Name = "treeFileSystem";
            this.treeFileSystem.SelectedImageIndex = 0;
            this.treeFileSystem.Size = new System.Drawing.Size(121, 167);
            this.treeFileSystem.TabIndex = 0;
            this.treeFileSystem.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeFileSystem_BeforeExpand);
            this.treeFileSystem.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeFileSystem_AfterSelect);
            // 
            // fileBrowser
            // 
            this.fileBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileBrowser.Location = new System.Drawing.Point(0, 0);
            this.fileBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.fileBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.fileBrowser.Name = "fileBrowser";
            this.fileBrowser.Size = new System.Drawing.Size(498, 167);
            this.fileBrowser.TabIndex = 0;
            this.fileBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.fileBrowser_Navigating);
            this.fileBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.fileBrowser_Navigated);
            // 
            // cmTree
            // 
            this.cmTree.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.cmTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiTreeDeleteProject,
            this.toolStripSeparator1,
            this.cmiTreeAddProvider,
            this.cmiTreeDeleteProvider,
            this.toolStripSeparator2,
            this.cmiTreeAddConnectionString,
            this.cmiTreeDeleteConnectionString,
            this.toolStripMenuItem1,
            this.cmiGenerateObjectEnabled});
            this.cmTree.Name = "cmTree";
            this.cmTree.Size = new System.Drawing.Size(205, 154);
            this.cmTree.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmTree_ItemClicked);
            // 
            // cmiTreeDeleteProject
            // 
            this.cmiTreeDeleteProject.Image = ((System.Drawing.Image)(resources.GetObject("cmiTreeDeleteProject.Image")));
            this.cmiTreeDeleteProject.Name = "cmiTreeDeleteProject";
            this.cmiTreeDeleteProject.Size = new System.Drawing.Size(204, 22);
            this.cmiTreeDeleteProject.Text = "Delete Project";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
            // 
            // cmiTreeAddProvider
            // 
            this.cmiTreeAddProvider.Image = ((System.Drawing.Image)(resources.GetObject("cmiTreeAddProvider.Image")));
            this.cmiTreeAddProvider.Name = "cmiTreeAddProvider";
            this.cmiTreeAddProvider.Size = new System.Drawing.Size(204, 22);
            this.cmiTreeAddProvider.Text = "Add Provider";
            // 
            // cmiTreeDeleteProvider
            // 
            this.cmiTreeDeleteProvider.Image = ((System.Drawing.Image)(resources.GetObject("cmiTreeDeleteProvider.Image")));
            this.cmiTreeDeleteProvider.Name = "cmiTreeDeleteProvider";
            this.cmiTreeDeleteProvider.Size = new System.Drawing.Size(204, 22);
            this.cmiTreeDeleteProvider.Text = "Delete Provider";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(201, 6);
            // 
            // cmiTreeAddConnectionString
            // 
            this.cmiTreeAddConnectionString.Image = ((System.Drawing.Image)(resources.GetObject("cmiTreeAddConnectionString.Image")));
            this.cmiTreeAddConnectionString.Name = "cmiTreeAddConnectionString";
            this.cmiTreeAddConnectionString.Size = new System.Drawing.Size(204, 22);
            this.cmiTreeAddConnectionString.Text = "Add Connection String";
            // 
            // cmiTreeDeleteConnectionString
            // 
            this.cmiTreeDeleteConnectionString.Image = ((System.Drawing.Image)(resources.GetObject("cmiTreeDeleteConnectionString.Image")));
            this.cmiTreeDeleteConnectionString.Name = "cmiTreeDeleteConnectionString";
            this.cmiTreeDeleteConnectionString.Size = new System.Drawing.Size(204, 22);
            this.cmiTreeDeleteConnectionString.Text = "Delete Connection String";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(201, 6);
            // 
            // cmiGenerateObjectEnabled
            // 
            this.cmiGenerateObjectEnabled.Name = "cmiGenerateObjectEnabled";
            this.cmiGenerateObjectEnabled.Size = new System.Drawing.Size(204, 22);
            this.cmiGenerateObjectEnabled.Text = "Generate Object?";
            // 
            // SubStageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 535);
            this.Controls.Add(this.kryptonSplitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SubStageForm";
            this.Text = "SubStage";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SubStageForm_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
            this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
            this.kryptonSplitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
            this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
            this.kryptonSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel1)).EndInit();
            this.kryptonSplitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel2)).EndInit();
            this.kryptonSplitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2)).EndInit();
            this.kryptonSplitContainer2.ResumeLayout(false);
            this.tabMaster.ResumeLayout(false);
            this.tabMasterProperties.ResumeLayout(false);
            this.tabMasterScaffolds.ResumeLayout(false);
            this.tabMasterAPIReference.ResumeLayout(false);
            this.tabMasterForums.ResumeLayout(false);
            this.tabMasterWorkItems.ResumeLayout(false);
            this.tabDetail.ResumeLayout(false);
            this.tabDetailLog.ResumeLayout(false);
            this.tabDetailLog.PerformLayout();
            this.tsEventLog.ResumeLayout(false);
            this.tsEventLog.PerformLayout();
            this.tabDetailConfigOutput.ResumeLayout(false);
            this.tabDetailConfigOutput.PerformLayout();
            this.tsConfigFile.ResumeLayout(false);
            this.tsConfigFile.PerformLayout();
            this.tabDetailFileBrowser.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.cmTree.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer2;
        private System.Windows.Forms.TabControl tabMaster;
        private System.Windows.Forms.TabPage tabMasterProperties;
        private System.Windows.Forms.TabControl tabDetail;
        private System.Windows.Forms.ImageList ilNodes;
        private System.Windows.Forms.ToolStripMenuItem miFileExit;
        private System.Windows.Forms.ToolStripMenuItem miHelpAbout;
        private System.Windows.Forms.ToolStripButton btnNewProject;
        private System.Windows.Forms.ToolStripButton btnNewProvider;
        private System.Windows.Forms.TabPage tabMasterAPIReference;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.TabPage tabMasterForums;
        private System.Windows.Forms.TabPage tabMasterWorkItems;
        private System.Windows.Forms.WebBrowser webBrowser2;
        private System.Windows.Forms.WebBrowser webBrowser3;
        private System.Windows.Forms.ContextMenuStrip cmTree;
        private System.Windows.Forms.ToolStripMenuItem cmiTreeDeleteProject;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cmiTreeAddProvider;
        private System.Windows.Forms.ToolStripMenuItem cmiTreeDeleteProvider;
        private System.Windows.Forms.ToolStripButton btnAddConnectionString;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cmiTreeAddConnectionString;
        private System.Windows.Forms.ToolStripMenuItem cmiTreeDeleteConnectionString;
        private System.Windows.Forms.ToolStripButton btnInvokeProviders;
        private System.Windows.Forms.ToolStripMenuItem miFileImportProject;
        private PropertyGridEx.PropertyGridEx pGrid;
        private System.Windows.Forms.TabPage tabDetailConfigOutput;
        private System.Windows.Forms.ImageList ilTabs;
        private System.Windows.Forms.TabPage tabDetailFileBrowser;
        private System.Windows.Forms.WebBrowser fileBrowser;
        private System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.ToolStripButton pgbTestConnection;
        private System.Windows.Forms.ToolStripSplitButton btnSplitGenerateCode;
        private System.Windows.Forms.ToolStripMenuItem miScriptSchemas;
        private System.Windows.Forms.ToolStripMenuItem miScriptData;
        private System.Windows.Forms.ToolStripDropDownButton btnDDNodeView;
        private System.Windows.Forms.ToolStripMenuItem miUseGeneratedNames;
        private System.Windows.Forms.ToolStripMenuItem miUseDatabaseNames;
        private System.Windows.Forms.ToolStrip tsConfigFile;
        private System.Windows.Forms.ToolStripButton btnCopyConfig;
        private System.Windows.Forms.TabPage tabDetailLog;
        private System.Windows.Forms.ToolStrip tsEventLog;
        private System.Windows.Forms.ToolStripButton btnClearLog;
        private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox tbxLog;
        private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox tbxConfigOutput;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem miGenerateCSharp;
        private System.Windows.Forms.ToolStripMenuItem miGenerateVB;
        private System.Windows.Forms.TabPage tabMasterScaffolds;
        private System.Windows.Forms.WebBrowser webScaffolds;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cmiGenerateObjectEnabled;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeFileSystem;

    }
}


