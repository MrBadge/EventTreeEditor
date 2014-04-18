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
    class SQLManager
    {
        private static SqlConnection connection;
        private static DataSet dataSet = new DataSet();
        //private static SqlDataReader reader;
        private static readonly SqlCommand GetCategories = new SqlCommand("SELECT * FROM dbo.Categories")
        {
            CommandType = CommandType.Text
        };

        private static readonly SqlCommand GetExrGroups =
            new SqlCommand(
                "SELECT * FROM dbo.ExerciseGroups  WHERE FinishCondition_ID != '' AND StartCondition_ID != ''");
        private static readonly SqlCommand GetExrGroupping = new SqlCommand("SELECT * FROM dbo.ExerciseGroupping");
        private static readonly SqlCommand GetExr = new SqlCommand("SELECT * FROM dbo.Exercises");
        private static readonly SqlCommand GetConditions = new SqlCommand("SELECT ID, Comment FROM dbo.Conditions");
        private static readonly SqlCommand GetConditionsComplex =
            new SqlCommand("SELECT * FROM dbo.ConditionComplex");
        private static readonly SqlCommand GetExerciseErrors = new SqlCommand("SELECT * FROM dbo.ExerciseErrors");
        private static readonly SqlCommand GetErrors = new SqlCommand("SELECT * FROM dbo.Errors");
        

        public class InternalDataBase
        {
             
        }

        public static DataSet FillDataSet()
        {
            try
            {
                var conn_str = new SqlConnectionStringBuilder();
                conn_str.IntegratedSecurity = true;
                conn_str.DataSource = @".\SQLEXPRESS";
                //conn_str.UserInstance = true;
                conn_str.InitialCatalog = @"DrivingCourses";
                connection =
                    new SqlConnection(conn_str.ConnectionString);
                connection.Open();

                dataSet.Tables.Add(GetDataTable(GetCategories, "Categories"));
                dataSet.Tables.Add(GetDataTable(GetExrGroups, "ExerciseGroups"));
                dataSet.Tables.Add(GetDataTable(GetExrGroupping, "ExerciseGroupping"));
                dataSet.Tables.Add(GetDataTable(GetExr, "Exercises"));
                dataSet.Tables.Add(GetDataTable(GetConditions, "Conditions"));
                dataSet.Tables.Add(GetDataTable(GetConditionsComplex, "ConditionsComplex"));
                dataSet.Tables.Add(GetDataTable(GetExerciseErrors, "ExerciseErrors"));
                dataSet.Tables.Add(GetDataTable(GetErrors, "Errors"));
                    
                return dataSet;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        public static System.Windows.Forms.TreeNode PopulateTreeNode(DataSet ds, int ExerGroupID, int CatGrID)
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

            var node1 = new System.Windows.Forms.TreeNode(ExercisesGroup["StartCondition_ID"] + ". " + "начало");
            var node2 = new System.Windows.Forms.TreeNode(ExercisesGroup["FinishCondition_ID"] + ". " + "конец");
            var result = new[] { node1, node2 }.ToList();
            foreach (var row in exercises)
            {
                var nd1 = new System.Windows.Forms.TreeNode(row["StartCondition_ID"] + ". " + "начало");
                var nd2 = new System.Windows.Forms.TreeNode(row["FinishCondition_ID"] + ". " + "конец");
                var array = new[] {nd1, nd2}.ToList();
                var errors = GetErrorsID(ds, Convert.ToInt16(row["ID"]));
                array.AddRange(from error in errors
                    let tmp = ds.Tables["Errors"].Rows.Find(error["Error_ID"])
                    where tmp != null
                    select new System.Windows.Forms.TreeNode(error["Error_ID"] + ". " + tmp["Description"]));
                var trNd = new System.Windows.Forms.TreeNode(row["ID"] + ". " + row["Name"], array.ToArray());
                //var node = new System.Windows.Forms.TreeNode(row["ID"] + ". " + row["Name"]);

                result.Add(trNd);
            }
            //var treeNode = new System.Windows.Forms.TreeNode(ExerciseGroupsBox.SelectedItem.ToString(), array);

            var res = new System.Windows.Forms.TreeNode(ExercisesGroup["Description"].ToString(), result.ToArray());
            //result = result.Cast<System.Windows.Forms.TreeNode>();
            return res;
        }

        private static List<DataRow> GetErrorsID(DataSet ds, int ExrsID)
        {
            var errors =
                (from row in ds.Tables["ExerciseErrors"].AsEnumerable()
                 where row.Field<int>("Exercise_ID") == ExrsID
                 select row).ToList<DataRow>();
            return errors;
        } 

        private static DataTable GetDataTable(SqlCommand sqlCommand, string name)
        {
            sqlCommand.Connection = connection;
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            dataTable.TableName = name;
            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };
            return dataTable;
        }

        public static DataSet GetTable(SqlConnection conn)
        {
            return null;
        }
    }
}
