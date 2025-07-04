using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Services
{
    public class ExcelDb
    {
        private SqlConnection connection;
        private static ExcelDb instance;

        public string Location { get; set; }

        public static ExcelDb GetInstance()
        {
            if (instance == null) instance = new ExcelDb();

            return instance;
        }

        public void ConnectToExcel()
        {
            //var db = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};" +
            //    @"Extended Properties=""Excel 12.0 Xml;HDR=YES""", Location);
            var db = "Data Source=.;Initial Catalog=NLAS;Integrated Security=True;";

            connection = new SqlConnection(db);

            connection.Open();
        }

        public async Task<SqlDataReader> Read(string script)
        {
            var command = new SqlCommand(script, connection);

            return await command.ExecuteReaderAsync();
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }

        public void Close()
        {
            if (connection == null) return;
            if (connection.State != ConnectionState.Closed) return;

            connection.Close();
            connection.Dispose();
        }

        //public string[] GetTableNames()
        //{
        //    var activityDataTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

        //    var tables = new string[activityDataTable.Rows.Count];

        //    var counter = 0;

        //    for (int cnt = 0; cnt < activityDataTable.Rows.Count; cnt++)
        //    {
        //        var worksheetName = activityDataTable.Rows[cnt]["TABLE_NAME"].ToString();

        //        if (worksheetName.Contains("Print") ||
        //            worksheetName.Contains("OLE")) continue;

        //        tables[counter++] = worksheetName;
        //    }

        //    return tables;
        //}
    }
}
