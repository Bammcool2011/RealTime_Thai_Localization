namespace RealTime.Patches.BuildingAIPatches
{
    using HarmonyLib;

    internal class ShelterAIPatch
    {
        [HarmonyPatch(typeof(ShelterAI), "CreateBuilding")]
        [HarmonyPrefix]
        public static void CreateBuilding(ShelterAI __instance, ushort buildingID, ref Building data)
        {
            __instance.m_goodsStockpileAmount = ushort.MaxValue;
            data.m_customBuffer1 = ushort.MaxValue;
        }

        [HarmonyPatch(typeof(ShelterAI), "SimulationStep")]
        [HarmonyPrefix]
        public static void SimulationStep(ShelterAI __instance, ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            if (buildingData.m_productionRate == 0)
            {
                __instance.m_goodsConsumptionRate = 1;
            }
        }

        [HarmonyPatch(typeof(ShelterAI), "ProduceGoods")]
        [HarmonyPrefix]
        public static void ProduceGoods(ShelterAI __instance, ushort buildingID, ref Building buildingData, ref Building.Frame frameData, int productionRate, int finalProductionRate, ref Citizen.BehaviourData behaviour, int aliveWorkerCount, int totalWorkerCount, int workPlaceCount, int aliveVisitorCount, int totalVisitorCount, int visitPlaceCount) => __instance.m_goodsConsumptionRate = 1;

    }
}
