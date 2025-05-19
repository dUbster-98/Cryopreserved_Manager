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
        public static string MyConString = "Server=127.0.0.1;Port=3306;DataBase=Cyro;Uid=root;pwd=1234;";

        private UserInfo m_loggedInUser = new UserInfo();
        static private List<CellInfo> CellList = new List<CellInfo>();
        public CellManageService()
        {
            InitializeDatabase();
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
                string connectionString = $"Server=127.0.0.1;Port=3306;Uid=root;Pwd=1234;";
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS Cyro;";
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

                        command.CommandText = "INSERT INTO CellList (CellKey, CellName, Quantity, Location, ReceiptDay, BarcodeText, State, Desc) " +
                            "VALUES (@key, @cellName, @quantity, @location, @receiptDay, @barcodeText, @state, @description)";
                        command.Parameters.AddWithValue("@key", uuid);
                        command.Parameters.AddWithValue("@cellName", cellInfo.Name);
                        command.Parameters.AddWithValue("@quantity", cellInfo.Quantity);
                        command.Parameters.AddWithValue("@location", cellInfo.Location);
                        command.Parameters.AddWithValue("@receiptDay", cellInfo.ReceiptDay);
                        command.Parameters.AddWithValue("@barcodeText", cellInfo.BarcodeText);
                        command.Parameters.AddWithValue("@state", cellInfo.State);
                        command.Parameters.AddWithValue("@description", cellInfo.Desc);
                        command.ExecuteNonQuery();                  
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
            try
            {
                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {


                        command.CommandText = "DELETE FROM CellList WHERE CellKey = @key";
                        command.Parameters.AddWithValue("@key", cellId);
                    }
                }
            }
            catch
            {

            }
        }

        public List<CellInfo> GetAllCellInfos()
        {
            CellList.Clear();
            int id = 0;
            try
            {
                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "CREATE TABLE IF NOT EXISTS CellList(CellKey VARCHAR(255) PRIMARY KEY, CellName VARCHAR(50), Quantity VARCHAR(10), Location VARCHAR(50), " +
                                              "ReceiptDay VARCHAR(50), BarcodeText VARCHAR(50), State VARCHAR(50), Description VARCHAR(255));";
                        command.ExecuteNonQuery();

                        command.CommandText = "SELECT * FROM CellList";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CellInfo cell = new CellInfo();
                                cell.Id = id++;
                                cell.Key = reader.GetString(0);
                                cell.Name = reader.GetString(1);
                                cell.Quantity = reader.GetString(2);
                                cell.Location = reader.GetString(3);
                                cell.ReceiptDay = reader.GetString(4);
                                cell.BarcodeText = reader.GetString(5);
                                cell.State = reader.GetString(6);
                                cell.Desc = reader.GetString(7);
                                CellList.Add(cell);
                            }
                        }
                    }
                    db.Close();
                }
                return CellList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public CellInfo GetCellInfoById(string cellId)
        {
            // Retrieve a specific cell info record by ID
            return new CellInfo();
        }
    }
}
