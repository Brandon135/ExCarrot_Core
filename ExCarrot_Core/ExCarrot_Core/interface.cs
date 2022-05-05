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
        public bool UseSSLConnection;

        public int WatchdogRefreshTime;
        public string ServerDomain;
        public string DBName;
        public string ID;
        public string PW;

        public SQLInitializeArgument(string ServerDomain, string DBName, string ID, string PW, bool UseSSLConnection)
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

            this.ServerDomain = Argument[0];
            this.DBName = Argument[1];
            this.ID = Argument[2];
            this.PW = Argument[3];
            this.UseSSLConnection = UseSSLConnection;
        }

        internal string GetConnectionString()
        {
            string BaseConnectionString = String.Format("Server={0};Uid={1};Pwd={2};Database={3};", ServerDomain, ID, PW, DBName);

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

    internal interface ICarrotModule
    {
        bool IsExtension { get; }
        string ModuleName { get; }
        string ModuleDesc { get; }


    }

    internal interface ICarrotExtension : ICarrotModule
    {
        void Initalize();
        string Production { get; }
    }

    public abstract class CloudVDatabaseModuleBase : ExCarrotModuleBase
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

    public class CloudVExtensionBase : ExCarrotExtension 
    { 
        private string _ModuleName;
        private string _ModuleDesc;
        private string _Production;


        string ExCarrotModule.ModuleName { get { return _ModuleName; } }
        string ExCarrotModule.ModuleDesc { get { return _ModuleDesc; } }
        bool ExCarrotModule.IsExtension { get { return true; } }

        public CloudVExtensionBase(string ModuleName, string ModuleDesc, string Production)
        {
            _ModuleName = ModuleName;
            _ModuleDesc = ModuleDesc;
            _Production = Production;
        }

        string ExCarrotExtension.Production
        {
            get { return _Production; }
        }

        public virtual void Initalize()
        {

        }
    }

    public abstract class ExCarrotModuleBase : ExCarrotModule, IDisposable
    {


        private string ModuleName;
        private string ModuleDesc;

        string ExCarrotModule.ModuleName { get { return ModuleName; } }
        string ExCarrotModule.ModuleDesc { get { return ModuleDesc; } }
        bool ExCarrotModule.IsExtension { get { return false; } }

        private bool disposed;

        public void Dispose()
        {
            //Log(ExCarrot_Core.Log.LogType.Info, "Instance Disposing..."); 로그 작성
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

        public ExCarrotModuleBase(string ModuleName, string ModuleDesc)
        {
            this.ModuleName = ModuleName;
            this.ModuleDesc = ModuleDesc;
            //Log(ExCarrot_Core.Log.LogType.Info, "Created New Instance.");
        }

        public abstract void Unloading();

        /*internal protected void Log(Log.LogType LT, string Content)
        {
            ExCarrot_Core.Log.WriteLog(this, Content, LT);
        }*/
    }

}
