using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Cryopreserved_Manager.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Windows.Documents;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using System.Windows;

namespace Cryopreserved_Manager.Services
{
    public interface IUserManagementService
    {
        bool LoadAllUsers();
        void InsertUserDB(string firstname, string lastname, string userID, string status, string role, string department, 
            string phone, string email, string password, bool needResetPW, bool adminAlarm, string pwChangeDate, string pwRetry);
        void ModifyUserDB(string UUID, string firstName, string lastName, string status, string role, string department,
            string phone, string email, string password, string pwChangeDate, string pwRetry);
        void ModifyDBPWRetry(string userID, string status, string pwRetry);
        void ModifyPassword(string userID, string password);
        void DeleteUser(string userID);
        int GetPWRetry(string userID);
        string GetUserStatus(string userID);
        bool IsNeedChangePassword(string userID);
        bool UserLogin(string userID, string password);
        UserInfo GetLoggedInUser();
        List<UserInfo> GetUserList();
    }

    public class UserManagementService : IUserManagementService
    {
        private static string m_DatabasePath = "C:\\ProgramData\\Cryopreserved\\etc\\user.db";
        private static string MyConString = "Server=127.0.0.1;Port=3306;Database=AppUsers;Uid=root;pwd=1234;";
        private UserInfo m_loggedInUser = new UserInfo();
        static private List<UserInfo> listUser = new List<UserInfo>();

        public UserManagementService()
        {
            InitializeDataBase();
        }
        public string DatabasePath { get { return m_DatabasePath; } }

        public UserInfo GetLoggedInUser()
        {
            return m_loggedInUser;
        }
        public List<UserInfo> GetUserList()
        {
            return listUser;
        }

        private void InitializeDataBase()
        {
            try
            {
                using (var db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    InsertUserDB("Admin", "Admin", "Admin", "Active", "Administrator", "", "", "", "1234", false, false, DateTime.Now.ToString(), "0");
                }
            }
            catch
            {
                string connectionString = $"Server=127.0.0.1;Port=3306;Uid=root;Pwd=1234;";
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS AppUsers;";
                    cmd.ExecuteNonQuery();
                }

                using (var db = new MySqlConnection(MyConString))
                {
                    InsertUserDB("Admin", "Admin", "Admin", "Active", "Administrator", "", "", "", "1234", false, false, DateTime.Now.ToString(), "0");
                }
            }
        }

        public void InsertUserDB(string firstname, string lastname, string userID,
            string status, string role, string department, string phone, string email, string password, bool needResetPW, bool adminAlarm, string pwChangeDate, string pwRetry)
        {
            try
            {
                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        string uuid = Guid.NewGuid().ToString();

                        command.CommandText = "CREATE TABLE IF NOT EXISTS AppUsers(UserKey VARCHAR(255) PRIMARY KEY, FirstName VARCHAR(100), LastName VARCHAR(100), UserID VARCHAR(100), " +
                            "Status VARCHAR(50), Role VARCHAR(50), Department VARCHAR(100), Phone VARCHAR(20), Email VARCHAR(100), Password VARCHAR(255), NeedResetPW TINYINT(1), " +
                            "AdminAlarm TINYINT(1), PWChangeDate VARCHAR(50), PWRetry INT);";
                        command.ExecuteNonQuery();
                        // 동일한 ID 검색
                        command.CommandText = "SELECT EXISTS(SELECT * FROM AppUsers WHERE UserID = @ID)";
                        command.Parameters.AddWithValue("@ID", userID);
                        object isDataExist = command.ExecuteScalar();
                        int nDataExist = Convert.ToInt32(isDataExist);
                        if ((int)nDataExist == 0)
                        {
                            // 없으면 생성
                            command.CommandText = "INSERT INTO AppUsers (UserKey, FirstName, LastName, UserID, Status, Role, Department, Phone, Email, Password, NeedResetPW, AdminAlarm, " +
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
                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE AppUsers SET FirstName = @firstName, LastName = @lastName, Status = @status, " +
                            "Role = @role, Department = @department, Phone = @phone, Email = @email, Password = @password, PWChangeDate = @pwChangeDate, PWRetry = @pwRetry WHERE UserKey = @key";
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
                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE AppUsers SET Status = @status, PWRetry = @pwRetry WHERE UserID = @userID";
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
                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE AppUsers SET Password = @password, NeedResetPW = @needResetPW, PWChangeDate = @pwChangeDate WHERE UserID = @userID";
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
                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "UPDATE AppUsers SET AdminAlarm = @alarmed WHERE Role = @role";
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
            users.AddRange(listUser.Where(p => p.UserID == userID).ToList());

            if (users.Count != 1)
                return "";

            return users[0].Status;
        }

        public int GetPWRetry(string userID)
        {
            int nPWRetry = 0;

            if (userID == "")
                return -1;

            List<UserInfo> users = new List<UserInfo>();
            users.AddRange(listUser.Where(p => p.UserID == userID).ToList());

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
            users.AddRange(listUser.Where(p => p.UserID == userID).ToList());

            if (users.Count != 1)
                return false;

            if (users[0].NeedResetPW)
                return true;

            DateTime now = DateTime.Now;
            DateTime Password_Changed_date = Convert.ToDateTime(users[0].Password_Changed_Date);

            return false;
        }

        public bool IsAdminAlarm(string userID)
        {
            List<UserInfo> users = new List<UserInfo>();
            users.AddRange(listUser.Where(p => p.UserID == userID).ToList());
            if (users.Count != 1)
                return false;
            return users[0].AdminAlarm;
        }

        public bool UserLogin(string userID, string password)
        {
            List<UserInfo> users = new List<UserInfo>();
            users.AddRange(listUser.Where(p => p.UserID == userID).ToList());

            if (users.Count != 1)
            {
                m_loggedInUser.UserID = "";
                return false;
            }

            m_loggedInUser = users[0];
            if (m_loggedInUser.Password != password)
            {
                m_loggedInUser.UserID = "";
                return false;
            }

            else
            {
                return true;
            }
        }

        public void UserLogout()
        {
            m_loggedInUser.UserID = "";
        }

        public bool LoadAllUsers()
        {
            listUser.Clear();

            try
            {
                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM AppUsers";
                        
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserInfo user = new UserInfo();
                                user.Key = reader.GetString(0);
                                user.FirstName = reader.GetString(1);
                                user.LastName = reader.GetString(2);
                                user.UserID = reader.GetString(3);
                                user.Status = reader.GetString(4);
                                user.Role = reader.GetString(5);
                                user.Department = reader.GetString(6);
                                user.Phone = reader.GetString(7);
                                user.Email = reader.GetString(8);
                                user.Password = reader.GetString(9);
                                user.NeedResetPW = reader.GetBoolean(10);
                                user.AdminAlarm = reader.GetBoolean(11);
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
            selected = list.Where(p => p.UserID == UserID).ToList();
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

                using (MySqlConnection db = new MySqlConnection(MyConString))
                {
                    db.Open();
                    using (var command = db.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM AppUsers WHERE UserID = @userID";
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
