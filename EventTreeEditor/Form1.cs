using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace EventTreeEditor
{
    public partial class Form1 : Form
    {
        //public const int DefPointRad = 2;
        //public const int DefCirclRad = 10;
        //public const int DefDiagRad = 20;
        //public const int DefLineWidth = 1;
        //public static int CurSecCount;
        public static int Glob_Ind_Tmp;
        //public static int CurFigure = 0;
        //public static int CurTool = 0;
        public static int Glob_X_Tmp;
        public static int Glob_Y_tmp;
        //public static double Alpha_tmp;
        public static Point CurPlace = new Point(-1, -1);
        //public static Color CurColor;
        public static bool IsDrawing = false;
        public static bool IsMoving = false;
        public static bool IsDragging = false;
        //public static bool IsScaling = false;
        public static bool IsConnecting = false;
        public static Bitmap bmp_tmp;
        public static int StartNode;
        public static List<GraphNode> ObjArr = new List<GraphNode>();
        public static DataSet dataSet;
        public static int ExerGroupID = -1;
        public static int CategorieID = -1;

        public Form1()
        {
            InitializeComponent();
            //CurFigure = 0;
            //CurTool = 0;
            //CurColor = mainField.BackColor;
            Glob_Ind_Tmp = -1;
            StartNode = -1;
            //ObjArr.Capacity = 0;
            var bmp = new Bitmap(mainField.Width, mainField.Height);
            mainField.Image = bmp;
            mainField.AutoSize = true;

            mainField.MouseWheel += Zoom;
            DoubleBuffered = true;
            KeyPreview = true;
        }

        public void Zoom(object sender, MouseEventArgs e) //MAKE THIS WORK
        {
            double zoomFactor = e.Delta > 0 ? 0.1 : -0.1;
            //Size newSize = new Size((int)(mainField.Image.Width * zoomFactor), (int)(mainField.Image.Height * zoomFactor));
            //Bitmap bmp = new Bitmap(mainField.Image, newSize);
            //mainField.Image = bmp;
            var tmp = TreeNode.zoomFactor + zoomFactor >= 0.1
                ? TreeNode.zoomFactor + zoomFactor
                : TreeNode.zoomFactor;
            if (tmp > TreeNode.MaxZoomFactor)
                TreeNode.zoomFactor = TreeNode.MaxZoomFactor;
            else
                TreeNode.zoomFactor = tmp;
            //var tmp = Convert.ToInt32(TreeNode.DefCirclRad*zoomFactor);
            //TreeNode.DefCirclRad = tmp <= TreeNode.DefPointRad ? TreeNode.DefPointRad + 5 : tmp;
            ReDarawScene(mainField);
            /*var img = mainField.Image;
            Bitmap bm = new Bitmap(img, Convert.ToInt32(img.Width * zoomFactor), Convert.ToInt32(img.Height * zoomFactor));
            Graphics grap = Graphics.FromImage(bm);
            grap.InterpolationMode = InterpolationMode.HighQualityBicubic;
            mainField.Image = bm;*/

            /*var brush = new SolidBrush(Color.Black);
            Image image = mainField.Image;

            var bmp = new Bitmap(mainField.Image.Width, mainField.Image.Height);
            Graphics graph = Graphics.FromImage(bmp);

            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int) (mainField.Image.Width*zoomFactor);
            var scaleHeight = (int) (mainField.Image.Height*zoomFactor);

            graph.FillRectangle(brush, new RectangleF(0, 0, mainField.Width, mainField.Height));
            graph.DrawImage(image,
                new Rectangle((mainField.Width - scaleWidth)/2, (mainField.Height - scaleHeight)/2, scaleWidth,
                    scaleHeight));*/
        }

        public static bool IsOutOfBounds(PictureBox img, int R, Point p)
        {
            /*int i = 0;
            while (i < 20*Math.PI)
            {
                double X = R*Math.Cos(i) + p.X;
                double Y = R*Math.Sin(i) + p.Y;
                if ((X < 0) || (X > img.Width) || (Y < 0) || (Y > img.Height - 10))
                    return true;
                i += 1;
            }*/
            return false;
        }

        public void ReDarawScene(PictureBox img)
        {
            if (bmp_tmp != null) bmp_tmp.Dispose();
            bmp_tmp = new Bitmap(img.Width, img.Height);
            Graphics g = Graphics.FromImage(bmp_tmp);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            for (int i = 0; i < ObjArr.Count; i++)
            {
                ObjArr[i].Draw(g);
            }
            img.Image = bmp_tmp;
            img.Refresh();
            g.Dispose();
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var result = new Bitmap(width, height);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        public void DrawConnection(PictureBox img, int startNode)
        {
            if (bmp_tmp != null) bmp_tmp.Dispose();
            bmp_tmp = new Bitmap(img.Width, img.Height);
            Graphics g = Graphics.FromImage(bmp_tmp);
            //var myBrush = new SolidBrush(color);
            foreach (GraphNode node in ObjArr)
            {
                node.Draw(g);
            }
            var Pen = new Pen(Color.Black, GraphNode.DefLineWidth) {EndCap = LineCap.ArrowAnchor};
            var from = new Point(ObjArr[startNode].X, ObjArr[startNode].Y);
            Point to = GetCurPoint();
            double lambda = ObjArr[startNode].Radius/(Utils.LineLength(from, to) - ObjArr[startNode].Radius);
            double X_from = Math.Round((from.X + lambda*to.X)/(1 + lambda));
            double Y_from = Math.Round(from.Y + lambda*to.Y)/(1 + lambda);
            try
            {
                var p = new Point(Convert.ToInt32(X_from), Convert.ToInt32(Y_from));
                g.DrawLine(Pen, p, GetCurPoint());
                g.Dispose();
            }
            catch
            {
            }
            img.Image = bmp_tmp;
            img.Refresh();
            Pen.Dispose();
        }

        public bool MouseInObj(Point p, int i)
        {
            var p_tmp = new Point(ObjArr[i].X, ObjArr[i].Y);
            //var p_cur = new Point(Convert.ToInt32(p.X*TreeNode.zoomFactor), Convert.ToInt32(p.Y*TreeNode.zoomFactor));
            return Math.Sqrt(Math.Pow((p.X - p_tmp.X), 2) + Math.Pow((p.Y - p_tmp.Y), 2)) <= ObjArr[i].Radius;
        }

        public int GetObjIndex(Point p)
        {
            for (int i = ObjArr.Count; i > 0; i--)
                if (MouseInObj(p, i - 1))
                    return (i - 1);
            return -1;
        }

        public int GetDist(Point from, Point to)
        {
            return Convert.ToInt32(Math.Round(Math.Sqrt(Math.Pow((from.X - to.X), 2) + Math.Pow((from.Y - to.Y), 2))));
        }

        public static Point GetCurPoint()
        {
            return CurPlace;
        }

        private void mainField_MouseDown(object sender, MouseEventArgs e)
        {
            //CurPlace = new Point(Convert.ToInt32(e.X), Convert.ToInt32(e.Y));
            if (IsDragging || cms.Visible) return;
            var p = new Point(Convert.ToInt32(e.X), Convert.ToInt32(e.Y));
            Glob_Ind_Tmp = GetObjIndex(p);
            if (e.Button == MouseButtons.Right && Glob_Ind_Tmp != -1)
            {
                cms.Show(mainField, p);
            }
            if (e.Button == MouseButtons.Middle)
            {
                IsDragging = true;
                Cursor = Cursors.SizeAll;
                Glob_X_Tmp = p.X;
                Glob_Y_tmp = p.Y;
            }
            if (e.Button != MouseButtons.Left) return;
            //Bmp_tmp = mainField.Image;
            /*if (Glob_Ind_Tmp != -1 && ModifierKeys.HasFlag(Keys.Shift))
            {
                IsScaling = true;
            }
            else*/
            if (Glob_Ind_Tmp != -1 && ModifierKeys.HasFlag(Keys.Control) && (ObjArr[Glob_Ind_Tmp].Left == null || 
                ObjArr[Glob_Ind_Tmp].Right == null))
            {
                IsConnecting = true;
                StartNode = Glob_Ind_Tmp;
                DrawConnection(mainField, StartNode);
            }
            else if (Glob_Ind_Tmp != -1)
            {
                IsMoving = true;
                Glob_X_Tmp = ObjArr[Glob_Ind_Tmp].X;
                Glob_Y_tmp = ObjArr[Glob_Ind_Tmp].Y;
            }
            else if (Glob_Ind_Tmp == -1)
            {
                Glob_Ind_Tmp = ObjArr.Count;
                ObjArr.Add(new GraphNode(null));
                ObjArr[Glob_Ind_Tmp].ChangeParams(p.X, p.Y, TreeNode.DefCirclRad /*CurColor,*/);
                ReDarawScene(mainField);
            }
        }

        private void mainField_MouseMove(object sender, MouseEventArgs e)
        {
            CurPlace = new Point(e.X, e.Y);
            if (IsDragging)
            {
                foreach (var node in ObjArr)
                {
                    node.Move(node.X + Convert.ToInt32((CurPlace.X - Glob_X_Tmp)*TreeNode.zoomFactor),
                        node.Y + Convert.ToInt32((CurPlace.Y - Glob_Y_tmp)*TreeNode.zoomFactor));
                }
                Glob_X_Tmp = CurPlace.X;
                Glob_Y_tmp = CurPlace.Y;
                ReDarawScene(mainField);
            }
            else if (IsDrawing)
            {
                try
                {
                    //mainField.Image = Bmp_tmp;
                    var p = new Point(ObjArr[ObjArr.Count - 1].X, ObjArr[ObjArr.Count - 1].Y);
                    //int Rtmp = ObjArr[ObjArr.Count - 1].Radius;
                    //ObjArr[ObjArr.Count - 1].Scale(GetDist(p, new Point(e.X, e.Y)));
                    if (IsOutOfBounds(mainField, ObjArr[ObjArr.Count - 1].Radius, p))
                    {
                        //ObjArr[ObjArr.Count - 1].Scale(Rtmp - 25);
                        IsDrawing = false;
                        //throw new Exception("You are out of bounds!");
                    }
                    ReDarawScene(mainField);
                }
                catch (Exception)
                {
                    ReDarawScene(mainField);
                    MessageBox.Show("You are out of bounds!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
                    //throw;
                }
            }
            else if (IsMoving)
            {
                try
                {
                    var p = new Point(e.X, e.Y);
                    ObjArr[Glob_Ind_Tmp].Move(p.X, p.Y);
                    if (IsOutOfBounds(mainField, ObjArr[ObjArr.Count - 1].Radius, p))
                    {
                        ObjArr[Glob_Ind_Tmp].Move(Glob_X_Tmp, Glob_Y_tmp);
                        IsMoving = false;
                        //throw new Exception("You are out of bounds!");
                    }
                    ReDarawScene(mainField);
                }
                catch (Exception)
                {
                    ReDarawScene(mainField);
                    MessageBox.Show("You are out of bounds!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
                    //throw;
                }
            }
                /*else if (IsScaling)
            {
                try
                {
                    var p = new Point(ObjArr[Glob_Ind_Tmp].X, ObjArr[Glob_Ind_Tmp].Y);
                    ObjArr[Glob_Ind_Tmp].Scale(GetDist(p, new Point(e.X, e.Y)));
                    int Rtmp = ObjArr[Glob_Ind_Tmp].Radius;
                    if (IsOutOfBounds(mainField, ObjArr[Glob_Ind_Tmp].Radius, p))
                    {
                        ObjArr[Glob_Ind_Tmp].Scale(Rtmp - 25);
                        IsScaling = false;
                        //throw new Exception("You are out of bounds!");
                    }
                    ReDarawScene(mainField);
                }
                catch (Exception)
                {
                    ReDarawScene(mainField);
                    MessageBox.Show("You are out of bounds!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                                    MessageBoxDefaultButton.Button1);
                    //throw;
                }
            }*/
            else if (IsConnecting)
            {
                DrawConnection(mainField, StartNode);
            }    
        }

        private void mainField_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Middle) return;
            /*if ((Glob_Ind_Tmp != -1) && (ObjArr.Count > 0))
            {
                if (ObjArr[Glob_Ind_Tmp].Radius <= GraphNode.DefPointRad + 2)
                {
                    for (int i = 0; i < ObjArr.Count - 1; i++)
                    {
                        //if (ObjArr[i].childNodes.Contains(ObjArr[Glob_Ind_Tmp]))
                        //{
                        ObjArr[i].childNodes.RemoveAll(item => item.Equals(ObjArr[Glob_Ind_Tmp]));
                        //}
                    }
                    ObjArr.RemoveAt(Glob_Ind_Tmp);
                    ReDarawScene(mainField);
                }
            }*/
            int objTmp = GetObjIndex(e.Location);
            if (StartNode != -1 && objTmp != -1 && objTmp != StartNode && 
                ObjArr[objTmp].Parent == null && !IsParent(objTmp, StartNode))
            {
                if (ObjArr[StartNode].Left == null)
                    ObjArr[StartNode].Left = ObjArr[objTmp];
                else
                    ObjArr[StartNode].Right = ObjArr[objTmp];
                //ObjArr[StartNode].childNodes.Add(ObjArr[objTmp]);
                ObjArr[objTmp].Parent = ObjArr[StartNode];
            }
            StartNode = -1;
            ReDarawScene(mainField);
            IsDrawing = false;
            IsMoving = false;
            IsConnecting = false;
            IsDragging = false;
            Cursor = Cursors.Default;
            //IsScaling = false;
        }

        private bool IsParent(int start, int end)
        {
            var tmp = ObjArr[end].Parent;
            while (tmp != null)
            {
                if (tmp.Equals(ObjArr[start]))
                    return true;
                tmp = tmp.Parent;
            }
            return false;
        }

        private double GetGoodPerc(double val, double defval)
        {
            return (val <= 0 || val >= 1) ? defval : val;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //main_panel.Width = Convert.ToInt32(Width*0.8);
            //main_panel.Height = tree_panel.Height;
            //tree_panel.Left = main_panel.Width + 7;
            //tree_panel.Width = Width - main_panel.Width - 27;
            //mainField.Width = main_panel.Width - 5;
            //mainField.Height = main_panel.Height - 10;
            splitContainer1.Width = Width - 19;
            splitContainer1.Top = toolStrip1.Height;
            splitContainer1.Height = Height - toolStrip1.Height - 40;
            mainTV.Top = ExerciseGroupsBox.Top + ExerciseGroupsBox.Height + 2;
            mainTV.Left = ExerciseGroupsBox.Left;
            //mainField.Top = main_panel.Top;
            //mainField.Left = main_panel.Left;
            
            //treeView2.Width = splitContainer2.Width;
            mainTV.Height = splitContainer2.Height - 44;
            //splitContainer1.SplitterDistance = Convert.ToInt32(Width*0.8);

            //if (mainField.Image != null)
            //mainField.Image = ResizeImage(mainField.Image, Width, Height);
        }

        void toolStripComboBox1_DropDownClosed(object sender, EventArgs e)
        {
            cms.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Width = Properties.Settings.Default.Width;
            //Height = Properties.Settings.Default.Height;
            Form1_Resize(sender, e);
            //var tmp = Properties.Settings.Default.MainFieldSD;
            splitContainer2.SplitterDistance =
                Convert.ToInt32(0.25 * Width);//Properties["MainFieldSD"]);
            splitContainer1.SplitterDistance =
                Convert.ToInt32(0.75 * Width);//Properties["MainTreeSD"]);
            splitContainer3.SplitterDistance =
                Convert.ToInt32(0.5 * Height);//Properties["PropPanelSD"]);
            dataSet = SQLManager.FillDataSet();
            if (dataSet == null)
            {
                /*mainTV.Enabled = false;
                subTV.Enabled = false;
                CategoriesBox.Enabled = false;
                ExerciseGroupsBox.Enabled = false;
                cms.Items[0].Enabled = cms.Items[1].Enabled = cms.Items[2].Enabled = false;
                saveCurExrLocal.Enabled = false;
                syncSB.Enabled = false;
                return;*/
                MessageBox.Show("Work without SQL DB is not fully supported now, closing", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                Application.Exit(); 
                return;
            }
            foreach (DataRow row in dataSet.Tables["Categories"].Rows)
            {
                //Console.Write(row);
                CategoriesBox.Items.Add(row["ID"] + ". " + row["Name"]);
            }
            CategoriesBox.SelectedItem = CategoriesBox.Items[0];
            //CategorieID = Convert.ToInt16(CategoriesBox.SelectedItem.ToString().Split('.')[0]);
            //CategorieID = 
            //CategorieID = dataSet.Tables["Categories"].
            //mainField.ContextMenuStrip = cms;
            toolStripComboBox1.SelectedIndexChanged += toolStripComboBox1_SelectedIndexChanged;
            toolStripComboBox1.DropDownClosed += toolStripComboBox1_DropDownClosed;
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            //mainField.Image = ResizeImage(mainField.Image, main_panel.Width, main_panel.Height);
            //Tree<TreeNode> tree = Utils.ObjArrToTree(ObjArr);
            //var tmp = tree.Height(tree.Root);
            Utils.NormalizeGraph(ObjArr, mainField);
            ReDarawScene(mainField);
            //Thread.Sleep(10);
        }

        private void main_panel_MouseEnter(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 1)
                mainField.Focus();
        }

        //private Tree<> 

        /*private void cms_Opening(object sender, CancelEventArgs e)
        {
            /*if (GetObjIndex(MousePosition) == -1)
            {
                //cms.Close();
                SendKeys.Send("{ESC}");
            }
        }*/

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ObjArr[Glob_Ind_Tmp].IsRoot && ObjArr[Glob_Ind_Tmp].ID == GetCurExrID())
            {
                MessageBox.Show("Can't remove exercise root node", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            if (ObjArr[Glob_Ind_Tmp].Parent != null)
            {
                if (ObjArr[Glob_Ind_Tmp].Parent.Left != null &&
                    ObjArr[Glob_Ind_Tmp].Parent.Left.Equals(ObjArr[Glob_Ind_Tmp]))
                    ObjArr[Glob_Ind_Tmp].Parent.Left = null;
                else
                    ObjArr[Glob_Ind_Tmp].Parent.Right = null;
                //ObjArr[Glob_Ind_Tmp].Parent
                //ObjArr[Glob_Ind_Tmp].Parent.childNodes.RemoveAll(item => item.Equals(ObjArr[Glob_Ind_Tmp]));
            }
            if (ObjArr[Glob_Ind_Tmp].Left != null)
                ObjArr[Glob_Ind_Tmp].Left.Parent = null;
            if (ObjArr[Glob_Ind_Tmp].Right != null)
                ObjArr[Glob_Ind_Tmp].Right.Parent = null;
            /*foreach (GraphNode node in ObjArr[Glob_Ind_Tmp].childNodes)
            {
                node.Parent = null;
            }*/
            ObjArr.RemoveAt(Glob_Ind_Tmp);
            ReDarawScene(mainField);
        }

        private string GetCurExrID()
        {
            return subTV.Nodes.Count == 0 ? "-1" : subTV.Nodes[0].Text.Split('.')[0];
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            SQLManager.UploadSqlServer(dataSet.Tables["Conditions"]);
            SQLManager.UploadSqlServer(dataSet.Tables["ConditionComplex"]);
            //Cursor.Current = Cursors.Hand;
            //this.Cursor = Cursors.Hand;
            //IsDragging = true;
            //var exrs = new Exsercises();
            //exrs.Show();
            //SQLManager.ConnectSQL();
            Cursor.Current = Cursors.Default;
        }

        private void ExerciseGroupsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TreeView tw = new TreeView();
            ExerGroupID = ExerciseGroupsBox.SelectedItem != null ? Convert.ToInt16(ExerciseGroupsBox.SelectedItem.ToString().Split('.')[0]) : -1;
            if (ExerGroupID != -1 && CategorieID != -1)
            {
                mainTV.Nodes.Clear();
                mainTV.Nodes.Add(SQLManager.PopulateTreeNodeMain(dataSet, ExerGroupID, CategorieID));
                mainTV.ExpandAll();
            }
            //treeView2.Nodes.Add("начало");
            //treeView2.Nodes.Add("конец");
        }

        private void ExerciseGroupsBox_DropDown(object sender, EventArgs e)
        {
            ExerciseGroupsBox.Items.Clear();
            foreach (DataRow row in dataSet.Tables["ExerciseGroups"].Rows)
            {
                ExerciseGroupsBox.Items.Add(row["ID"] + ". " + row["Description"]);
            }
            ExerciseGroupsBox.SelectedItem = ExerciseGroupsBox.Items[0];
        }

        private void CategoriesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CategorieID = Convert.ToInt16(CategoriesBox.SelectedItem.ToString().Split('.')[0]);
            mainTV.Nodes.Clear();
            subTV.Nodes.Clear();
            ObjArr.Clear();
            ExerciseGroupsBox.SelectedIndex = -1;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            mainTV.Width = panel1.Width;
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level != 2 && (e.Node.Level != 1 || e.Node.GetNodeCount(false) != 0)) return;
            subTV.Nodes.Clear();
            try
            {
                var tmp = SQLManager.PopulateTreeNode(dataSet, Convert.ToInt16(e.Node.Text.Split('.')[0]), e.Node.Index < 2);
                subTV.Nodes.Add(tmp);
                subTV.ExpandAll();
                ObjArr.Clear();
                var tmpNodes = Utils.TreeToObjArr(tmp);
                if (tmpNodes != null)
                {
                    //ObjArr.Clear();
                    ObjArr = tmpNodes;
                    Utils.NormalizeGraph(ObjArr, mainField);
                }
            }
            catch
            {
                ObjArr.Clear();
                //MessageBox.Show(ex.Message);
            }
            ReDarawScene(mainField);
            //Utils.NormalizeGraph(ObjArr, mainField);
            //MessageBox.Show(e.Node.Level.ToString());
        }

        private void treeView2_MouseEnter(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 1)
                mainTV.Focus();
        }

        private void treeView1_MouseEnter(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 1)
                subTV.Focus();
        }

        private void splitContainer2_Resize(object sender, EventArgs e)
        {
            ReDarawScene(mainField);
            Utils.NormalizeGraph(ObjArr, mainField);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void ShowPropeties(DataGridView dg, Dictionary<string, string> prop)
        {

            //dg.RowCount = prop.Count;
            //var i = 0;
            var propArray = (from row in prop select new { Property = row.Key, Value = row.Value }).ToArray();
            dg.DataSource = propArray;
            dg.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //dg.AutoResizeColumns();
            //dg.ReadOnly = false;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0 || e.Node.Index % 2 == 1) return;
            var name = e.Node.Text.Split(new[] { '.', ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
            var id = e.Node.Text.Split('.')[0];
            var prop = new Dictionary<string, string>
            {
                {"ID", id},
                {"Name", name}
            };  
            ShowPropeties(dgProp, prop);
        }

        private void mainField_Click(object sender, EventArgs e)
        {
            if (Glob_Ind_Tmp == -1 || cms.Visible) return;
            var tmp = ObjArr.Find(item => item.Checked);
            if (tmp != null && !tmp.Equals(ObjArr[Glob_Ind_Tmp]))
                tmp.Checked = false;
            //if (tmp != null && tmp.Equals(ObjArr[Glob_Ind_Tmp]))
            //    return;
            var prop = new Dictionary<string, string>
            {
                {"ID", ObjArr[Glob_Ind_Tmp].ID},
                {"Name", ObjArr[Glob_Ind_Tmp].Label},
                //{"Operand", ObjArr[Glob_Ind_Tmp].operand},
                //{"Radius", Convert.ToString(ObjArr[Glob_Ind_Tmp].Radius)},
                //{"X", Convert.ToString(ObjArr[Glob_Ind_Tmp].X)},
                //{"Y", Convert.ToString(ObjArr[Glob_Ind_Tmp].Y)}
            };
            if (ObjArr[Glob_Ind_Tmp].operand != null)
            {
                prop.Add("Operand 1", ObjArr[Glob_Ind_Tmp].Left == null ? "" : ObjArr[Glob_Ind_Tmp].Left.ID);
                prop.Add("Operation", ObjArr[Glob_Ind_Tmp].operand);
                prop.Add("Operand 2", ObjArr[Glob_Ind_Tmp].Right == null ? "" : ObjArr[Glob_Ind_Tmp].Right.ID);
            }
            ObjArr[Glob_Ind_Tmp].Checked = true;
            ShowPropeties(dgProp, prop);
        }

        private void dataGridView1_MouseEnter(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 1)
                dgProp.Focus();
        }

        /*public void SelectionDone()
        {
            ReDarawScene(mainField);
        }*/

        private void cms_Opening(object sender, CancelEventArgs e)
        {
            cms.Items[0].Visible = ObjArr[Glob_Ind_Tmp].HasChildren;
            cms.Items[1].Visible = !ObjArr[Glob_Ind_Tmp].IsRoot;
        }

        private void editLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmp = new PropEditor();
            if (String.IsNullOrEmpty(ObjArr[Glob_Ind_Tmp].ID)) return;
            tmp.EditLabel(dataSet, ObjArr[Glob_Ind_Tmp]);
            tmp.ShowDialog();
            if (ObjArr[Glob_Ind_Tmp].GetParent.ID == null) return;
            subTV.Nodes.Clear();
            subTV.Nodes.Add(SQLManager.PopulateTreeNode(dataSet, Convert.ToInt16(ObjArr[Glob_Ind_Tmp].GetParent.ID), true));
            subTV.ExpandAll();
        }

        private void editPropetiesToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            toolStripComboBox1.Items.Clear();
            foreach (var operand in SQLManager.GetOperands(dataSet))
            {
                toolStripComboBox1.Items.Add(operand);
            }
            foreach (var item in toolStripComboBox1.Items.Cast<object>().Where(item => 
                item.ToString() == ObjArr[Glob_Ind_Tmp].operand))
            {
                toolStripComboBox1.SelectedItem = item;
                break;
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObjArr[Glob_Ind_Tmp].operand = toolStripComboBox1.SelectedItem.ToString();
            var rw = dataSet.Tables["ConditionComplex"].Select("Condition_ID=" + ObjArr[Glob_Ind_Tmp].ID).FirstOrDefault();
            if (rw != null) rw["Operation_ID"] = Convert.ToInt16(((SQLManager.Operand)toolStripComboBox1.SelectedItem).ID);
            //treeView1.Nodes.Clear();
            //treeView1.Nodes.Add(SQLManager.PopulateTreeNode(dataSet, Convert.ToInt16(ObjArr[Glob_Ind_Tmp].GetParent.ID)));
            //treeView1.ExpandAll();
            toolStripComboBox1.DroppedDown = false;
            ReDarawScene(mainField);
        }

        private void editPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*var tmp = new PropEditor();
            //if (String.IsNullOrEmpty(ObjArr[Glob_Ind_Tmp].operand))
            //{
            //    MessageBox.Show("Select operation first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            if (ObjArr[Glob_Ind_Tmp].HasChildren)
            {
                tmp.ChooseSubTreeProp(dataSet, ObjArr[Glob_Ind_Tmp], ObjArr, ObjArr[Glob_Ind_Tmp].ChildrenCount != 2);   
            }
            else
            {
                tmp.ChooseLeafProp(dataSet, ObjArr[Glob_Ind_Tmp]);
            }
            tmp.ShowDialog();
            Utils.NormalizeGraph(ObjArr, mainField);
            ReDarawScene(mainField);*/
        }

        private void simpleConditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmp = new PropEditor();
            tmp.ChooseLeafProp(dataSet, ObjArr[Glob_Ind_Tmp], ObjArr);
            tmp.ShowDialog();
            Utils.NormalizeGraph(ObjArr, mainField);
            ReDarawScene(mainField);
        }

        private void addSubtreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmp = new PropEditor();
            tmp.ChooseSubTreeProp(dataSet, ObjArr[Glob_Ind_Tmp], ObjArr, ObjArr[Glob_Ind_Tmp].ChildrenCount == 1);
            tmp.ShowDialog();
            Utils.NormalizeGraph(ObjArr, mainField);
            ReDarawScene(mainField);
        }

        private void saveCurExrLocal_Click(object sender, EventArgs e)
        {
            if (Utils.GraphNodeArrayCorrect(ObjArr))
            {
                Utils.GraphNodeArrayToDataSet(ObjArr, dataSet);
            }
            else
            {
                MessageBox.Show("Graph error occurs", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*Properties.Settings.Default.Width = Width;
            Properties.Settings.Default.Height = Height;
            Properties.Settings.Default.MainFieldSD = splitContainer2.SplitterDistance;
            Properties.Settings.Default.MainTreeSD = splitContainer1.SplitterDistance;
            Properties.Settings.Default.PropPanelSD = splitContainer3.SplitterDistance;
            Properties.Settings.Default.Save();*/
        }

        private void mainTV_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void AddExerciseErrorClick(object sender, EventArgs e)
        {
            //MessageBox.Show("test");
            var tmp = new PropEditor();
            tmp.AddExerciseError(dataSet, Convert.ToInt32(mainTV.SelectedNode.Text.Split('.')[0]));
            tmp.ShowDialog();
            ExerciseGroupsBox_SelectedIndexChanged(null, null);
        }

        private void AddExerciseClick(object sender, EventArgs e)
        {
            var tmp = new PropEditor();
            tmp.AddExercise(dataSet, Convert.ToInt32(ExerciseGroupsBox.Text.Split('.')[0]),
                Convert.ToInt32(CategoriesBox.Text.Split('.')[0]));
            tmp.ShowDialog();
            ExerciseGroupsBox_SelectedIndexChanged(null, null);
        }

        private void RemoveErrorClick(object sender, EventArgs e)
        {
            var Id = mainTV.SelectedNode.Text.Split('.')[0];
            var rw = dataSet.Tables["ExerciseErrors"].Select("ID=" + Id).FirstOrDefault();
            DataRow rw2 = null;
            if (rw != null)
            {
                rw2 = dataSet.Tables["Errors"].Select("ID=" + rw["Error_ID"]).FirstOrDefault();
                rw.Delete();
            }
            if (rw2 != null) rw2.Delete();
            dataSet.AcceptChanges();
            ExerciseGroupsBox_SelectedIndexChanged(null, null);
        }

        private void RemoveExerciseGroupClick(object sender, EventArgs e)
        {
            var ExrsID = mainTV.SelectedNode.Text.Split('.')[0];
            var ExrsGrID = ExerciseGroupsBox.Text.Split('.')[0];
            var rw1 = dataSet.Tables["ExerciseGroupping"].Select("ExerciseGroup_ID=" + ExrsGrID + "AND Exercise_ID=" + ExrsID).FirstOrDefault();
            if (rw1 != null) rw1.Delete();
            /*var rws2 = dataSet.Tables["ExerciseErrors"].Select("Exercise_ID=" + ExrsID);
            foreach (var row in rws2)
            {
                row.Delete();
            }*/
            dataSet.AcceptChanges();
            ExerciseGroupsBox_SelectedIndexChanged(null, null);
        }

        private void mainTV_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            mainTV.SelectedNode = e.Node;
            if (e.Node.Level == 0)
            {
                cmsTV.Items.Clear();
                cmsTV.Items.Add(new ToolStripMenuItem("Add new exercise", null, AddExerciseClick));
                cmsTV.Show(mainTV, e.Location);
            }
            if (e.Node.Level == 1 && e.Node.Index > 1)
            {
                cmsTV.Items.Clear();
                cmsTV.Items.Add(new ToolStripMenuItem("Add new error", null, AddExerciseErrorClick));
                cmsTV.Items.Add(new ToolStripMenuItem("Remove exercise group", null, RemoveExerciseGroupClick));
                cmsTV.Show(mainTV, e.Location);
            }
            if (e.Node.Level == 2 && e.Node.Index > 1)
            {
                cmsTV.Items.Clear();
                cmsTV.Items.Add(new ToolStripMenuItem("Remove error", null, RemoveErrorClick));
                cmsTV.Show(mainTV, e.Location);
            }
            
        }

    }
}