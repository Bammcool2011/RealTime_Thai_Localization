namespace RealTime.Patches.BuildingAIPatches
{
    using ColossalFramework;
    using HarmonyLib;
    using UnityEngine;

    internal class FishingAIPatch
    {
        [HarmonyPatch(typeof(FishingHarborAI), "TrySpawnBoat")]
        [HarmonyPrefix]
        public static bool FishingHarborAITrySpawnBoat(ref Building buildingData) => (buildingData.m_flags & Building.Flags.Active) != 0;

        [HarmonyPatch(typeof(FishingHarborAI), "GetColor")]
        [HarmonyPrefix]
        public static bool FishingHarborAIGetColor(FishingHarborAI __instance, ushort buildingID, ref Building data, InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode, ref Color __result)
        {
            if (infoMode == InfoManager.InfoMode.Fishing)
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

        [HarmonyPatch(typeof(FishFarmAI), "GetColor")]
        [HarmonyPrefix]
        public static bool FishFarmAIGetColor(FishFarmAI __instance, ushort buildingID, ref Building data, InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode, ref Color __result)
        {
            if (infoMode == InfoManager.InfoMode.Fishing)
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
