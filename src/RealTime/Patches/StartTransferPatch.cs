namespace RealTime.Patches
{
    using HarmonyLib;
    using RealTime.CustomAI;

    [HarmonyPatch]
    internal static class StartTransferPatch
    {
        /// <summary>Gets or sets the custom AI object for buildings.</summary>
        public static RealTimeBuildingAI RealTimeBuildingAI { get; set; }

        [HarmonyPatch(typeof(HelicopterDepotAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool HelicopterDepotAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(HospitalAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool HospitalAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(PoliceStationAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool PoliceStationAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(FireStationAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool FireStationAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DepotAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool DepotAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason reason, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(MaintenanceDepotAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool MaintenanceDepotAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(BankOfficeAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool BankOfficeAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(PostOfficeAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool PostOfficeAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(DisasterResponseBuildingAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool DisasterResponseBuildingAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(CemeteryAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool CemeteryAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(LandfillSiteAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool LandfillSiteAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(FishFarmAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool FishFarmAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(FishingHarborAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool FishingHarborAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(SnowDumpAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool SnowDumpAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(WaterFacilityAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool WaterFacilityAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ExtractingFacilityAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool ExtractingFacilityAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ProcessingFacilityAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool ProcessingFacilityAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(WarehouseAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool WarehouseAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(WarehouseStationAI), "StartTransfer")]
        [HarmonyPrefix]
        public static bool WarehouseStationAIStartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(CableCarStationAI), "ProduceGoods")]
        [HarmonyPrefix]
        public static bool CableCarStationAIProduceGoods(ushort buildingID, ref Building buildingData, ref Building.Frame frameData, int productionRate, int finalProductionRate, ref Citizen.BehaviourData behaviour, int aliveWorkerCount, int totalWorkerCount, int workPlaceCount, int aliveVisitorCount, int totalVisitorCount, int visitPlaceCount)
        {
            if (RealTimeBuildingAI != null && !RealTimeBuildingAI.IsBuildingWorking(buildingID))
            {
                return false;
            }
            return true;
        }

    }
}
