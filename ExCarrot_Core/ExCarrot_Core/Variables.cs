using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExCarrot_Core
{
    // 내부 변수
    internal static class Internal_Variables
    {

        internal static bool IsInited = false;
        internal static bool IsDBConnected = false;
        internal static bool UsingDBModule;
        internal static string ServiceName;
        internal static bool EnableLogFile = true;
        internal static bool DebugMode = true;
        internal static SecureScreenLevel CurrentSecureScreenLevel;
        internal static InitializeArgument PlatformInfomation;

    }

}