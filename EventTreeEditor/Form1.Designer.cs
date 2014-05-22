namespace EventTreeEditor
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.saveCurExrLocal = new System.Windows.Forms.ToolStripButton();
            this.normGraphSB = new System.Windows.Forms.ToolStripButton();
            this.syncSB = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.main_panel = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ExerciseGroupsBox = new System.Windows.Forms.ComboBox();
            this.CategoriesBox = new System.Windows.Forms.ComboBox();
            this.mainTV = new System.Windows.Forms.TreeView();
            this.mainField = new System.Windows.Forms.PictureBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.dgProp = new System.Windows.Forms.DataGridView();
            this.subTV = new System.Windows.Forms.TreeView();
            this.cms = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editPropetiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.editPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simpleConditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSubtreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeUniqueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsTV = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.main_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgProp)).BeginInit();
            this.cms.SuspendLayout();
            this.cmsTV.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveCurExrLocal,
            this.normGraphSB,
            this.syncSB});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1620, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // saveCurExrLocal
            // 
            this.saveCurExrLocal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveCurExrLocal.Image = ((System.Drawing.Image)(resources.GetObject("saveCurExrLocal.Image")));
            this.saveCurExrLocal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveCurExrLocal.Name = "saveCurExrLocal";
            this.saveCurExrLocal.Size = new System.Drawing.Size(23, 22);
            this.saveCurExrLocal.Text = "Save current graph to local DB";
            this.saveCurExrLocal.Click += new System.EventHandler(this.saveCurExrLocal_Click);
            // 
            // normGraphSB
            // 
            this.normGraphSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.normGraphSB.Image = ((System.Drawing.Image)(resources.GetObject("normGraphSB.Image")));
            this.normGraphSB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.normGraphSB.Name = "normGraphSB";
            this.normGraphSB.Size = new System.Drawing.Size(23, 22);
            this.normGraphSB.Text = "Graph normalization";
            this.normGraphSB.Click += new System.EventHandler(this.toolStripButton1_Click_1);
            // 
            // syncSB
            // 
            this.syncSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.syncSB.Image = ((System.Drawing.Image)(resources.GetObject("syncSB.Image")));
            this.syncSB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.syncSB.Name = "syncSB";
            this.syncSB.Size = new System.Drawing.Size(23, 22);
            this.syncSB.Text = "Synchronization with DB";
            this.syncSB.ToolTipText = "Synchronization with DB";
            this.syncSB.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(0, 52);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.main_panel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(1616, 808);
            this.splitContainer1.SplitterDistance = 1316;
            this.splitContainer1.TabIndex = 7;
            // 
            // main_panel
            // 
            this.main_panel.AutoScroll = true;
            this.main_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.main_panel.Controls.Add(this.splitContainer2);
            this.main_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.main_panel.Location = new System.Drawing.Point(0, 0);
            this.main_panel.Margin = new System.Windows.Forms.Padding(4);
            this.main_panel.Name = "main_panel";
            this.main_panel.Size = new System.Drawing.Size(1316, 808);
            this.main_panel.TabIndex = 6;
            this.main_panel.MouseEnter += new System.EventHandler(this.main_panel_MouseEnter);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.mainField);
            this.splitContainer2.Size = new System.Drawing.Size(1312, 804);
            this.splitContainer2.SplitterDistance = 433;
            this.splitContainer2.TabIndex = 4;
            this.splitContainer2.Resize += new System.EventHandler(this.splitContainer2_Resize);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ExerciseGroupsBox);
            this.panel1.Controls.Add(this.CategoriesBox);
            this.panel1.Controls.Add(this.mainTV);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(433, 804);
            this.panel1.TabIndex = 0;
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // ExerciseGroupsBox
            // 
            this.ExerciseGroupsBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ExerciseGroupsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ExerciseGroupsBox.FormattingEnabled = true;
            this.ExerciseGroupsBox.Location = new System.Drawing.Point(0, 33);
            this.ExerciseGroupsBox.Margin = new System.Windows.Forms.Padding(4);
            this.ExerciseGroupsBox.Name = "ExerciseGroupsBox";
            this.ExerciseGroupsBox.Size = new System.Drawing.Size(433, 33);
            this.ExerciseGroupsBox.TabIndex = 2;
            this.ExerciseGroupsBox.DropDown += new System.EventHandler(this.ExerciseGroupsBox_DropDown);
            this.ExerciseGroupsBox.SelectedIndexChanged += new System.EventHandler(this.ExerciseGroupsBox_SelectedIndexChanged);
            // 
            // CategoriesBox
            // 
            this.CategoriesBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.CategoriesBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CategoriesBox.FormattingEnabled = true;
            this.CategoriesBox.Location = new System.Drawing.Point(0, 0);
            this.CategoriesBox.Margin = new System.Windows.Forms.Padding(4);
            this.CategoriesBox.Name = "CategoriesBox";
            this.CategoriesBox.Size = new System.Drawing.Size(433, 33);
            this.CategoriesBox.TabIndex = 1;
            this.CategoriesBox.SelectedIndexChanged += new System.EventHandler(this.CategoriesBox_SelectedIndexChanged);
            // 
            // mainTV
            // 
            this.mainTV.Location = new System.Drawing.Point(24, 171);
            this.mainTV.Margin = new System.Windows.Forms.Padding(4);
            this.mainTV.Name = "mainTV";
            this.mainTV.Size = new System.Drawing.Size(120, 525);
            this.mainTV.TabIndex = 0;
            this.mainTV.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView2_AfterSelect);
            this.mainTV.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.mainTV_NodeMouseClick);
            this.mainTV.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mainTV_MouseClick);
            this.mainTV.MouseEnter += new System.EventHandler(this.treeView2_MouseEnter);
            // 
            // mainField
            // 
            this.mainField.BackColor = System.Drawing.Color.White;
            this.mainField.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mainField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainField.Location = new System.Drawing.Point(0, 0);
            this.mainField.Margin = new System.Windows.Forms.Padding(4);
            this.mainField.Name = "mainField";
            this.mainField.Size = new System.Drawing.Size(875, 804);
            this.mainField.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.mainField.TabIndex = 6;
            this.mainField.TabStop = false;
            this.mainField.Click += new System.EventHandler(this.mainField_Click);
            this.mainField.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainField_MouseDown);
            this.mainField.MouseEnter += new System.EventHandler(this.main_panel_MouseEnter);
            this.mainField.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mainField_MouseMove);
            this.mainField.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mainField_MouseUp);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.dgProp);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.subTV);
            this.splitContainer3.Size = new System.Drawing.Size(296, 808);
            this.splitContainer3.SplitterDistance = 400;
            this.splitContainer3.TabIndex = 6;
            // 
            // dgProp
            // 
            this.dgProp.AllowUserToAddRows = false;
            this.dgProp.AllowUserToDeleteRows = false;
            this.dgProp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgProp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgProp.Location = new System.Drawing.Point(0, 0);
            this.dgProp.Margin = new System.Windows.Forms.Padding(4);
            this.dgProp.Name = "dgProp";
            this.dgProp.RowHeadersVisible = false;
            this.dgProp.RowTemplate.Height = 33;
            this.dgProp.Size = new System.Drawing.Size(296, 400);
            this.dgProp.TabIndex = 0;
            this.dgProp.MouseEnter += new System.EventHandler(this.dataGridView1_MouseEnter);
            // 
            // subTV
            // 
            this.subTV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subTV.Location = new System.Drawing.Point(0, 0);
            this.subTV.Margin = new System.Windows.Forms.Padding(4);
            this.subTV.Name = "subTV";
            this.subTV.Size = new System.Drawing.Size(296, 404);
            this.subTV.TabIndex = 1;
            this.subTV.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.subTV.MouseEnter += new System.EventHandler(this.treeView1_MouseEnter);
            // 
            // cms
            // 
            this.cms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editPropetiesToolStripMenuItem,
            this.editPropertiesToolStripMenuItem,
            this.editLabelToolStripMenuItem,
            this.makeUniqueToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.cms.Name = "contextMenuStrip1";
            this.cms.Size = new System.Drawing.Size(254, 248);
            this.cms.Opening += new System.ComponentModel.CancelEventHandler(this.cms_Opening);
            // 
            // editPropetiesToolStripMenuItem
            // 
            this.editPropetiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.editPropetiesToolStripMenuItem.Name = "editPropetiesToolStripMenuItem";
            this.editPropetiesToolStripMenuItem.Size = new System.Drawing.Size(253, 40);
            this.editPropetiesToolStripMenuItem.Text = "Operand";
            this.editPropetiesToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editPropetiesToolStripMenuItem_DropDownOpening);
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 43);
            // 
            // editPropertiesToolStripMenuItem
            // 
            this.editPropertiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.simpleConditionToolStripMenuItem,
            this.addSubtreeToolStripMenuItem});
            this.editPropertiesToolStripMenuItem.Name = "editPropertiesToolStripMenuItem";
            this.editPropertiesToolStripMenuItem.Size = new System.Drawing.Size(253, 40);
            this.editPropertiesToolStripMenuItem.Text = "Edit properties";
            this.editPropertiesToolStripMenuItem.Click += new System.EventHandler(this.editPropertiesToolStripMenuItem_Click);
            // 
            // simpleConditionToolStripMenuItem
            // 
            this.simpleConditionToolStripMenuItem.Name = "simpleConditionToolStripMenuItem";
            this.simpleConditionToolStripMenuItem.Size = new System.Drawing.Size(328, 40);
            this.simpleConditionToolStripMenuItem.Text = "Add simple condition";
            this.simpleConditionToolStripMenuItem.Click += new System.EventHandler(this.simpleConditionToolStripMenuItem_Click);
            // 
            // addSubtreeToolStripMenuItem
            // 
            this.addSubtreeToolStripMenuItem.Name = "addSubtreeToolStripMenuItem";
            this.addSubtreeToolStripMenuItem.Size = new System.Drawing.Size(328, 40);
            this.addSubtreeToolStripMenuItem.Text = "Add subtree";
            this.addSubtreeToolStripMenuItem.Click += new System.EventHandler(this.addSubtreeToolStripMenuItem_Click);
            // 
            // editLabelToolStripMenuItem
            // 
            this.editLabelToolStripMenuItem.Name = "editLabelToolStripMenuItem";
            this.editLabelToolStripMenuItem.Size = new System.Drawing.Size(253, 40);
            this.editLabelToolStripMenuItem.Text = "Edit label";
            this.editLabelToolStripMenuItem.Click += new System.EventHandler(this.editLabelToolStripMenuItem_Click);
            // 
            // makeUniqueToolStripMenuItem
            // 
            this.makeUniqueToolStripMenuItem.Name = "makeUniqueToolStripMenuItem";
            this.makeUniqueToolStripMenuItem.Size = new System.Drawing.Size(253, 40);
            this.makeUniqueToolStripMenuItem.Text = "Make unique";
            this.makeUniqueToolStripMenuItem.Click += new System.EventHandler(this.makeUniqueToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(253, 40);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // cmsTV
            // 
            this.cmsTV.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.cmsTV.Name = "cmsTV";
            this.cmsTV.Size = new System.Drawing.Size(76, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(75, 22);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1620, 865);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Condition graph editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.main_panel.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainField)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgProp)).EndInit();
            this.cms.ResumeLayout(false);
            this.cmsTV.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton normGraphSB;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ContextMenuStrip cms;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton syncSB;
        private System.Windows.Forms.Panel main_panel;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox ExerciseGroupsBox;
        private System.Windows.Forms.ComboBox CategoriesBox;
        private System.Windows.Forms.TreeView mainTV;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TreeView subTV;
        private System.Windows.Forms.PictureBox mainField;
        private System.Windows.Forms.DataGridView dgProp;
        private System.Windows.Forms.ToolStripMenuItem editPropetiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editLabelToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripButton saveCurExrLocal;
        private System.Windows.Forms.ToolStripMenuItem editPropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simpleConditionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSubtreeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsTV;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem makeUniqueToolStripMenuItem;

    }
}