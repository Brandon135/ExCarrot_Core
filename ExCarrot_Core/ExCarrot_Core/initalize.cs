using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ExCarrot_Core
{
    public static partial class Main
    {

        public static event EventHandler DBConnectionFaliure;
        public static event EventHandler DBConnectionRecover;
        internal static ICarrotModule CoreCredential = new CoreCredential();
        internal static SQL SQLModule;

        internal static Dictionary<string, CloudVExtensionBase> ExtensionList;

        public static CloudVExtensionBase GetExtension(string ModuleName)
        {
            if (Internal_Variables.IsInited != true)
            {
                throw new CloudVException(CloudVExceptionType.InvalidCall.ToString());
            }

            if (ExtensionList.ContainsKey(ModuleName) != true)
            {
                throw new CloudVException(CloudVExceptionType.InternalError.ToString());
            }

            return ExtensionList[ModuleName];
        }

        internal static void Event_NullSender(EventHandler Handler, EventArgs Args)
        {
            var handler = Handler;
            if (handler != null)
                handler(null, Args);
        }

        internal static void Event_WithSender(EventHandler Handler, EventArgs Args, object Sender)
        {
            var handler = Handler;
            if (handler != null)
                handler(Sender, Args);
        }

        internal static void SQLEvent(bool type)
        {
            if (type)
            {
                Event_NullSender(DBConnectionRecover, EventArgs.Empty);
            }
            else
            {
                Event_NullSender(DBConnectionFaliure, EventArgs.Empty);
            }
        }

        private static void initializeModule(string[] DependencyList)
        {
            CoreLog("Loading Modules..", Log.LogType.Info);


            if (Internal_Variables.IsInited)
            {
                throw new CloudVException(CloudVExceptionType.InvalidCall.ToString());
            }

            string[] dllFileNames = null;
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"\extensions";

            if (Directory.Exists(Path))
            {
                dllFileNames = Directory.GetFiles(Path, "*.dll");
            }
            else
            {
                CoreLog("No Extensions Available.", Log.LogType.Info);
                return;
            }

            ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
            foreach (string dllFile in dllFileNames)
            {
                AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                Assembly assembly = Assembly.Load(an);
                assemblies.Add(assembly);

            }

            Type pluginType = typeof(ICloudVExtension);
            ICollection<Type> pluginTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    try
                    {
                        Type[] types = assembly.GetTypes();

                        foreach (Type type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {

                                if (type.GetInterface(pluginType.FullName) != null)
                                {

                                    pluginTypes.Add(type);
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Log.ExceptionHandler(CoreCredential, e);
                        throw new CloudVException(CloudVExceptionType.InternalError.ToString());
                    }
                }
            }


            ExtensionList = new Dictionary<string, CloudVExtensionBase>();
            foreach (Type type in pluginTypes)
            {
                CloudVExtensionBase plugin = (CloudVExtensionBase)Activator.CreateInstance(type);
                ICloudVExtension Arg = (ICloudVExtension)plugin;

                if (Arg.APILevel != null)
                {

                    if (Arg.APILevel == Internal_Variables.CurrentAPILevel == false)
                    {
                        CoreLog("APILevel Mismatch :" + Arg.ModuleName, Log.LogType.Warning);
                        UI.Dialog.BasicDialog.ShowPrompt(UI.Dialog.Base.Error, "CloudV(TM) 시스템 오류",
                            String.Format(@"확장 프로그램 '{0}' 은 현재 CloudV(TM) Core 버전에서 호환되지 않습니다.", Arg.ModuleName), false,
                            false);
                        break;
                    }

                }


                if (ExtensionList.ContainsKey(Arg.ModuleName))
                {
                    CoreLog("Already Loaded Extension :" + Arg.ModuleName, Log.LogType.Warning);
                    break;
                }

                try
                {
                    plugin.Initalize();
                }
                catch (Exception e)
                {
                    Log.ExceptionHandler(plugin, e);
                    CoreLog("Extension initialize Failure :" + Arg.ModuleName, Log.LogType.Warning);
                    Report R = new Report();
                    R.MakeReport_Exception(e, plugin, "확장 프로그램을 초기화 하는 중 예외가 발생하였습니다.");
                    UI.Dialog.ReportDialog.ShowReport(UI.Dialog.Base.Warning, "CloudV(TM) 시스템 경고",
                        "확장 프로그램을 초기화 하는 도중 오류가 발생하여, " + Environment.NewLine + "확장 프로그램을 로드 할 수 없습니다.",
                        "확장 프로그램 로드 실패", R.ExportReportData(), false);
                    break;
                }

                CoreLog("Extension initialized :" + Arg.ModuleName, Log.LogType.Warning);
                ExtensionList.Add(Arg.ModuleName, plugin);
            }

            if (DependencyList != null)
            {
                foreach (string Dependency in DependencyList)
                {
                    CoreLog(String.Format("Checking Dependency '{0}'", Dependency), Log.LogType.Info);
                    if (ExtensionList.ContainsKey(Dependency) != true)
                    {

                        CoreLog(String.Format("Dependency Error : Can't Find Extension '{0}'", Dependency), Log.LogType.Error);
                        UI.Dialog.BasicDialog.ShowPrompt(UI.Dialog.Base.Error, "CloudV(TM) 시스템 오류",
                            String.Format(@"플렛폼 의존성 '{0}' 패키지가 누락되었기 때문에, 현재 플렛폼을 시작할 수 없습니다.", Dependency), false,
                            false);
                        throw new InitializeException(InitializeExceptionType.DependencyError.ToString());
                    }
                }
            }

            if (ExtensionList.Count < 0)
            {
                CoreLog("No Extensions Available.", Log.LogType.Info);
            }
            else
            {
                CoreLog("Total Extensions : " + ExtensionList.Count().ToString(), Log.LogType.Info);
            }


        }

        public static async Task Initialize(InitializeArgument IA)
        {
            CoreLog("CloudV Core Service : Version " + Internal_Variables.CurrentVersion, Log.LogType.Info);
            CoreLog("Platform Service : " + IA.ServiceName, Log.LogType.Info);
            CoreLog("Initialize...", Log.LogType.Info);
            Internal_Variables.PlatformInfomation = IA;

            if (Internal_Variables.IsInited)
            {
                CoreLog("CloudV Core is already Initialized.", Log.LogType.Error);
                throw new InitializeException(InitializeExceptionType.AlreadyInitalized.ToString());
            }

            if (IA == null)
            {
                CoreLog("Invalid InitializeArgument", Log.LogType.Error);
                throw new InitializeException(InitializeExceptionType.InvalidArgument.ToString());
            }

            Internal_Variables.ServiceName = IA.ServiceName;
            Internal_Variables.UsingDBModule = IA.IsUseDB;
            List<string> DependencyList = IA.DependencyList;


            if (IA.SafeMod)
            {
                CoreLog("The extension is not loaded because it is initializing in safe mode.", Log.LogType.Warning);
            }
            else
            {
                initializeModule(DependencyList.ToArray());
            }


            CoreLog("Initalizing Complete", Log.LogType.Info);
            Internal_Variables.IsInited = true;

        }

        public static async Task SQLInitialize(SQLInitializeArgument SIA)
        {
            CoreLog("SQLInitialize...", Log.LogType.Info);
            if (Internal_Variables.IsInited != true || Internal_Variables.UsingDBModule != true)
            {
                CoreLog("CloudV Core is not initialized.", Log.LogType.Error);
                throw new CloudVException(CloudVExceptionType.InvalidCall.ToString());
            }


            if (SIA == null)
            {
                CoreLog("Invalid SQLInitializeArgument", Log.LogType.Info);
                throw new InitializeException(InitializeExceptionType.InvalidArgument.ToString());
            }

            SQLModule = new SQL(SIA);

            SQLModule.ConnectToSQLServer();
            CoreLog("SQL initialize Complete", Log.LogType.Info);
        }

        internal static void CoreLog(string Content, Log.LogType LT)
        {
            Log.Begin(CoreCredential, Content, LT);
        }

    }


    internal class CoreCredential : ICarrotModule
    {
        string ICarrotModule.ModuleName { get { return "CloudVCore"; } }
        string ICarrotModule.ModuleDesc { get { return "CloudV Core Service"; } }
        bool ICarrotModule.IsExtension { get { return false; } }
    }
}
