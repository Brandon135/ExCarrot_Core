using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.

namespace ExCarrot_Core
{
    internal class SQL
    {

        private MySqlConnection SQLConn;
        private SQLInitializeArgument IA;
        private bool DBConnectionStatus;
        DispatcherTimer WatchdogsTimer;

        public SQL(SQLInitializeArgument IA)
        {
            Main.CoreLog("Initalize new SQL instance..", Log.LogType.Info);
            if (Internal_Variables.IsInited != true || Internal_Variables.UsingDBModule != true)
            {
                throw new CloudVException(CloudVExceptionType.InvalidCall.ToString());
            }


            this.IA = IA;

        }


        private void DBCheck(object sender, EventArgs E)
        {
            if (SQLConn.State == System.Data.ConnectionState.Closed || SQLConn.State == System.Data.ConnectionState.Broken)
            {
                Main.CoreLog("MySQLServer Watchdog Faliure", Log.LogType.Error);
                DBConnectionStatus = false;
                Main.SQLEvent(false);
                return;
            }

            if (DBConnectionStatus != true)
            {
                Main.CoreLog("Try to reconnect MySQLServer..", Log.LogType.Info);
                try
                {
                    SQLConn.Open();
                }
                catch
                {
                    Main.CoreLog("Falied To Reconnect MySQLServer.", Log.LogType.Error);
                    return;
                }

                Main.CoreLog("MySQLServer Connection restored.", Log.LogType.Info);
                DBConnectionStatus = true;
                Main.SQLEvent(true);

            }



        }

        internal void CloseConnection()
        {
            Main.CoreLog("Closing MySQL Connection..", Log.LogType.Info);
            WatchdogsTimer.Stop();
            SQLConn.Close();

        }

        internal async void ConnectToSQLServer()
        {

            Main.CoreLog("Connecting SQL Server..", Log.LogType.Info);
            SQLConn = new MySqlConnection(IA.GetConnectionString());

            try
            {
                await SQLConn.OpenAsync();
            }
            catch (Exception e)
            {
                Main.CoreLog("Failed To Connect SQL Server :" + e.ToString(), Log.LogType.Error);
                SQLConn = null;
                throw new CloudVException(CloudVExceptionType.InternalError.ToString(), e);
            }

            if (IA.UseWatchdog)
            {
                WatchdogsTimer = new DispatcherTimer();
                WatchdogsTimer.Interval = TimeSpan.FromMilliseconds(IA.WatchdogRefreshTime);
                WatchdogsTimer.Tick += new EventHandler(DBCheck);
                WatchdogsTimer.Start();
            }

            Internal_Variables.IsDBConnected = true;
            DBConnectionStatus = true;

        }

        public bool ServerConnectionStatus()
        {
            if (SQLConn == null)
            {
                throw new CloudVException(CloudVExceptionType.InvalidCall.ToString());
            }

            return SQLConn.Ping();

        }

        public MySqlConnection GetMySQLConnection(ICloudVModule Module)
        {
            Main.CoreLog(Module.ModuleName.ToString() + " has Requested MySQLConnection object.", Log.LogType.Warning);

            if (SQLConn == null || Internal_Variables.IsDBConnected != true)
            {
                Main.CoreLog("SecurityCheck Faliure.", Log.LogType.Error);
                throw new CloudVException(CloudVExceptionType.InvalidCall.ToString());
            }

            if (Module.IsExtension)
            {
                try
                {
                    Security.DB_SecureScreen((ICloudVExtension)Module);
                }
                catch
                {
                    Main.CoreLog("Permission Denied.", Log.LogType.Error);
                    throw new CloudVException(CloudVExceptionType.Unauthorized.ToString());
                }

            }

            return SQLConn;
        }

    }
}

