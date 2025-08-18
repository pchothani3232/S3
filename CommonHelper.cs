using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Reflection;

namespace DatabaseConfiguration
{
    public class CommonHelper
    {
        public static SqlParameter[] GetParameter(string storedProcName, sqlparam obj)
        {

            DataTable dt = GetStoredProcedureParameters(storedProcName);

            SqlParameter[] parameters = new SqlParameter[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string paramName = dt.Rows[i]["PARAMETER_NAME"].ToString();  // e.g. @Cpos_Key
                string paramMode = dt.Rows[i]["PARAMETER_MODE"].ToString();  // IN, OUT, INOUT
                string sqlTypeName = dt.Rows[i]["DATA_TYPE"].ToString();     // decimal, int, nvarchar...

                // Match property name with parameter name (without '@')
                string propertyName = paramName.Substring(1); // remove '@'
                PropertyInfo property = obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                object value = property != null ? property.GetValue(obj) ?? DBNull.Value : DBNull.Value;

                SqlDbType sqlDbType;
                if (!Enum.TryParse(sqlTypeName, true, out sqlDbType))
                {
                    sqlDbType = SqlDbType.VarChar; // fallback
                }

                // Create parameter
                parameters[i] = new SqlParameter(paramName, sqlDbType)
                {
                    Value = value
                };

                // Set parameter direction
                if (paramMode.Equals("OUT", StringComparison.OrdinalIgnoreCase) ||
                    paramMode.Equals("INOUT", StringComparison.OrdinalIgnoreCase))
                {
                    parameters[i].Direction = ParameterDirection.Output;
                }
            }

            return parameters;
        }
        public static DataTable GetDataTable(string storedProcedure, sqlparam obj)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CommClass.Connection))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(storedProcedure, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter[] spParams = GetParameter(storedProcedure, obj);
                        cmd.Parameters.AddRange(spParams);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                return null;
                MessageBox.Show("Error loading combobox: " + ex.Message);
            }
        }

        private static DataTable GetStoredProcedureParameters(string spName)
        {
            string sql = @"
            SELECT
                PARAMETER_NAME,
                PARAMETER_MODE,
                DATA_TYPE
            FROM INFORMATION_SCHEMA.PARAMETERS
            WHERE SPECIFIC_NAME = @SPName
            ORDER BY ORDINAL_POSITION";
            using (SqlConnection con = new SqlConnection(CommClass.Connection))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@SPName", spName);

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
                con.Close();
            }
        }
    }
    public class sqlparam
    {
        public string status { get; set; }
        public string stateName { get; set; }
    }
}
