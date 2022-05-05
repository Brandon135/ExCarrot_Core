using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExCarrot_Core
{
    public class InitializeArgument
    {
        public string ServiceName;
        public bool IsUseDB;
        public bool SafeMod;
        public double Version;
        internal List<string> DependencyList = new List<string>();

        public InitializeArgument(string ServiceName, bool IsUseDB, bool EnableSafemod, double Version)
        {
            if (String.IsNullOrWhiteSpace(ServiceName))
            {
                throw new DataException(DataExceptionType.InvalidData.ToString());
            }

            this.ServiceName = ServiceName;
            this.IsUseDB = IsUseDB;
            this.SafeMod = EnableSafemod;
        }

        public void AddDependency(string ExtensionName)
        {
            if (DependencyList.Contains(ExtensionName))
            {
                throw new InitializeException(InitializeExceptionType.DependencyError.ToString());
            }

            DependencyList.Add(ExtensionName);
        }
    }

    public class SQLInitializeArgument
    {
        public bool UseWatchdog;
        public bool ShouldAutoReconnect;
        public bool UseSSLConnection;

        public int WatchdogRefreshTime;
        public string ServerDomain;
        public string DBName;
        public string ID;
        public string PW;
        public int Port;

        public SQLInitializeArgument(string ServerDomain, string DBName, string ID, string PW, int Port, bool UseWatchdog, bool ShouldAutoReconnect, bool UseSSLConnection)
        {
            string[] Argument = new string[3];
            Argument[0] = ServerDomain;
            Argument[1] = DBName;
            Argument[2] = ID;
            Argument[3] = PW;

            foreach (string Parameter in Argument)
            {
                if (String.IsNullOrWhiteSpace(Parameter))
                {
                    throw new DataException(DataExceptionType.InvalidData.ToString());
                }

            }

            if (Port <= 0 || Port >= 65535)
            {
                throw new DataException(DataExceptionType.InvalidData.ToString());
            }


            this.ServerDomain = Argument[0];
            this.DBName = Argument[1];
            this.ID = Argument[2];
            this.PW = Argument[3];

            this.UseWatchdog = UseWatchdog;
            this.ShouldAutoReconnect = ShouldAutoReconnect;
            this.UseSSLConnection = UseSSLConnection;
        }

        internal string GetConnectionString()
        {
            string BaseConnectionString = String.Format("server={0}port={1};uid={2};pwd={3};database={4};", ServerDomain, Port.ToString(), ID, PW, DBName);

            if (UseSSLConnection != true)
            {
                BaseConnectionString += "SslMode=none";
            }

            return BaseConnectionString;
        }
    }



    internal enum SecureScreenLevel
    {
        High,
        ExecutiveOnly,
        Disable,
    }

    internal interface ICloudVModule
    {
        bool IsExtension { get; }
        string ModuleName { get; }
        string ModuleDesc { get; }


    }

    internal interface ICloudVExtension : ICloudVModule
    {
        void Initalize();
        string Production { get; }
        int? APILevel { get; }
    }

    public abstract class CloudVDatabaseModuleBase : CloudVModuleBase
    {
        public CloudVDatabaseModuleBase(string ModuleName, string ModuleDesc, bool IsRequireDBConnection) : base(ModuleName, ModuleDesc)
        {
            if (Internal_Variables.UsingDBModule != true)
            {
                throw new CloudVException(CloudVExceptionType.InvalidCall.ToString());
            }

            if (IsRequireDBConnection)
            {
                if (Internal_Variables.IsDBConnected != true)
                {
                    throw new DataException(DataExceptionType.DBNotConnected.ToString());
                }
            }


        }


    }

    public class CloudVExtensionBase : ICloudVExtension
    {
        private string _ModuleName;
        private string _ModuleDesc;
        private string _Production;
        private int? _APILevel;


        string ICloudVModule.ModuleName { get { return _ModuleName; } }
        string ICloudVModule.ModuleDesc { get { return _ModuleDesc; } }
        bool ICloudVModule.IsExtension { get { return true; } }

        public CloudVExtensionBase(string ModuleName, string ModuleDesc, string Production, int? APILevel)
        {
            _ModuleName = ModuleName;
            _ModuleDesc = ModuleDesc;
            _APILevel = APILevel;
            _Production = Production;
        }

        string ICloudVExtension.Production
        {
            get { return _Production; }
        }

        int? ICloudVExtension.APILevel
        {
            get { return _APILevel; }
        }

        public virtual void Initalize()
        {

        }
    }

    public abstract class CloudVModuleBase : ICloudVModule, IDisposable
    {


        private string ModuleName;
        private string ModuleDesc;

        string ICloudVModule.ModuleName { get { return ModuleName; } }
        string ICloudVModule.ModuleDesc { get { return ModuleDesc; } }
        bool ICloudVModule.IsExtension { get { return false; } }

        private bool disposed;

        public void Dispose()
        {
            Log(CloudV_Core.Log.LogType.Info, "Instance Disposing...");
            Unloading();
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing)
            {

            }
            this.disposed = true;
        }

        public CloudVModuleBase(string ModuleName, string ModuleDesc)
        {
            this.ModuleName = ModuleName;
            this.ModuleDesc = ModuleDesc;
            Log(CloudV_Core.Log.LogType.Info, "Created New Instance.");
        }

        public abstract void Unloading();

        internal protected void Log(Log.LogType LT, string Content)
        {
            ExCarrot_Core.Log.WriteLog(this, Content, LT);
        }
    }

}
