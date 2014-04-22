using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace EventTreeEditor
{
    public partial class Form1 : Form
    {
        public const int DefPointRad = 2;
        public const int DefCirclRad = 20;
        //public const int DefDiagRad = 20;
        public const int DefLineWidth = 1;
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
        }

        public void Zoom(object sender, MouseEventArgs e) //MAKE THIS WORK
        {
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            //Size newSize = new Size((int)(mainField.Image.Width * zoomFactor), (int)(mainField.Image.Height * zoomFactor));
            //Bitmap bmp = new Bitmap(mainField.Image, newSize);
            //mainField.Image = bmp;
            var brush = new SolidBrush(Color.Black);
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
                    scaleHeight));
        }

        public static bool IsOutOfBounds(PictureBox img, int R, Point p)
        {
            int i = 0;
            while (i < 20*Math.PI)
            {
                double X = R*Math.Cos(i) + p.X;
                double Y = R*Math.Sin(i) + p.Y;
                if ((X < 0) || (X > img.Width) || (Y < 0) || (Y > img.Height - 10))
                    return true;
                i += 1;
            }
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
            for (int i = 0; i < ObjArr.Count; i++)
            {
                ObjArr[i].Draw(g);
            }
            var Pen = new Pen(Color.Black, DefLineWidth);
            Pen.EndCap = LineCap.ArrowAnchor;
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
            CurPlace = new Point(e.X, e.Y);
            var p = new Point(e.X, e.Y);
            Glob_Ind_Tmp = GetObjIndex(p);
            if (e.Button == MouseButtons.Right && Glob_Ind_Tmp != -1)
            {
                cms.Show(mainField, p);
            }
            if (e.Button != MouseButtons.Left) return;
            //Bmp_tmp = mainField.Image;
            /*if (Glob_Ind_Tmp != -1 && ModifierKeys.HasFlag(Keys.Shift))
            {
                IsScaling = true;
            }
            else*/
            if (Glob_Ind_Tmp != -1 && ModifierKeys.HasFlag(Keys.Control) && ObjArr[Glob_Ind_Tmp].childNodes.Count < 2)
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
                ObjArr[Glob_Ind_Tmp].ChangeParams(p.X, p.Y, /*CurColor,*/ DefCirclRad);
                ReDarawScene(mainField);
            }
        }

        private void mainField_MouseMove(object sender, MouseEventArgs e)
        {
            CurPlace = new Point(e.X, e.Y);
            if (IsDrawing)
            {
                try
                {
                    //mainField.Image = Bmp_tmp;
                    var p = new Point(ObjArr[ObjArr.Count - 1].X, ObjArr[ObjArr.Count - 1].Y);
                    int Rtmp = ObjArr[ObjArr.Count - 1].Radius;
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
            }
            else*/
            if (IsConnecting)
            {
                DrawConnection(mainField, StartNode);
            }
        }

        private void mainField_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if ((Glob_Ind_Tmp != -1) && (ObjArr.Count > 0))
            {
                if (ObjArr[Glob_Ind_Tmp].Radius <= DefPointRad + 2)
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
            }
            int objTmp = GetObjIndex(e.Location);
            if (StartNode != -1 && objTmp != -1 && objTmp != StartNode &&
                !ObjArr[StartNode].childNodes.Contains(ObjArr[objTmp]) && ObjArr[objTmp].Parent == null)
            {
                ObjArr[StartNode].childNodes.Add(ObjArr[objTmp]);
                ObjArr[objTmp].Parent = ObjArr[StartNode];
            }
            StartNode = -1;
            ReDarawScene(mainField);
            IsDrawing = false;
            IsMoving = false;
            IsConnecting = false;
            //IsScaling = false;
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
            treeView2.Top = ExerciseGroupsBox.Top + ExerciseGroupsBox.Height + 2;
            treeView2.Left = ExerciseGroupsBox.Left;
            splitContainer2.SplitterDistance = Convert.ToInt32(Width*0.25);
            splitContainer3.SplitterDistance = Convert.ToInt32(Height*0.5);
            //mainField.Top = main_panel.Top;
            //mainField.Left = main_panel.Left;
            
            //treeView2.Width = splitContainer2.Width;
            treeView2.Height = splitContainer2.Height - 44;
            //splitContainer1.SplitterDistance = Convert.ToInt32(Width*0.8);

            //if (mainField.Image != null)
            //mainField.Image = ResizeImage(mainField.Image, Width, Height);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1_Resize(sender, e);
            dataSet = SQLManager.FillDataSet();
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
        }

        private void mainField_MouseEnter(object sender, EventArgs e)
        {
            mainField.Focus();
        }

        //private Tree<> 

        private void cms_Opening(object sender, CancelEventArgs e)
        {
            /*if (GetObjIndex(MousePosition) == -1)
            {
                //cms.Close();
                SendKeys.Send("{ESC}");
            }*/
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ObjArr[Glob_Ind_Tmp].Parent != null)
            {
                ObjArr[Glob_Ind_Tmp].Parent.childNodes.RemoveAll(item => item.Equals(ObjArr[Glob_Ind_Tmp]));
            }
            foreach (GraphNode node in ObjArr[Glob_Ind_Tmp].childNodes)
            {
                node.Parent = null;
            }
            ObjArr.RemoveAt(Glob_Ind_Tmp);
            ReDarawScene(mainField);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //var exrs = new Exsercises();
            //exrs.Show();
            //SQLManager.ConnectSQL();
        }

        private void ExerciseGroupsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TreeView tw = new TreeView();
            ExerGroupID = ExerciseGroupsBox.SelectedItem != null ? Convert.ToInt16(ExerciseGroupsBox.SelectedItem.ToString().Split('.')[0]) : -1;
            if (ExerGroupID != -1 && CategorieID != -1)
            {
                treeView2.Nodes.Add(SQLManager.PopulateTreeNodeMain(dataSet, ExerGroupID, CategorieID));
                treeView2.ExpandAll();
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
            //ExerciseGroupsBox.SelectedItem = ExerciseGroupsBox.Items
        }

        private void CategoriesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CategorieID = Convert.ToInt16(CategoriesBox.SelectedItem.ToString().Split('.')[0]);
            treeView2.Nodes.Clear();
            ExerciseGroupsBox.SelectedIndex = -1;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            treeView2.Width = panel1.Width;
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level != 2 && (e.Node.Level != 1 || e.Node.GetNodeCount(false) != 0)) return;
            treeView1.Nodes.Clear();
            try
            {
                var tmp = SQLManager.PopulateTreeNode(dataSet, Convert.ToInt16(e.Node.Text.Split('.')[0]));
                treeView1.Nodes.Add(tmp);
                treeView1.ExpandAll();
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            //Utils.NormalizeGraph(ObjArr, mainField);
            //MessageBox.Show(e.Node.Level.ToString());
        }
    }
}