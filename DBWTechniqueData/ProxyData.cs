using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DBWTechniqueData.Models;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

namespace DBWTechniqueData
{
    public class ProxyData
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        public List<ProxyModel> GetProxyFromDatabase()
        {
            List<ProxyModel> PModel = new List<ProxyModel>();
            try
            {
                string query = "SELECT * FROM `proxy` WHERE IsDeleted = 0 LIMIT 100";
                
                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        PModel.Add(new ProxyModel
                        {
                            ID = (int)reader["ID"],
                            IpPort = reader["IpPort"].ToString(),
                            Ip = reader["Ip"].ToString(),
                            Port = reader["Port"].ToString(),
                            Status = (int)reader["Status"],
                            Country = reader["Country"].ToString(),
                            Type = reader["Type"].ToString(),
                            LastChecked = (DateTime)reader["LastChecked"],
                            FoundDate = (DateTime)reader["ProxyFoundDate"]
                        });
                    }

                    //close connection
                    this.CloseConnection();
                }
            }
            catch(Exception)
            {

            }
            return PModel;
        }

        public void GetProxyFromDatabaseAndUpdate()
        {
            try
            {
                string query = "SELECT * FROM `proxy` WHERE IsDeleted = 0 LIMIT 100";

                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        //updateProxyStatus((int)reader["ID"], PingFunction(reader["Ip"].ToString(), (int)reader["Port"]), DateTime.Now);
                        updateProxyStatus((int)reader["ID"], 1 , DateTime.Now);
                    }

                    //close connection
                    this.CloseConnection();
                }
            }
            catch (Exception)
            {

            }
        }

        public void updateProxyStatus(int Id, int Status, DateTime LastChecked)
        {
            try
            {
                string query = "UPDATE proxy SET Status= " + Status + ",LastChecked= '" + LastChecked.ToString("yyyy-MM-dd HH:mm:ss") + "', `LastInactiveSet`= NULL WHERE ID = " + Id + "";
                if (Status==0)
                {
                    query = "UPDATE proxy SET Status= " + Status + ",LastChecked= '" + LastChecked.ToString("yyyy-MM-dd HH:mm:ss") + "',LastInactiveSet= '" + LastChecked.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE ID = " + Id + " AND LastInactiveSet IS NULL";
                }

                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                }
            }
            catch(Exception)
            {

            }
        }

        public void storeProxyFromProviderList(string ipport, string ip, string port, string country, string type, int Status, string ProxySource)
        {
            try
            {
                if(checkIfProxyAlreadyExistInDatabase(ipport)==0)
                {
                    string query = "INSERT INTO `proxy`(`IpPort`, `Ip`, `Port`, `Country`, `LastChecked`, `Type`, `Status`, `ProxyFoundDate`, `ProxySource`, `IsDeleted`) VALUES ('" + ipport + "','" + ip + "','" + port + "','" + country + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + type + "', '" + Status + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ProxySource + "','0')";
                    if (Status == 0)
                    {
                        query = "INSERT INTO `proxy`(`IpPort`, `Ip`, `Port`, `Country`, `LastChecked`, `Type`, `Status`, `ProxyFoundDate`, `ProxySource`, `IsDeleted`,`LastInactiveSet`) VALUES ('" + ipport + "','" + ip + "','" + port + "','" + country + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + type + "', '" + Status + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ProxySource + "','0','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    }

                    //open connection
                    if (this.OpenConnection() == true)
                    {
                        //create command and assign the query and connection from the constructor
                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        //Execute command
                        cmd.ExecuteNonQuery();

                        //close connection
                        this.CloseConnection();
                    }
                }
                else
                {
                   
                }
            }
            catch (Exception)
            {

            }
        }

        public int countNumberOfProxiesInDatabase()
        {
            int proxyCount = 0;
            try
            {
                string query = "SELECT Count(ID) as ProxyCount FROM `proxy` WHERE Type='http' and IsDeleted=0";

                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        proxyCount = Convert.ToInt32(reader["ProxyCount"]);
                    }

                    //close connection
                    this.CloseConnection();
                }
            }
            catch (Exception)
            {

            }
            return proxyCount;
        }

        public void deleteAndClearProxy()
        {
            try
            {
                string query = "DELETE FROM `proxy`";

                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                }
            }
            catch (Exception)
            {

            }
        }

        public int checkIfProxyAlreadyExistInDatabase(string IpPort)
        {
            int Exist = 0;
            try
            {
                string query = "SELECT Count(ID) as ProxyCount FROM `proxy` WHERE IpPort='" + IpPort + "' and IsDeleted=0";

                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Exist = Convert.ToInt32(reader["ProxyCount"]);
                    }

                    //close connection
                    this.CloseConnection();
                }
            }
            catch (Exception)
            {

            }
            return Exist;
        }
        
        public void findOneWeekOldNonActiveProxy()
        {
            //List<ProxyModel> PModel = new List<ProxyModel>();
            try
            {
                string query = "SELECT * FROM `proxy` WHERE Status = 0 and IsDeleted = 0 LIMIT 100";

                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        deleteOneWeekOldNonActiveProxy((int)reader["ID"], (int)reader["Status"], (DateTime)reader["LastInactiveSet"]);
                    };

                    //close connection
                    this.CloseConnection();
                }
            }
            catch (Exception)
            {

            }
        }

        public void deleteOneWeekOldNonActiveProxy(int Id, int Status, DateTime lastChecked)
        {
            try
            {
                double totalDays = (DateTime.Now - lastChecked).TotalDays;
                if (totalDays > 7.0)
                {
                    string query = "UPDATE proxy SET IsDeleted= 1 WHERE ID = " + Id + "";

                    //open connection
                    if (this.OpenConnection() == true)
                    {
                        //create command and assign the query and connection from the constructor
                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        //Execute command
                        cmd.ExecuteNonQuery();

                        //close connection
                        this.CloseConnection();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        #region databaseConfigurations
        /// <summary>
        /// Initilize connection to database
        /// </summary>
        public void InitilizeConncetion()
        {
            server = "localhost";
            database = "dbwtproject";
            uid = "root";
            password = "!tex!a-1213";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Open connection to database
        /// </summary>
        /// <returns></returns>
        private bool OpenConnection()
        {
            InitilizeConncetion();
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        //MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        //MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        /// <summary>
        /// Close connection
        /// </summary>
        /// <returns></returns>
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
        }
        #endregion

        #region Functions
        public int PingFunction(string host, int port)
        {
            int is_success = 0;
            try
            {
                var connsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                connsock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 200);
                //System.Threading.Thread.Sleep(500);
                var hip = IPAddress.Parse(host);
                var ipep = new IPEndPoint(hip, port);
                connsock.Connect(ipep);
                if (connsock.Connected)
                {
                    is_success = 1;
                }
                connsock.Close();
            }
            catch (Exception)
            {
                is_success = 0;
            }
            return is_success;
        }

        #endregion
    }
}
