using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cryopreserved_Manager.Services
{
    public struct UserInfo
    {
        public string key { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userID { get; set; }
        public string status { get; set; }
        public string role { get; set; }
        public string department { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool needResetPW { get; set; }
        public string Password_Changed_Date { get; set; }
        public string Password_retry { get; set; }
        public bool adminAlarm { get; set; }
    }

    public interface IUserManagementService
    {
        bool LoadAllUsers();
        void InsertUserDB(string firstname, string lastname, string userID, string status, string role, string department, 
            string phone, string email, string password, bool needResetPW, bool adminAlarm, string pwChangeDate, string pwRetry);
        void ModifyUserDB(string UUID, string firstName, string lastName, string status, string role, string department,
            string phone, string email, string password, string pwChangeDate, string pwRetry);
        void ModifyDBPWRetry(string userID, string status, string pwRetry);
        void ModifyPassword(string userID, string password);
        int GetPWRetry(string userID);
        string GetUserStatus(string userID);
        bool IsNeedChangePassword(string userID);
        bool UserLogin(string userID, string password);
    }

    public class UserManagementService : IUserManagementService
    {
        private string m_DatabasePath = "";
        private UserInfo m_loggedInUser = new UserInfo();
        static private List<UserInfo> listUser = new List<UserInfo>();

        public UserManagementService()
        {
            GenerateDBFileName();
        }
        public string DatabasePath { get { return m_DatabasePath; } }
        private void GenerateDBFileName()
        {
            m_DatabasePath = "C:\\ProgramData\\Cryopreserved\\etc\\user.db";
        }

        public UserInfo GetLoggedInUser()
        {
            return m_loggedInUser;
        }
        static public List<UserInfo> GetUserList()
        {
            return listUser;
        }
        public void InsertUserDB(string firstname, string lastname, string userID,
            string status, string role, string department, string phone, string email, string password, bool needResetPW, bool adminAlarm, string pwChangeDate, string pwRetry)
        {
            try
            {
                if (!File.Exists(m_DatabasePath))
                {
                    // Database 폴더가 없으면 생성
                    string folderPath = "C:\\ProgramData\\Cryopreserved\\etc";
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    SQLiteConnection.CreateFile(m_DatabasePath);
                }

                string connectionString = $"Data Source={m_DatabasePath}";
                using (var db = new SQLiteConnection(connectionString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        string uuid = Guid.NewGuid().ToString();

                        command.CommandText = "CREATE TABLE IF NOT EXISTS User(Key TEXT PRIMARY KEY, FirstName TEXT, LastName TEXT, UserID TEXT, " +
                            "Status TEXT, Role TEXT, Department TEXT, Phone TEXT, Email TEXT, Password TEXT, NeedResetPW, AdminAlarm BOOLEAN, " +
                            "PWChangeDate TEXT, PWRetry)";
                        command.ExecuteNonQuery();
                        // 동일한 ID 검색
                        command.CommandText = "SELECT EXISTS(SELECT * FROM User WHERE UserID = @ID)";
                        command.Parameters.AddWithValue("@ID", userID);
                        object isDataExist = command.ExecuteScalar();
                        int nDataExist = Convert.ToInt32(isDataExist);
                        if ((int)nDataExist == 0)
                        {
                            // 없으면 생성
                            command.CommandText = "INSERT INTO User (Key, FirstName, LastName, UserID, Status, Role, Department, Phone, Email, Password, NeedResetPW, AdminAlarm, " +
                                "PWChangeDate, PWRetry) " +
                                "VALUES (@key, @firstName, @lastName, @userID, @status, @role, @department, @phone, @email, @password, @needResetPW, @AdminAlarm," +
                                "@pwChangeDate, @pwRetry)";
                            command.Parameters.AddWithValue("@key", uuid);
                            command.Parameters.AddWithValue("@firstName", firstname);
                            command.Parameters.AddWithValue("@lastName", lastname);
                            command.Parameters.AddWithValue("@userID", userID);
                            command.Parameters.AddWithValue("@status", status);
                            command.Parameters.AddWithValue("@role", role);
                            command.Parameters.AddWithValue("@department", department);
                            command.Parameters.AddWithValue("@phone", phone);
                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@password", password);
                            command.Parameters.AddWithValue("@needResetPW", needResetPW);
                            command.Parameters.AddWithValue("@AdminAlarm", adminAlarm);
                            command.Parameters.AddWithValue("@pwChangeDate", pwChangeDate);
                            command.Parameters.AddWithValue("@pwRetry", pwRetry);
                            command.ExecuteNonQuery();
                        }

                        else
                        {
                            MessageBox.Show("Duplicated User ID exists.");
                        }
                    }
                    db.Close();
                }
            }

            catch
            {

            }
        }

        public void ModifyUserDB(string UUID, string firstName, string lastName, string status, string role, string department,
            string phone, string email, string password, string pwChangeDate, string pwRetry)
        {
            try
            {
                if (!File.Exists(m_DatabasePath))
                {
                    return;
                }

                string connectionString = $"Data Source={m_DatabasePath}";
                using (var db = new SQLiteConnection(connectionString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE User SET FirstName = @firstName, LastName = @lastName, Status = @status, " +
                            "Role = @role, Department = @department, Phone = @phone, Email = @email, Password = @password, PWChangeDate = @pwChangeDate, PWRetry = @pwRetry WHERE Key = @key";
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@department", department);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@pwChangeDate", pwChangeDate);
                        command.Parameters.AddWithValue("@pwRetry", pwRetry);
                        command.Parameters.AddWithValue("@key", UUID);
                        command.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }

            catch
            {

            }
        }

        public void ModifyDBPWRetry(string userID, string status, string pwRetry)
        {
            try
            {
                if (!File.Exists(m_DatabasePath))
                {
                    return;
                }

                string connectionString = $"Data Source={m_DatabasePath}";
                using (var db = new SQLiteConnection(connectionString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE User SET Status = @status, PWRetry = @pwRetry WHERE UserID = @userID";
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@pwRetry", pwRetry);
                        command.Parameters.AddWithValue("@userID", userID);
                        command.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }

            catch
            {

            }
        }

        public void ModifyPassword(string userID, string password)
        {
            try
            {
                if (!File.Exists(m_DatabasePath))
                {
                    return;
                }

                string connectionString = $"Data Source={m_DatabasePath}";
                using (var db = new SQLiteConnection(connectionString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE User SET Password = @password, NeedResetPW = @needResetPW, PWChangeDate = @pwChangeDate WHERE UserID = @userID";
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@needResetPW", false);
                        command.Parameters.AddWithValue("@pwChangeDate", DateTime.Now.ToString());
                        command.Parameters.AddWithValue("@userID", userID);
                        command.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }

            catch
            {

            }
        }

        public void ModifyAdminAlarm(bool isAlarmed)
        {
            try
            {
                if (!File.Exists(m_DatabasePath))
                {
                    return;
                }

                string connectionString = $"Data Source={m_DatabasePath}";
                using (var db = new SQLiteConnection(connectionString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE User SET AdminAlarm = @alarmed WHERE Role = @role";
                        command.Parameters.AddWithValue("@alarmed", isAlarmed);
                        command.Parameters.AddWithValue("@role", "Administrator");
                        command.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }

            catch
            {

            }
        }

        public string GetUserStatus(string userID)
        {
            if (userID == "")
                return "";

            List<UserInfo> users = new List<UserInfo>();
            users.AddRange(listUser.Where(p => p.userID == userID).ToList());

            if (users.Count != 1)
                return "";

            return users[0].status;
        }

        public int GetPWRetry(string userID)
        {
            int nPWRetry = 0;

            if (userID == "")
                return -1;

            List<UserInfo> users = new List<UserInfo>();
            users.AddRange(listUser.Where(p => p.userID == userID).ToList());

            if (users.Count != 1)
                return -1;

            if (int.TryParse(users[0].Password_retry, out nPWRetry))
                return nPWRetry;

            else
                return -1;
        }

        public bool IsNeedChangePassword(string userID)
        {
            List<UserInfo> users = new List<UserInfo>();
            users.AddRange(listUser.Where(p => p.userID == userID).ToList());

            if (users.Count != 1)
                return false;

            if (users[0].needResetPW)
                return true;

            DateTime now = DateTime.Now;
            DateTime Password_Changed_date = Convert.ToDateTime(users[0].Password_Changed_Date);

            return false;
        }

        public bool IsAdminAlarm(string userID)
        {
            List<UserInfo> users = new List<UserInfo>();
            users.AddRange(listUser.Where(p => p.userID == userID).ToList());
            if (users.Count != 1)
                return false;
            return users[0].adminAlarm;
        }

        public bool UserLogin(string userID, string password)
        {
            List<UserInfo> users = new List<UserInfo>();
            users.AddRange(listUser.Where(p => p.userID == userID).ToList());

            if (users.Count != 1)
            {
                m_loggedInUser.userID = "";
                return false;
            }

            m_loggedInUser = users[0];
            if (m_loggedInUser.password != password)
            {
                m_loggedInUser.userID = "";
                return false;
            }

            else
            {
                return true;
            }
        }

        public void UserLogout()
        {
            m_loggedInUser.userID = "";
        }

        public bool LoadAllUsers()
        {
            try
            {
                listUser.Clear();
                if (!File.Exists(m_DatabasePath))
                {
                    DateTime now = DateTime.Now;
                    InsertUserDB("Admin", "Admin", "Admin", "Active", "Administrator", "", "", "", "1234", false, false, now.ToString(), "0");
                }

                string connectionString = $"Data Source={m_DatabasePath}";
                using (var db = new SQLiteConnection(connectionString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM User";
                        ;
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserInfo user = new UserInfo();
                                user.key = reader.GetString(0);
                                user.firstName = reader.GetString(1);
                                user.lastName = reader.GetString(2);
                                user.userID = reader.GetString(3);
                                user.status = reader.GetString(4);
                                user.role = reader.GetString(5);
                                user.department = reader.GetString(6);
                                user.phone = reader.GetString(7);
                                user.email = reader.GetString(8);
                                user.password = reader.GetString(9);
                                user.needResetPW = reader.GetBoolean(10);
                                user.adminAlarm = reader.GetBoolean(11);
                                user.Password_Changed_Date = reader.GetString(12);
                                user.Password_retry = reader.GetString(13);
                                listUser.Add(user);
                            }
                        }
                    }
                    db.Close();
                }
                return true;
            }

            catch
            {
                return false;
            }
        }

        public bool LoadUserInfo(string UserID, ref UserInfo user)
        {
            List<UserInfo> list = listUser;
            List<UserInfo> selected = new List<UserInfo>();
            selected = list.Where(p => p.userID == UserID).ToList();
            if (selected.Count != 1)
            {
                return false;
            }

            user = selected[0];
            return true;
        }

        public void DeleteUser(string UserId)
        {
            try
            {
                if (!File.Exists(m_DatabasePath))
                {
                    return;
                }

                string connectionString = $"Data Source={m_DatabasePath}";
                using (var db = new SQLiteConnection(connectionString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM User WHERE UserID = @userID";
                        command.Parameters.AddWithValue("@userID", UserId);
                        command.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }

            catch
            {

            }
        }
    }
}
