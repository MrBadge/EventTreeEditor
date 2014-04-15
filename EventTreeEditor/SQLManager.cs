using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EventTreeEditor
{
    class SQLManager
    {
        private static SqlConnection connection;

        public static bool ConnectSQL()
        {
            try
            {
                var conn_str = new SqlConnectionStringBuilder();
                conn_str.IntegratedSecurity = true;
                conn_str.DataSource = @".\SQLEXPRESS";
                conn_str.UserInstance = true;
                conn_str.InitialCatalog = @"DrivingCourses";
                connection =
                    new SqlConnection(conn_str.ConnectionString);
                connection.Open();
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
    }
}
