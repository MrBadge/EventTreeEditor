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
        private int defWidth = 350;
        //
        private GraphNode CurGn;
        private int CurExerID;
        private int CurCatGrID;
        private int CurExerGrID;
        private ComboBox cb;
        private TreeView tv;
        private TextBox tb;
        private Dictionary<int, string> conditionNames;
        private Dictionary<int, string> operations;
        private List<GraphNode> objArr; 
        private DataSet ds;
        //Add ExrcErr
        private TextBox ErrNameTB;
        private TextBox WeightTb;
        private RadioButton SimpleCondRB;
        private RadioButton ComplexCondRB;
        private ComboBox ConditionCB;
        //
        //AddExercise
        private TextBox nameTB;
        private TextBox briefTB;
        private ComboBox stCB;
        private TreeView stTV;
        private ComboBox fnCB;
        private TreeView fnTV;
        //
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

        private List<ComplexCondition> GetComlexConditions(bool unary = false, bool all = true)
        {
            conditionNames = (from rw in ds.Tables["Conditions"].AsEnumerable()
                              where (string)rw["Comment"] != ""
                              select rw).ToDictionary(row => (int)row["ID"], row => Convert.ToString(row["Comment"]));
            operations = (from rw in ds.Tables["ConditionOperations"].AsEnumerable()
                          select rw).ToDictionary(row => (int)row["ID"], row => Convert.ToString(row["Name"]));
            var complexConditions = new List<ComplexCondition>();
            try
            {
                if (!all)
                {
                    complexConditions = (from rw in ds.Tables["ConditionComplex"].AsEnumerable()
                        where
                            (!unary
                                ? rw["Operand_1_ID"].ToString() != "" && rw["Operand_2_ID"].ToString() != ""
                                : (rw["Operand_1_ID"].ToString() != "" ^ rw["Operand_2_ID"].ToString() != "")) &&
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
                        where
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
            }
            catch
            {
                complexConditions = new List<ComplexCondition>();
            }
            complexConditions.Sort((x, y) => String.Compare(x.ToString(true), y.ToString(true)));
            return complexConditions;
        } 

        public void ChooseSubTreeProp(DataSet ds, GraphNode gn, List<GraphNode> arr,  bool unary = false)
        {
            CurGn = gn;
            objArr = arr;
            this.ds = ds;
            Text = "Choose subtree properties";
            if (ds == null || !ds.Tables.Contains("Conditions") || !ds.Tables.Contains("ConditionComplex") ||
                !ds.Tables.Contains("ConditionOperations")) return;
            //var test = conditionNames[4];
            var complexConditions = GetComlexConditions(unary);
            
            //var operations = (from rw in ds.Tables["ConditionOperations"].AsEnumerable()
             //                 select rw["Name"]).ToList();
            //complexConditions
            cb = new ComboBox
            {
                Parent = this,
                DataSource = complexConditions,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = defWidth,
                Height = 20,
                Top = 0,
                Left = 5
            };
            tv = new TreeView
            {
                Parent = this,
                Width = defWidth,
                Height = 100,
                Left = 5,
                Top = cb.Top + cb.Height + 2,
            };
            cb.SelectedIndexChanged += cb_SelectedIndexChanged;
            //cb.SelectedItem = cb.Items.
            //tv.Nodes.Add(SQLManager.PopulateTreeNode(ds, Convert.ToInt16(CurGn.ID)));
            cb_SelectedIndexChanged(null, null);
            //AutoSize = true;
            PlaceButtons(tv, CloseForm, ApplySubTree);
        }

        private void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb.SelectedItem == null) return;
            tv.Nodes.Clear();
            int id = Convert.ToInt16(cb.SelectedItem.ToString().Split('.')[0]);
            tv.Nodes.Add(SQLManager.PopulateTreeNode(ds, id, true));
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
                Left = 5,
                Width = defWidth,
                //Height = 50,
                Text = gn.Label,
                AcceptsReturn = false
            };
            //AutoSize = true;
            PlaceButtons(tb, CloseForm, ApplyLabel);
        }

        private List<SimpleConditon> GetSimpleComditions()
        {
            var complexConditions = (from rw in ds.Tables["ConditionComplex"].AsEnumerable()
                                     select rw["Condition_ID"]).ToList();
            var simpleConditions = (from rw in ds.Tables["Conditions"].AsEnumerable()
                                    where (string)rw["Comment"] != "" && !complexConditions.Contains(rw["ID"])
                                    select rw)
                .Select(row => new SimpleConditon(Convert.ToString(row["ID"]), Convert.ToString(row["Comment"])))
                .ToList();
            simpleConditions.Sort((x, y) => String.Compare(x.ToString(true), y.ToString(true)));
            return simpleConditions;
        } 

        public void ChooseLeafProp(DataSet ds, GraphNode gn, List<GraphNode> arr)
        {
            CurGn = gn;
            this.ds = ds;
            objArr = arr;
            Text = "Choose simple condition";
            if (ds == null || !ds.Tables.Contains("Conditions")) return;
            var simpleConditions = GetSimpleComditions();
            //simpleConditions.Sort((x, y) => String.Compare(x.ToString(true), y.ToString(true)));
            cb = new ComboBox
            {
                Parent = this,
                DataSource = simpleConditions,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = Width-18,
                Height = 20,
                Top = 0,
                Left = 5
            };
            //if (gn.ID != "")
           //     cb.SelectedItem =
           //         cb.Items.Cast<SimpleConditon>().Select(item => item.ID == gn.ID && item.Name == gn.SubTreeName);
            //AutoSize = true;
            PlaceButtons(cb, CloseForm, ApplyLeaf);
        }

        public void AddExercise(DataSet dataSet, int exrGrID, int catGrID)
        {
            ds = dataSet;
            CurCatGrID = Convert.ToInt16(ds.Tables["Categories"].Select("ID=" + catGrID).FirstOrDefault()["CategoryGroup_ID"]);
            CurExerGrID = exrGrID;
            Text = "Exercise adding";
            var label1 = new Label()
            {
                Parent = this,
                Top = 5,
                Left = 5,
                Text = "Name:",
                Width = defWidth,
                Height = 20
            };
            nameTB = new TextBox()
            {
                Parent = this,
                Left = 5,
                Top = label1.Top + label1.Height + 5,
                Width = defWidth,
                Height = 20,
                AcceptsReturn = false,
            };
            var label2 = new Label()
            {
                Parent = this,
                Left = 5,
                Top = nameTB.Top + nameTB.Height + 5,
                Width = defWidth,
                Height = 20,
                Text = "Brief:"
            };
            briefTB = new TextBox()
            {
                Parent = this,
                Left = 5,
                Top = label2.Top + label2.Height + 5,
                Width = defWidth,
                Height = 20,
            };
            var label3 = new Label()
            {
                Parent = this,
                Left = 5,
                Top = briefTB.Top + briefTB.Height + 5,
                Width = defWidth,
                Height = 20,
                Text = "Start Condition ID:"
            };
            var allConditions = GetSimpleComditions();
            allConditions.AddRange(GetComlexConditions());
            stCB = new ComboBox()
            {
                Parent = this,
                Left = 5,
                Top = label3.Top + label3.Height + 5,
                Width = defWidth,
                Height = 20,
                DataSource = allConditions,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            stTV = new TreeView
            {
                Parent = this,
                Width = defWidth,
                Height = 100,
                Left = 5,
                Top = stCB.Top + stCB.Height + 5,
            };
            stCB.SelectedIndexChanged += StCbOnSelectedIndexChanged;
            fnCB = new ComboBox()
            {
                Parent = this,
                Left = 5,
                Top = stTV.Top + stTV.Height + 5,
                Width = defWidth,
                Height = 20,
                BindingContext = new BindingContext(),
                DataSource = allConditions,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            fnTV = new TreeView
            {
                Parent = this,
                Width = defWidth,
                Height = 100,
                Left = 5,
                Top = fnCB.Top + fnCB.Height + 5,
            };
            fnCB.SelectedIndexChanged += FnCbOnSelectedIndexChanged;

            PlaceButtons(fnTV, CloseForm, ApplyExercise);
        }

        private void FnCbOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (fnCB.SelectedItem == null) return;
            fnTV.Nodes.Clear();
            int id = Convert.ToInt16(fnCB.SelectedItem.ToString().Split('.')[0]);
            fnTV.Nodes.Add(SQLManager.PopulateTreeNode(ds, id, true));
            fnTV.Nodes[0].Expand();
        }

        private void StCbOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (stCB.SelectedItem == null) return;
            stTV.Nodes.Clear();
            int id = Convert.ToInt16(stCB.SelectedItem.ToString().Split('.')[0]);
            stTV.Nodes.Add(SQLManager.PopulateTreeNode(ds, id, true));
            stTV.Nodes[0].Expand();
        }

        private void ApplyExercise(object sender, EventArgs e)
        {
            if (nameTB.Text == "")
            {
                MessageBox.Show("Exercise name can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                nameTB.Focus();
                return;
            }
            var newExer = ds.Tables["Exercises"].NewRow();
            newExer["Name"] = nameTB.Text;
            newExer["CategoryGroup_ID"] = CurCatGrID;
            newExer["Number"] = 0; //What does this column state for?
            newExer["Brief"] = briefTB.Text;
            newExer["StartCondition_ID"] = (stCB.SelectedItem as SimpleConditon).ID;
            newExer["FinishCondition_ID"] = (fnCB.SelectedItem as SimpleConditon).ID;
            ds.Tables["Exercises"].Rows.Add(newExer);

            var newExrInGr = ds.Tables["ExerciseGroupping"].NewRow();
            newExrInGr["ExerciseGroup_ID"] = CurExerGrID;
            newExrInGr["Exercise_ID"] = newExer["ID"];
            ds.Tables["ExerciseGroupping"].Rows.Add(newExrInGr);

            //ds.AcceptChanges();

            CloseForm(sender, e);
        }

        public void AddExerciseError(DataSet dataSet, int exrsId)
        {
            ds = dataSet;
            CurExerID = exrsId;
            var label1 = new Label()
            {
                Parent = this,
                Top = 5,
                Left = 5,
                Text = "Error name:",
                Width = defWidth,
                Height = 20
            };
            ErrNameTB = new TextBox()
            {
                Parent = this,
                Left = 5,
                Top = label1.Top + label1.Height + 5,
                Width = defWidth,
                Height = 20,
                AcceptsReturn = false,
            };
            var label2 = new Label()
            {
                Parent = this,
                Left = 5,
                Top = ErrNameTB.Top + ErrNameTB.Height + 5,
                Width = defWidth,
                Height = 20,
                Text = "Weight:"
            };
            WeightTb = new TextBox()
            {
                Parent = this,
                Left = 5,
                Top = label2.Top + label2.Height + 5,
                Width = defWidth,
                Height = 20,
                MaxLength = 3
            };
            WeightTb.KeyPress += WeightTbOnKeyPress;
            var label3 = new Label()
            {
                Parent = this,
                Left = 5,
                Top = WeightTb.Top + WeightTb.Height + 5,
                Width = defWidth,
                Height = 20,
                Text = "Root Condition ID:"
            };
            SimpleCondRB = new RadioButton()
            {
                Parent = this,
                Left = label3.Left + 30,
                Top = label3.Top + label3.Height + 5,
                Width = defWidth / 2,
                Height = 20,
                Checked = true,
                Text = "Simple"
            };
            SimpleCondRB.CheckedChanged += SimpleCondRbOnCheckedChanged;
            ComplexCondRB = new RadioButton()
            {
                Parent = this,
                Left = label3.Left + 30,
                Top = SimpleCondRB.Top + SimpleCondRB.Height + 5,
                Width = defWidth / 2,
                Height = 20,
                Checked = false,
                Text = "Complex"
            };
            ComplexCondRB.CheckedChanged += ComplexCondRbOnCheckedChanged;
            ConditionCB = new ComboBox()
            {
                Parent = this,
                Left = 5,
                Top = ComplexCondRB.Top + ComplexCondRB.Height + 5,
                Width = defWidth,
                Height = 20,
                DataSource = GetSimpleComditions(),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            tv = new TreeView
            {
                Parent = this,
                Width = defWidth,
                Height = 100,
                Left = 5,
                Top = ConditionCB.Top + ConditionCB.Height + 5,
            };
            ConditionCB.SelectedIndexChanged += ConditionCbOnSelectedIndexChanged;

            //AutoSize = true;
            PlaceButtons(tv, CloseForm, ApplyExerciseError);
        }

        private void ComplexCondRbOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            ConditionCB.DataSource = null;
            ConditionCB.Items.Clear();
            ConditionCB.DataSource = GetComlexConditions();
        }

        private void SimpleCondRbOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            ConditionCB.DataSource = null;
            ConditionCB.Items.Clear();
            ConditionCB.DataSource = GetSimpleComditions();
            tv.Nodes.Clear();
        }


        private void ConditionCbOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (/*ComplexCondRB.Checked && */ConditionCB.SelectedItem != null)
            {
                tv.Nodes.Clear();
                int id = Convert.ToInt16(ConditionCB.SelectedItem.ToString().Split('.')[0]);
                tv.Nodes.Add(SQLManager.PopulateTreeNode(ds, id, true));
                tv.Nodes[0].Expand();
            }
        }

        private void WeightTbOnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((e.KeyChar <= 57 && e.KeyChar >= 48) || e.KeyChar == 13 || e.KeyChar == 8))
                e.KeyChar = Convert.ToChar(0);
        }

        public void PlaceButtons(Control prevCntrl, EventHandler cncl, EventHandler aply)
        {
            var apply = new Button()
            {
                Parent = this,
                Text = "Apply",
                Width = 60,
                Height = 30,
                Top = prevCntrl.Top + prevCntrl.Height + 10, 
            };
            var cancel = new Button()
            {
                Parent = this,
                Text = "Cancel",
                Width = 60,
                Height = 30,
                Top = prevCntrl.Top + prevCntrl.Height + 10,
            };
            cancel.Click += new EventHandler(cncl);
            apply.Click += new EventHandler(aply);
            apply.Left = prevCntrl.Width / 2 - apply.Width - 5;
            cancel.Left = prevCntrl.Width / 2 + 5;
            //AutoSize = true;
            //Height = apply.Top + apply.Height + 5;
            //Width = defWidth;
        }

        private void CloseForm(object sender, EventArgs e)
        {
            Close();
        }

        private void ApplyExerciseError(object sender, EventArgs e)
        {
            if (ErrNameTB.Text == "")
            {
                MessageBox.Show("Error name can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ErrNameTB.Focus();
                return;
            }
            var newError = ds.Tables["Errors"].NewRow();
            newError["Description"] = ErrNameTB.Text;
            ds.Tables["Errors"].Rows.Add(newError);
            var newExerError = ds.Tables["ExerciseErrors"].NewRow();
            newExerError["Exercise_ID"] = CurExerID;
            newExerError["Error_ID"] = newError["ID"];
            if (Convert.ToInt32(WeightTb.Text) > 255)
                WeightTb.Text = "255"; 
            newExerError["Weight"] = Convert.ToByte(WeightTb.Text);
            newExerError["Condition_ID"] = (ConditionCB.SelectedItem as SimpleConditon).ID;
            ds.Tables["ExerciseErrors"].Rows.Add(newExerError);

            //ds.AcceptChanges();

            CloseForm(sender, e);
        }

        private void ApplyLabel(object sender, EventArgs e)
        {
            CurGn.Label = tb.Text;
            var rw = ds.Tables["Conditions"].Select("ID=" + CurGn.ID).FirstOrDefault();
            if (rw != null) rw["Comment"] = tb.Text;

            //ds.AcceptChanges();

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
