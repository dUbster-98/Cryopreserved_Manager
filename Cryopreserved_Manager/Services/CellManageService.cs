using Cryopreserved_Manager.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace Cryopreserved_Manager.Services
{
    public interface ICellManageService
    {
        void InsertCellDB(CellInfo cellInfo);
        void ModifyCellDB(CellInfo cellInfo);
        void DeleteCellInfo(string cellId);
        List<CellInfo> GetAllCellInfos();
        CellInfo GetCellInfoById(string cellId);
    }

    public class CellManageService : ICellManageService
    {
        public static string MyConString = "Server=127.0.0.1;Port=3306;DataBase=CellList;Uid=root;pwd=1111;";

        private UserInfo m_loggedInUser = new UserInfo();
        static private List<CellInfo> cellList = new List<CellInfo>();
        public CellManageService()
        {

        }
        public void InitializeDatabase()
        {
            try
            {
                using (MySqlConnection mySqlConn = new MySqlConnection(MyConString))
                {
                    mySqlConn.Open();
                }
            }
            catch (Exception ex)
            {
                string connectionString = $"Server=127.0.0.1;Port=3306,Uid=root;Pwd=1234;";
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS CellList;";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertCellDB(CellInfo cellInfo)
        {
            try
            {
                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        string uuid = Guid.NewGuid().ToString();

                        command.CommandText = "CREATE TABLE IF NOT EXISTS CellList(CellKey VARCHAR(255) PRIMARY KEY, CellName VARCHAR(50), Quantity INT, Location VARCHAR(50), " +
                            "ReceiptDay VARCHAR(50), BarcodeText VARCHAR(50), State VARCHAR(50));";
                        command.ExecuteNonQuery();
                        // 동일한 ID 검색
                        command.CommandText = "SELECT EXISTS(SELECT * FROM CellList WHERE UserID = @ID)";
                        command.Parameters.AddWithValue("@ID", cellInfo.Id);
                        object isDataExist = command.ExecuteScalar();
                        int nDataExist = Convert.ToInt32(isDataExist);
                        if ((int)nDataExist == 0)
                        {
                            // 없으면 생성
                            command.CommandText = "INSERT INTO CellList (CellKey, CellName, Quantity, Location, ReceiptDay, BarcodeText) " +
                                "VALUES (@key, @cellName, @quantity, @location, @receiptDay, @state)";
                            command.Parameters.AddWithValue("@key", uuid);
                            command.Parameters.AddWithValue("@cellName", cellInfo.Name);
                            command.Parameters.AddWithValue("@quantity", cellInfo.Quantity);
                            command.Parameters.AddWithValue("@location", cellInfo.Location);
                            command.Parameters.AddWithValue("@receiptDay", cellInfo.ReceiptDay);
                            command.Parameters.AddWithValue("@state", cellInfo.State);
                            command.ExecuteNonQuery();
                        }
                    }
                    db.Close();
                }
            }

            catch
            {

            }
        }

        public void ModifyCellDB(CellInfo cellInfo)
        {
            // Update an existing cell info record in the database
        }

        public void DeleteCellInfo(string cellId)
        {
            // Delete a cell info record from the database by ID
        }

        public List<CellInfo> GetAllCellInfos()
        {
            // Retrieve all cell info records from the database
            return new List<CellInfo>();
        }

        public CellInfo GetCellInfoById(string cellId)
        {
            // Retrieve a specific cell info record by ID
            return new CellInfo();
        }
    }
}
