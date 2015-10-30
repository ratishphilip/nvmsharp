using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVMSharp.Common
{
    internal static class RegistryManager
    {
        public static void SetEnvironmentVariable(string varName, string varValue, EnvironmentVariableTarget varType)
        {
            Environment.SetEnvironmentVariable(varName, varValue, varType);
        }

        public static void DeleteEnvironmentVariable(string varName, EnvironmentVariableTarget varType)
        {
            Environment.SetEnvironmentVariable(varName, null, varType);
        }

        public static void SaveEnvironmentVariable(EnVar enVar, EnvironmentVariableTarget varType)
        {
            Environment.SetEnvironmentVariable(enVar.Key, enVar.Data, varType);
        }

        public static void DeleteEnvironmentVariable(EnVar enVar, EnvironmentVariableTarget varType)
        {
            Environment.SetEnvironmentVariable(enVar.Key, null, varType);
        }
    }
}
