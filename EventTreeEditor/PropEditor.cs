using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace EventTreeEditor
{
    public partial class PropEditor : Form
    {
        private GraphNode CurGn;
        private ComboBox cb;
        private TreeView tv;
        private TextBox tb;
        private Dictionary<int, string> conditionNames;
        private Dictionary<int, string> operations;
        private List<GraphNode> objArr; 
        private DataSet ds;

        private class SimpleConditon
        {
            public string ID { get; set; }
            public string Name { get; set; }

            public SimpleConditon(string id, string name)
            {
                ID = id;
                Name = name;
            }

            public string ToString(bool onlyName = false)
            {
                return onlyName ? Name : ID + ". " + Name;
            }

            public override string ToString()
            {
                return ToString();
            }
        }

        private class ComplexCondition : SimpleConditon
        {
            public SimpleConditon Op1 { get; set; }
            public SimpleConditon Op2 { get; set; }
            public string Operation { get; set; }

            public ComplexCondition(string id, string name, SimpleConditon operand1, string operation, SimpleConditon operand2) : base(id, name)
            {
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }

        public PropEditor()
        {
            InitializeComponent();
            //AutoSize = true;
            //Width = 400;
            //Height = 200;
        }

        private void Exsercises_Shown(object sender, EventArgs e)
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        public void ChooseSubTreeProp(DataSet ds, GraphNode gn, List<GraphNode> arr,  bool unary = false)
        {
            CurGn = gn;
            objArr = arr;
            this.ds = ds;
            Text = "Choose subtree properties";
            if (ds == null || !ds.Tables.Contains("Conditions") || !ds.Tables.Contains("ConditionComplex") ||
                !ds.Tables.Contains("ConditionOperations")) return;
            conditionNames = (from rw in ds.Tables["Conditions"].AsEnumerable()
                where (string) rw["Comment"] != ""
                select rw).ToDictionary(row => (int)row["ID"], row => Convert.ToString(row["Comment"]));
            operations = (from rw in ds.Tables["ConditionOperations"].AsEnumerable()
                          select rw).ToDictionary(row => (int)row["ID"], row => Convert.ToString(row["Name"]));
            //var test = conditionNames[4];
            var complexConditions = new List<ComplexCondition>();
            if (!unary)
            {
                complexConditions = (from rw in ds.Tables["ConditionComplex"].AsEnumerable()
                    where
                        rw["Operand_1_ID"].ToString() != "" && rw["Operand_2_ID"].ToString() != "" &&
                        conditionNames.Keys.Contains(Convert.ToInt16(rw["Condition_ID"]))
                    select rw).Select(
                        row =>
                            new ComplexCondition(Convert.ToString(row["Condition_ID"]),
                                conditionNames[Convert.ToInt16(row["Condition_ID"])],
                                new SimpleConditon(Convert.ToString(row["Operand_1_ID"]),
                                    conditionNames[Convert.ToInt16(row["Operand_1_ID"])]),
                                operations[Convert.ToInt16(row["Operation_ID"])],
                                new SimpleConditon(Convert.ToString(row["Operand_1_ID"]),
                                    conditionNames[Convert.ToInt16(row["Operand_1_ID"])])))
                    .ToList();

            }
            else
            {
                complexConditions = (from rw in ds.Tables["ConditionComplex"].AsEnumerable()
                    where (rw["Operand_1_ID"].ToString() != "" ^ rw["Operand_2_ID"].ToString() != "") &&
                          conditionNames.Keys.Contains(Convert.ToInt16(rw["Condition_ID"]))
                                     select rw).Select(
                        row =>
                            new ComplexCondition(Convert.ToString(row["Condition_ID"]),
                                conditionNames[Convert.ToInt16(row["Condition_ID"])],
                                new SimpleConditon(Convert.ToString(row["Operand_1_ID"]),
                                    conditionNames[Convert.ToInt16(row["Operand_1_ID"])]),
                                operations[Convert.ToInt16(row["Operation_ID"])],
                                new SimpleConditon(Convert.ToString(row["Operand_1_ID"]),
                                    conditionNames[Convert.ToInt16(row["Operand_1_ID"])])))
                    .ToList();
            }
            //var operations = (from rw in ds.Tables["ConditionOperations"].AsEnumerable()
             //                 select rw["Name"]).ToList();
            //complexConditions
            cb = new ComboBox
            {
                Parent = this,
                DataSource = complexConditions,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = Width,
                Height = 20,
                Top = 0,
                Left = 0
            };
            tv = new TreeView
            {
                Parent = this,
                Width = cb.Width,
                Height = 100,
                Left = 0,
                Top = cb.Top + cb.Height + 2,
            };
            cb.SelectedIndexChanged += cb_SelectedIndexChanged;
            //cb.SelectedItem = cb.Items.
            //tv.Nodes.Add(SQLManager.PopulateTreeNode(ds, Convert.ToInt16(CurGn.ID)));
            cb_SelectedIndexChanged(null, null);
            AutoSize = true;
            PlaceButtons(tv.Height + tv.Top, CloseForm, ApplySubTree);
        }

        private void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            tv.Nodes.Clear();
            int id = Convert.ToInt16(cb.SelectedItem.ToString().Split('.')[0]);
            tv.Nodes.Add(SQLManager.PopulateTreeNode(ds, id));
            tv.Nodes[0].Expand();
        }

        public void EditLabel(DataSet ds, GraphNode gn)
        {
            CurGn = gn;
            Text = "Label editor";
            this.ds = ds;
            tb = new TextBox
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = Width,
                //Height = 50,
                Text = gn.Label
            };
            //AutoSize = true;
            PlaceButtons(tb.Height, CloseForm, ApplyLabel);
        }

        public void ChooseLeafProp(DataSet ds, GraphNode gn)
        {
            CurGn = gn;
            this.ds = ds;
            Text = "Choose simple condition";
            if (ds == null || !ds.Tables.Contains("Conditions")) return;
            var complexConditions = (from rw in ds.Tables["ConditionComplex"].AsEnumerable()
                                     select rw["Condition_ID"]).ToList();
            var simpleConditions = (from rw in ds.Tables["Conditions"].AsEnumerable()
                where (string) rw["Comment"] != "" && !complexConditions.Contains(rw["ID"])
                select rw)
                .Select(row => new SimpleConditon(Convert.ToString(row["ID"]), Convert.ToString(row["Comment"])))
                .ToList();
            simpleConditions.Sort((x, y) => String.Compare(x.ToString(true), y.ToString(true)));
            cb = new ComboBox
            {
                Parent = this,
                DataSource = simpleConditions,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = Width-18,
                Height = 20,
                Top = 0,
                Left = 0
            };
            //if (gn.ID != "")
           //     cb.SelectedItem =
           //         cb.Items.Cast<SimpleConditon>().Select(item => item.ID == gn.ID && item.Name == gn.SubTreeName);
            PlaceButtons(cb.Height, CloseForm, ApplyLeaf);
        }

        public void PlaceButtons(int minH,EventHandler cncl, EventHandler aply)
        {
            var apply = new Button()
            {
                Parent = this,
                Text = "Apply",
                Width = 60,
                Height = 30,
                Top = minH + 10, 
            };
            var cancel = new Button()
            {
                Parent = this,
                Text = "Cancel",
                Width = 60,
                Height = 30,
                Top = minH + 10,
            };
            cancel.Click += new EventHandler(cncl);
            apply.Click += new EventHandler(aply);
            apply.Left = Width/2 - apply.Width - 5;
            cancel.Left = Width/2 + 5;
            //AutoSize = true;
            //Height = apply.Top + apply.Height + 5;
        }

        private void CloseForm(object sender, EventArgs e)
        {
            Close();
        }

        private void ApplyLabel(object sender, EventArgs e)
        {
            CurGn.Label = tb.Text;
            var rw = ds.Tables["Conditions"].Select("ID=" + CurGn.ID).FirstOrDefault();
            if (rw != null) rw["Comment"] = tb.Text;
            CloseForm(sender, e);
            //ds.Tables["Conditions"].R
        }

        private void ApplySubTree(object sender, EventArgs e)
        {
            var subtree = tv.Nodes[0];
            //foreach (var node in objArr)
            //{
            //    if (GraphNode.IsChildOf(CurGn, node))
            //        objArr.Remove(node);
            //}
            var graphNodes = Utils.TreeToObjArr(subtree, CurGn.Parent);
            objArr.RemoveAll(item => GraphNode.IsChildOf(CurGn, item));
            objArr.Remove(CurGn);
            if (CurGn.Equals(CurGn.Parent.Left))
                CurGn.Parent.Left = graphNodes.Find(item => item.Parent.Equals(CurGn.Parent));
            else
                CurGn.Parent.Right = graphNodes.Find(item => item.Parent.Equals(CurGn.Parent));
            objArr.AddRange(graphNodes);
            CloseForm(sender, e);
        }

        private void ApplyLeaf(object sender, EventArgs e)
        {
            if (CurGn.HasChildren)
                objArr.RemoveAll(item => GraphNode.IsChildOf(CurGn, item));
            //objArr.Remove(CurGn);
            CurGn.operand = null;
            CurGn.Left = null;
            CurGn.Right = null;
            CurGn.ID = (cb.SelectedItem as SimpleConditon).ID;
            CurGn.Label = (cb.SelectedItem as SimpleConditon).Name; //cb.Items.Cast<SimpleConditon>().ToList()[cb.SelectedIndex].Name; //smth awesome
            //((Form1) Application.OpenForms[0]).SelectionDone();
            CloseForm(sender, e);
        }
    }
}
