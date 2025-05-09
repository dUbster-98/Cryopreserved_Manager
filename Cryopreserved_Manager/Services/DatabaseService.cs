using Cryopreserved_Manager.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Cryopreserved_Manager.Services
{
    public interface IDatabaseService
    {
        void InitializeDatabase();
        void AddCellInfo(CellInfo cellInfo);
        void UpdateCellInfo(CellInfo cellInfo);
        void DeleteCellInfo(string cellId);
        List<CellInfo> GetAllCellInfos();
        CellInfo GetCellInfoById(string cellId);
    }

    public class DatabaseService : IDatabaseService
    {
        public static string MyConString = "Server=127.0.0.1;Port=3306;Database=cogvision;Uid=root;pwd=1111;";

        public DatabaseService()
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
                MessageBox.Show(ex.ToString());
            }
        }

        public void AddCellInfo(CellInfo cellInfo)
        {
            // Add a new cell info record to the database
        }
        public void UpdateCellInfo(CellInfo cellInfo)
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
