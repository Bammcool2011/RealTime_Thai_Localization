// LibraryAIPatch.cs

namespace RealTime.Patches.BuildingAIPatches
{
    using ColossalFramework;
    using HarmonyLib;
    using RealTime.CustomAI;
    using UnityEngine;

    [HarmonyPatch]
    internal class LibraryAIPatch
    {
        /// <summary>Gets or sets the custom AI object for buildings.</summary>
        public static RealTimeBuildingAI RealTimeBuildingAI { get; set; }

        [HarmonyPatch(typeof(LibraryAI), "GetCurrentRange",
                [typeof(ushort), typeof(Building), typeof(float)],
                [ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal])]
        [HarmonyPrefix]
        public static bool GetCurrentRange(LibraryAI __instance, ushort buildingID, ref Building data, float radius, ref float __result)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                int num = data.m_productionRate;
                if ((data.m_flags & (Building.Flags.Evacuating)) != 0)
                {
                    num = 0;
                }
                else if ((data.m_flags & Building.Flags.RateReduced) != 0)
                {
                    num = Mathf.Min(num, 50);
                }
                int budget = Singleton<EconomyManager>.instance.GetBudget(__instance.m_info.m_class);
                num = PlayerBuildingAI.GetProductionRate(num, budget);
                __result = (float)num * radius * 0.01f;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(LibraryAI), "GetColor")]
        [HarmonyPrefix]
        public static bool GetColor(LibraryAI __instance, ushort buildingID, ref Building data, InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode, ref Color __result)
        {
            if (infoMode == InfoManager.InfoMode.Education && subInfoMode == InfoManager.SubInfoMode.LibraryEducation)
            {
                if (data.m_productionRate > 0)
                {
                    __result = Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_activeColor;
                }
                else
                {
                    __result = Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_inactiveColor;
                }
                return false;
            }
            if (infoMode == InfoManager.InfoMode.Entertainment && subInfoMode == InfoManager.SubInfoMode.PipeWater)
            {
                if (data.m_productionRate > 0)
                {
                    __result = Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_activeColor;
                }
                else
                {
                    __result = Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_inactiveColor;
                }
                return false;
            }
            return true;
        }

    }
}
