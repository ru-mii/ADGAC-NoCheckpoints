using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using System.IO;
using System.Reflection;
using HarmonyLib;

namespace NoCheckpoints
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class NoCheckpoints : BaseUnityPlugin
    {
        #region Variables

        public const string pluginGuid = "35a4c8fa9e0e48488959afec25a1738b";
        public const string pluginName = "NoCheckpoints";
        public const string pluginVersion = "2.0.0";

        public static string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static BepInEx.Logging.ManualLogSource pLogger;
        Harmony harmony = new Harmony(pluginGuid);

        List<MethodInfo> original = new List<MethodInfo>();
        List<MethodInfo> patch = new List<MethodInfo>();

        #endregion Variables

        #region LocalFunctions

        private void Awake()
        {
            // static logger
            pLogger = Logger;

            // SaveSystemJ -> GetPlayerData()
            original.Add(AccessTools.Method(typeof(SaveSystemJ), "GetPlayerData"));
            patch.Add(AccessTools.Method(typeof(SaveSystemJ_GetPlayerData), "Postfix"));

            // patch all
            for (int i = 0; i < original.Count; i++)
            {
                if (patch[i].Name == "Prefix") harmony.Patch(original[i], new HarmonyMethod(patch[i]));
                else if (patch[i].Name == "Postfix") harmony.Patch(original[i], null, new HarmonyMethod(patch[i]));
            }
        }

        #endregion LocalFunctions

        #region SaveSystemJ

        public static class SaveSystemJ_GetPlayerData
        {
            public static void Postfix(ref PlayerData __result)
            {
                __result = new PlayerData();
            }
        }

        #endregion SaveSystemJ
    }
}