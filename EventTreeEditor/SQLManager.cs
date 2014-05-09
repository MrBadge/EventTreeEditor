using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EventTreeEditor
{
    static class SQLManager
    {
        private static SqlConnection connection;
        private static string conn_string;
        private static DataSet dataSet = new DataSet();
        //private static SqlDataReader reader;
        //private const string GetTable = "SELECT * FROM @table";
        //private static readonly SqlCommand GetCategories = new SqlCommand("SELECT * FROM dbo.Categories");
        private static readonly SqlCommand GetExrGroups =
            new SqlCommand(
                "SELECT * FROM dbo.ExerciseGroups  WHERE FinishCondition_ID != '' AND StartCondition_ID != ''");
        /*private static readonly SqlCommand GetExrGroupping = new SqlCommand("SELECT * FROM dbo.ExerciseGroupping");
        private static readonly SqlCommand GetExr = new SqlCommand("SELECT * FROM dbo.Exercises");
        private static readonly SqlCommand GetConditions = new SqlCommand("SELECT ID, Comment FROM dbo.Conditions");
        private static readonly SqlCommand GetConditionsComplex =
            new SqlCommand("SELECT * FROM dbo.ConditionComplex");
        private static readonly SqlCommand GetExerciseErrors = new SqlCommand("SELECT * FROM dbo.ExerciseErrors");
        private static readonly SqlCommand GetErrors = new SqlCommand("SELECT * FROM dbo.Errors");
        private static readonly SqlCommand GetCondOperations = new SqlCommand("SELECT * FROM dbo.ConditionOperations");*/

        public struct Operand
        {
            public string ID;
            public string Name;
            public override string ToString()
            {
                return Name;
            }

            public Operand(string id, string nm)
            {
                ID = id;
                Name = nm;
            }
        }

        public static List<Operand> GetOperands(DataSet ds)
        {
            return (from row in ds.Tables["ConditionOperations"].AsEnumerable()
                select row).Select(row => new Operand(Convert.ToString(row["ID"]), Convert.ToString(row["Name"]).Trim()))
                .ToList();
        } 

        public static DataSet FillDataSet()
        {
            try
            {
                conn_string = Properties.Settings.Default.ConnectonString;
                if (String.IsNullOrEmpty(conn_string))
                { 
                    //Properties.Settings.Default.Save();
                    var conn_str = new SqlConnectionStringBuilder();
                    conn_str.IntegratedSecurity = true;
                    conn_str.DataSource = @".\SQLEXPRESS";
                    //conn_str.UserInstance = true;
                    conn_str.InitialCatalog = @"DrivingCourses";
                    conn_string = conn_str.ConnectionString;
                }
                connection =
                    new SqlConnection(conn_string);
                connection.Open();

                dataSet.Tables.Add(GetDataTable("Categories"));
                dataSet.Tables.Add(GetDataTable("ExerciseGroups", GetExrGroups));
                dataSet.Tables.Add(GetDataTable("ExerciseGroupping"));
                dataSet.Tables.Add(GetDataTable("Exercises"));
                dataSet.Tables.Add(GetDataTable("Conditions"));
                dataSet.Tables.Add(GetDataTable("ConditionComplex"));
                dataSet.Tables.Add(GetDataTable("ExerciseErrors"));
                dataSet.Tables.Add(GetDataTable("Errors"));
                dataSet.Tables.Add(GetDataTable("ConditionOperations"));
                    
                connection.Close();
                return dataSet;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        public static System.Windows.Forms.TreeNode PopulateTreeNodeMain(DataSet ds, int ExerGroupID, int CatGrID)
        {
            CatGrID = Convert.ToInt16(ds.Tables["Categories"].Rows.Find(CatGrID)["CategoryGroup_ID"]);
            var ExercisesGroup = ds.Tables["ExerciseGroups"].Rows.Find(ExerGroupID);
            var exersList =
                (from row in ds.Tables["ExerciseGroupping"].AsEnumerable()
                    where row.Field<int>("ExerciseGroup_ID") == ExerGroupID
                    select row["Exercise_ID"]).ToList();
            var exercises =
                (from row in ds.Tables["Exercises"].AsEnumerable()
                    where row.Field<int>("CategoryGroup_ID") == CatGrID
                    && exersList.Contains(row.Field<int>("ID")) 
                    select row).ToList<DataRow>();
            /*
             && !(row.Field<int?>("StartCondition_ID") == null
                    || row.Field<int?>("FinishCondition_ID") == null)
             */
            //var Exercises = new DataTable();
            //exercises.ToList()

            var node1 = new System.Windows.Forms.TreeNode(ExercisesGroup["StartCondition_ID"] + ". " + "begin");
            var node2 = new System.Windows.Forms.TreeNode(ExercisesGroup["FinishCondition_ID"] + ". " + "end");
            var result = new[] { node1, node2 }.ToList();
            foreach (var row in exercises)
            {
                var nd1 = new System.Windows.Forms.TreeNode(row["StartCondition_ID"] + ". " + "begin");
                var nd2 = new System.Windows.Forms.TreeNode(row["FinishCondition_ID"] + ". " + "end");
                var array = new[] {nd1, nd2}.ToList();
                var errors = GetErrorsID(ds, Convert.ToInt16(row["ID"]));
                array.AddRange(from error in errors
                    let tmp = ds.Tables["Errors"].Rows.Find(error["Error_ID"])
                    where tmp != null
                    select new System.Windows.Forms.TreeNode(error["ID"] + ". " + tmp["Description"]));
                var trNd = new System.Windows.Forms.TreeNode(row["ID"] + ". " + row["Name"], array.ToArray());
                //var node = new System.Windows.Forms.TreeNode(row["ID"] + ". " + row["Name"]);

                result.Add(trNd);
            }
            //var treeNode = new System.Windows.Forms.TreeNode(ExerciseGroupsBox.SelectedItem.ToString(), array);

            var res = new System.Windows.Forms.TreeNode(ExercisesGroup["Description"].ToString(), result.ToArray());
            //result = result.Cast<System.Windows.Forms.TreeNode>();
            return res;
        }

        private static string GetCondName(DataSet ds, int CondID)
        {
            var row = (from rw in ds.Tables["Conditions"].AsEnumerable()
                       where rw.Field<int>("ID") == CondID
                       select rw).ToList<DataRow>();
            if (row.Count == 0) return "";
            var record = row[0]["Comment"];
            return Convert.ToString(CondID) + ". " + record;
        }

        private static List<DataRow> GetCondRow(DataSet ds, int CondID)
        {
            return (from rw in ds.Tables["ConditionComplex"].AsEnumerable()
                       where rw.Field<int>("Condition_ID") == CondID
                       select rw).ToList<DataRow>();
        }

        private static int GetLeftChild(DataSet ds, int CondID)
        {
            var row = GetCondRow(ds, CondID);
            if (row.Count == 0) return -1;
            var LeftChild = GetCellVal(ds, CondID, "Operand_1_ID");
            return Convert.ToInt16(LeftChild);
        }

        private static int GetRightChild(DataSet ds, int CondID)
        {
            var row = GetCondRow(ds, CondID);
            if (row.Count == 0) return -1;
            var RightChild = GetCellVal(ds, CondID, "Operand_2_ID");
            return Convert.ToInt16(RightChild);
        }

        private static int ParseCondID(string str)
        {
            try
            {
                return Convert.ToInt16(str.Split('.')[0]);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static string GetCellVal(DataSet ds, int CondID, string CellName)
        {
            var row = GetCondRow(ds, CondID);
            return row.Count == 0 ? "err" : row[0][CellName].ToString();
        }

        private static string IDtoOperation(DataSet ds, string ID)
        {
            var row = (from rw in ds.Tables["ConditionOperations"].AsEnumerable()
                where rw.Field<int>("ID") == Convert.ToInt16(ID)
                select rw["Name"]).ToList();
            return row.Count != 1 ? "" : row[0].ToString().Trim();
        }

        public static string OperationtToID(DataSet ds, string operation)
        {
            var rw = ds.Tables["ConditionOperations"].Select("Name='" + operation+"'").FirstOrDefault();
            return rw != null ? Convert.ToString(rw["ID"]) : null;
        }

        private static List<System.Windows.Forms.TreeNode> GetChilds(DataSet ds, int CondID)
        {
            System.Windows.Forms.TreeNode LeftChild;
            System.Windows.Forms.TreeNode RightChild;
            System.Windows.Forms.TreeNode operation;
            try
            {
                LeftChild = new System.Windows.Forms.TreeNode(GetCondName(ds,
                    Convert.ToInt16(GetCellVal(ds, CondID, "Operand_1_ID"))));
            }
            catch (Exception)
            {
                LeftChild = null; //new System.Windows.Forms.TreeNode();
            }
            try
            {
                RightChild =
                new System.Windows.Forms.TreeNode(GetCondName(ds,
                    Convert.ToInt16(GetCellVal(ds, CondID, "Operand_2_ID"))));
            }
            catch (Exception)
            {
                RightChild = null; //new System.Windows.Forms.TreeNode();
            }
            try
            {
                operation = new System.Windows.Forms.TreeNode(IDtoOperation(ds, GetCellVal(ds, CondID, "Operation_ID")));
            }
            catch (Exception)
            {
                operation = null;
            }
            
            if (LeftChild == null && RightChild == null && operation == null)
                return new List<System.Windows.Forms.TreeNode>();
            if (LeftChild == null)
                return new[] { operation, RightChild }.ToList();
            return RightChild == null
                ? new[] {LeftChild, operation}.ToList()
                : new[] {LeftChild, operation, RightChild}.ToList();
        }

        public static System.Windows.Forms.TreeNode PopulateTreeNode(DataSet ds, int ID, bool isCond)
        {
            var CondID = !isCond ? Convert.ToInt16(ds.Tables["ExerciseErrors"].Select("ID=" + ID).FirstOrDefault()["Condition_ID"]) : ID;
            var ComplexCondList =
                (from row in ds.Tables["ConditionComplex"].AsEnumerable()
                 select row["Condition_ID"]).ToList();
            var root = new System.Windows.Forms.TreeNode(GetCondName(ds, CondID), GetChilds(ds, CondID).ToArray());
            //var CurNode = root;
            var allexpanded = false;
            var CurID = -1;
            //var test = new TreeView();
            //tv.Nodes.Add(root);
            while (!allexpanded)
            {
                allexpanded = true;
                //TreeNodeCollection tmp = root.Nodes;
                //var Nodes = root.Nodes;
                //while (CurNode != null)
                //{
                //    if (CurNode.Nodes[0] != null)
                //}
                IEnumerable<System.Windows.Forms.TreeNode> allNodes = root.Nodes.Cast<System.Windows.Forms.TreeNode>()
                    .Flatten<System.Windows.Forms.TreeNode>(n => n.Nodes.Cast<System.Windows.Forms.TreeNode>());
                foreach (var node in allNodes)
                {
                    //var tmp = (node as System.Windows.Forms.TreeNode);
                    CurID = ParseCondID(node.Text);
                    if (!ComplexCondList.Contains(CurID) || node.GetNodeCount(false) != 0) continue;
                    allexpanded = false;
                    node.Nodes.AddRange(GetChilds(ds, CurID).ToArray());
                }
            }  
            return root;
        }

        public static IEnumerable<T> Flatten<T>(
            this IEnumerable<T> rootNodes,
            Func<T, IEnumerable<T>> childrenFunction)
        {
            return rootNodes.SelectMany(
                child => new[] { child }
                    .Concat(childrenFunction(child).Flatten(childrenFunction)));
        }

        //private void PrintRecursive(System.Windows.Forms.TreeNode treeNode)
        //{
            // Print the node.
            //var tmp = (node as System.Windows.Forms.TreeNode);
            //CurID = ParseCondID(tmp.Text);
            //if (!ComplexCondList.Contains(CurID) || tmp.GetNodeCount(false) != 0) continue;
            //allexpanded = false;
            //tmp.Nodes.AddRange(GetChilds(ds, CurID).ToArray());
            // Print each node recursively.
            //foreach (System.Windows.Forms.TreeNode tn in treeNode.Nodes)
            //{
            //    PrintRecursive(tn);
            //}
        //}

        //obsolete method
        public static System.Windows.Forms.TreeNode PopulateTreeNodeError(DataSet ds, int CondID)
        {
            //var row = (from rw in ds.Tables["ConditionComplex"].AsEnumerable()
            //           where rw.Field<int>("Condition_ID") == CondID
            //    select rw).ToList<DataRow>();
            //if (row == null) return null;
            //var Op1ID = Convert.ToInt16(row["Operand_1_ID"]);
            //var Op2ID = Convert.ToInt16(row["Operand_2_ID"]);
            /*var ComplexCondList =
                (from row in ds.Tables["ConditionComplex"].AsEnumerable()
                 //where row.Field<int>("ExerciseGroup_ID") == ExerGroupID
                 select row["Condition_ID"]).ToList(); */
            var original = (from rw in ds.Tables["ConditionComplex"].AsEnumerable()
                       where rw.Field<int>("Condition_ID") == CondID
                       select rw).ToList<DataRow>();
            if (original.Count == 0)
                return null;
            var CurCondID = CondID;
            var PrevCondID = -1;
            var root = new Tree<string>(GetCondName(ds, CondID));
            var clone = root;

            while (original.Count != 0)
            {
                if (GetLeftChild(ds, CurCondID) != -1 && clone.Left == null)
                {
                    PrevCondID = CurCondID;
                    CurCondID = GetLeftChild(ds, CurCondID);
                    clone.Left = new Tree<string>(GetCondName(ds, CurCondID));
                    //original = original.Left;
                    clone = clone.Left;
                }
                else if (GetRightChild(ds, CurCondID) != -1 && clone.Right == null)
                {
                    PrevCondID = CurCondID;
                    CurCondID = GetRightChild(ds, CurCondID);
                    clone.Right = new Tree<string>(GetCondName(ds, CurCondID));
                    clone = clone.Right;
                }
                else
                {
                    original = (from rw in ds.Tables["ConditionComplex"].AsEnumerable()
                                where rw.Field<int>("Condition_ID") == PrevCondID
                                select rw).ToList<DataRow>();
                    clone = clone.Parent;
                }
            }

            //return root;

            return null;
        }

        private static List<DataRow> GetErrorsID(DataSet ds, int ExrsID)
        {
            var errors =
                (from row in ds.Tables["ExerciseErrors"].AsEnumerable()
                 where row.Field<int>("Exercise_ID") == ExrsID
                 select row).ToList<DataRow>();
            return errors;
        } 

        private static DataTable GetDataTable(string tablename,  SqlCommand sqlCommand = null)
        {
            SqlDataReader reader;
            if (sqlCommand != null)
            {
                sqlCommand.Connection = connection;
                reader = sqlCommand.ExecuteReader();
            }
            else
            {
                var cmd = String.Format("SELECT * FROM {0}", "dbo." + tablename);
                var sqlCmd = new SqlCommand(cmd, connection);
                reader = sqlCmd.ExecuteReader();
            }
            var dataTable = new DataTable();
            dataTable.Load(reader);
            dataTable.TableName = tablename;
            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };
            return dataTable;
        }

        public static void UploadSqlServer(DataTable dt)
        {
            using (var cmd = new SqlCommand(String.Format("SELECT * FROM {0}", "dbo." + dt.TableName), 
                new SqlConnection(conn_string)))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    using (var cb = new SqlCommandBuilder(da))
                        da.Update(dt);
                }
            }
            //dt.TableName = "YourDataTable";
            /*if (conn_string == null) return;
            using (var conn = new SqlConnection(conn_string))
            {
                conn.Open();
                SqlTransaction trans = connection.BeginTransaction(); //for rollback if error
                //Start bulkCopy
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection,
                SqlBulkCopyOptions.TableLock |
                SqlBulkCopyOptions.FireTriggers,
                trans))
                {
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.DestinationTableName = dt.TableName;
                    bulkCopy.WriteToServer(dt);
                }
                trans.Commit();
            }*/
        }

        public static DataTable GetCondCompl()
        {
            return dataSet.Tables["ConditionComplex"];
        } 

        /*public static DataSet GetTable(SqlConnection conn)
        {
            return null;
        }*/
    }
}
