// VehicleAIPatch.cs

namespace RealTime.Patches
{
    using ColossalFramework;
    using HarmonyLib;
    using RealTime.CustomAI;
    using UnityEngine;

    [HarmonyPatch]
    internal static class VehicleAIPatch
    {
        // <summary>Gets or sets the custom AI object for buildings.</summary>
        public static RealTimeBuildingAI RealTimeBuildingAI { get; set; }

        [HarmonyPatch]
        private sealed class FireTruckAIPatch
        {
            [HarmonyPatch(typeof(FireTruckAI), "ExtinguishFire")]
            [HarmonyPrefix]
            private static bool ExtinguishFirePrefix(FireTruckAI __instance, ushort vehicleID, ref Vehicle data, ushort buildingID, ref Building buildingData, ref bool __result)
            {
                if (RealTimeBuildingAI == null)
                {
                    return true;
                }
                byte fireIntensity = buildingData.m_fireIntensity;
                if(fireIntensity > 0)
                {
                    RealTimeBuildingAI.CreateBuildingFire(data.m_targetBuilding);
                    return RealTimeBuildingAI.ShouldExtinguishFire(buildingID);
                }
                if (fireIntensity == 0)
                {
                    RealTimeBuildingAI.RemoveBuildingFire(data.m_targetBuilding);
                }
                return true;               
            }

            [HarmonyPatch(typeof(FireTruckAI), "SetTarget")]
            [HarmonyPrefix]
            private static void SetTargetPrefix(ushort vehicleID, ref Vehicle data, ushort targetBuilding)
            {
                if (targetBuilding == 0 && RealTimeBuildingAI != null)
                {
                    RealTimeBuildingAI.RemoveBuildingFire(data.m_targetBuilding);
                }
                else
                {
                    ref var building = ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[targetBuilding];
                    if (building.m_fireIntensity == 0 && RealTimeBuildingAI != null)
                    {
                        RealTimeBuildingAI.RemoveBuildingFire(data.m_targetBuilding);
                    }
                }
            }
        }

        [HarmonyPatch]
        private sealed class FireCopterAIPatch
        {
            [HarmonyPatch(typeof(FireCopterAI), "ExtinguishFire",
                [typeof(ushort), typeof(Vehicle), typeof(ushort), typeof(Building)],
                [ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Ref])]
            [HarmonyPrefix]
            private static bool ExtinguishFirePrefix(FireCopterAI __instance, ushort vehicleID, ref Vehicle data, ushort buildingID, ref Building buildingData, ref bool __result)
            {
                byte fireIntensity = buildingData.m_fireIntensity;
                if (fireIntensity > 0 && RealTimeBuildingAI != null)
                {
                    RealTimeBuildingAI.CreateBuildingFire(data.m_targetBuilding);
                    if (RealTimeBuildingAI.ShouldExtinguishFire(buildingID))
                    {
                        return true;
                    }
                    else
                    {
                        int num2 = Mathf.Min(__instance.m_fireFightingRate, data.m_transferSize);
                        data.m_transferSize = (ushort)(data.m_transferSize - num2);
                    }
                    return false;
                }
                if (fireIntensity == 0 && RealTimeBuildingAI != null)
                {
                    RealTimeBuildingAI.RemoveBuildingFire(data.m_targetBuilding);
                }
                return true;
            }

            [HarmonyPatch(typeof(FireCopterAI), "SetTarget")]
            [HarmonyPrefix]
            private static void SetTargetPrefix(ushort vehicleID, ref Vehicle data, ushort targetBuilding)
            {
                if (targetBuilding == 0 && RealTimeBuildingAI != null)
                {
                    RealTimeBuildingAI.RemoveBuildingFire(data.m_targetBuilding);
                }
                else
                {
                    ref var building = ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[targetBuilding];
                    if (building.m_fireIntensity == 0 && RealTimeBuildingAI != null)
                    {
                        RealTimeBuildingAI.RemoveBuildingFire(data.m_targetBuilding);
                    }
                }
            }
        }

    }
}
