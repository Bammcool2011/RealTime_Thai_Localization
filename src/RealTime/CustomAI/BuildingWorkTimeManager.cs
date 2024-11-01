// BuildingWorkTimeManager.cs

namespace RealTime.CustomAI
{
    using System.Collections.Generic;
    using System.Linq;
    using ColossalFramework;
    using RealTime.Core;
    using RealTime.GameConnection;

    internal static class BuildingWorkTimeManager
    {
        public static Dictionary<ushort, WorkTime> BuildingsWorkTime;

        public static List<WorkTimePrefab> BuildingsWorkTimePrefabs;

        private static readonly string[] CarParkingBuildings = ["parking", "garage", "car park", "Parking", "Car Port", "Garage", "Car Park"];

        public struct WorkTime
        {
            public bool WorkAtNight;
            public bool WorkAtWeekands;
            public bool HasExtendedWorkShift;
            public bool HasContinuousWorkShift;
            public int WorkShifts;
            public bool IsDefault;
            public bool IsPrefab;
            public bool IsGlobal;
            public bool IsLocked;
        }

        public struct WorkTimePrefab
        {
            public string InfoName;
            public string BuildingAI;
            public bool WorkAtNight;
            public bool WorkAtWeekands;
            public bool HasExtendedWorkShift;
            public bool HasContinuousWorkShift;
            public int WorkShifts;
        }

        public static void Init()
        {
            BuildingsWorkTime = [];
            BuildingsWorkTimePrefabs = [];
        }

        public static void Deinit()
        {
            BuildingsWorkTime = [];
            BuildingsWorkTimePrefabs = [];
        }

        public static int GetIndex(string infoName, string buildingAIstr)
        {
            string defaultBuildingAIstr = "";
            if (buildingAIstr == "ExtendedBankOfficeAI")
            {
                defaultBuildingAIstr = "BankOfficeAI";
            }
            else if (buildingAIstr == "BankOfficeAI")
            {
                defaultBuildingAIstr = "ExtendedBankOfficeAI";
            }
            else if (buildingAIstr == "ExtendedPostOfficeAI")
            {
                defaultBuildingAIstr = "PostOfficeAI";
            }
            else if (buildingAIstr == "PostOfficeAI")
            {
                defaultBuildingAIstr = "ExtendedPostOfficeAI";
            }
            int index = BuildingsWorkTimePrefabs.FindIndex(item => item.InfoName == infoName &&
            defaultBuildingAIstr != "" ? (item.BuildingAI == buildingAIstr || item.BuildingAI == defaultBuildingAIstr) : item.BuildingAI == buildingAIstr);
            return index;
        }

        public static bool PrefabExist(BuildingInfo buildingInfo)
        {
            string BuildingAIstr = buildingInfo.GetAI().GetType().Name;
            int index = GetIndex(buildingInfo.name, BuildingAIstr);
            return index != -1;
        }

        public static WorkTimePrefab GetPrefab(BuildingInfo buildingInfo)
        {
            string BuildingAIstr = buildingInfo.GetAI().GetType().Name;
            int index = GetIndex(buildingInfo.name, BuildingAIstr);
            return index != -1 ? BuildingsWorkTimePrefabs[index] : default;
        }

        public static void SetPrefab(WorkTimePrefab workTimePrefab)
        {
            int index = GetIndex(workTimePrefab.InfoName, workTimePrefab.BuildingAI);
            if (index != -1)
            {
                BuildingsWorkTimePrefabs[index] = workTimePrefab;
            }            
        }

        public static void CreatePrefab(WorkTimePrefab workTimePrefab)
        {
            int index = GetIndex(workTimePrefab.InfoName, workTimePrefab.BuildingAI);
            if (index == -1)
            {
                BuildingsWorkTimePrefabs.Add(workTimePrefab);
            }
        }

        public static void RemovePrefab(WorkTimePrefab workTimePrefab)
        {
            int index = BuildingsWorkTimePrefabs.FindIndex(item => item.InfoName == workTimePrefab.InfoName && item.BuildingAI == workTimePrefab.BuildingAI);
            if (index != -1)
            {
                BuildingsWorkTimePrefabs.RemoveAt(index);
            }
        }

        public static WorkTime CreateBuildingWorkTime(ushort buildingID, BuildingInfo buildingInfo)
        {
            var workTime = CreateDefaultBuildingWorkTime(buildingID, buildingInfo);

            BuildingsWorkTime.Add(buildingID, workTime);

            return workTime;
        }

        public static bool ShouldHaveBuildingWorkTime(ushort buildingID)
        {
            var building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];

            if(building.Info.GetAI() is OutsideConnectionAI)
            {
                return false;
            }

            var service = building.Info.m_class.m_service;
            var level = building.Info.m_class.m_level;

            bool IsCarPark = CarParkingBuildings.Any(s => building.Info.name.Contains(s));

            if (IsCarPark)
            {
                return false;
            }

            switch (service)
            {
                case ItemClass.Service.Residential:
                case ItemClass.Service.HealthCare when level >= ItemClass.Level.Level4 && RealTimeBuildingAI.IsCimCareBuilding(buildingID):
                case ItemClass.Service.PlayerEducation when RealTimeBuildingAI.IsAreaResidentalBuilding(buildingID):
                case ItemClass.Service.PlayerIndustry when RealTimeBuildingAI.IsAreaResidentalBuilding(buildingID):
                    return false;
            }

            return true;
        }

        public static WorkTime CreateDefaultBuildingWorkTime(ushort buildingID, BuildingInfo buildingInfo)
        {
            var service = buildingInfo.m_class.m_service;
            var sub_service = buildingInfo.m_class.m_subService;
            var level = buildingInfo.m_class.m_level;
            var ai = buildingInfo.m_buildingAI;

            bool ExtendedWorkShift = HasExtendedFirstWorkShift(service, sub_service, level);
            bool ContinuousWorkShift = HasContinuousWorkShift(service, sub_service, level, ExtendedWorkShift);

            bool OpenAtNight = IsBuildingActiveAtNight(service, sub_service, level);
            bool OpenOnWeekends = IsBuildingActiveOnWeekend(service, sub_service, level);

            if (BuildingManagerConnection.IsHotel(buildingID) || RealTimeBuildingAI.IsAreaMainBuilding(buildingID) || RealTimeBuildingAI.IsWarehouseBuilding(buildingID))
            {
                OpenAtNight = true;
                OpenOnWeekends = true;
            }
            else if (service == ItemClass.Service.Beautification && sub_service == ItemClass.SubService.BeautificationParks)
            {
                var position = BuildingManager.instance.m_buildings.m_buffer[buildingID].m_position;
                byte parkId = DistrictManager.instance.GetPark(position);
                if (parkId != 0 && (DistrictManager.instance.m_parks.m_buffer[parkId].m_parkPolicies & DistrictPolicies.Park.NightTours) != 0)
                {
                    OpenAtNight = true;
                }
            }
            else if (RealTimeBuildingAI.IsEssentialIndustryBuilding(buildingID) && (sub_service == ItemClass.SubService.PlayerIndustryFarming || sub_service == ItemClass.SubService.PlayerIndustryForestry))
            {
                OpenAtNight = true;
            }
            else if (RealTimeBuildingAI.IsRecreationalCareBuilding(buildingID))
            {
                OpenAtNight = false;
                OpenOnWeekends = true;
                ExtendedWorkShift = false;
                ContinuousWorkShift = false;
            }

            if (CarParkingBuildings.Any(s => buildingInfo.name.Contains(s)))
            {
                OpenAtNight = true;
                OpenOnWeekends = true;
                ExtendedWorkShift = false;
                ContinuousWorkShift = false;
            }

            int WorkShifts = GetBuildingWorkShiftCount(service, sub_service, level, ai, OpenAtNight, ContinuousWorkShift);

            var workTime = new WorkTime()
            {
                WorkAtNight = OpenAtNight,
                WorkAtWeekands = OpenOnWeekends,
                HasExtendedWorkShift = ExtendedWorkShift,
                HasContinuousWorkShift = ContinuousWorkShift,
                WorkShifts = WorkShifts,
                IsDefault = true,
                IsPrefab = false,
                IsGlobal = false,
                IsLocked = false
            };

            return workTime;
        }

        public static bool BuildingWorkTimeExist(ushort buildingID) => BuildingsWorkTime.ContainsKey(buildingID);

        public static WorkTime GetBuildingWorkTime(ushort buildingID) => BuildingsWorkTime.TryGetValue(buildingID, out var workTime) ? workTime : default;

        public static void SetBuildingWorkTime(ushort buildingID, WorkTime workTime)
        {
            if (BuildingsWorkTime.TryGetValue(buildingID, out var _))
            {
                BuildingsWorkTime[buildingID] = workTime;
            }    
        }

        public static void RemoveBuildingWorkTime(ushort buildingID)
        {
            if(BuildingsWorkTime.TryGetValue(buildingID, out var _))
            {
                BuildingsWorkTime.Remove(buildingID);
            }
        }

        private static bool ShouldOccur(uint probability) => SimulationManager.instance.m_randomizer.Int32(100u) < probability;

        // has 3 normal shifts or 2 continous shifts
        private static bool IsBuildingActiveAtNight(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            switch (subService)
            {
                case ItemClass.SubService.CommercialTourist:
                case ItemClass.SubService.CommercialLeisure:
                case ItemClass.SubService.CommercialLow when ShouldOccur(RealTimeMod.configProvider.Configuration.OpenLowCommercialAtNightQuota):
                case ItemClass.SubService.IndustrialOil:
                case ItemClass.SubService.IndustrialOre:
                case ItemClass.SubService.PlayerIndustryOre:
                case ItemClass.SubService.PlayerIndustryOil:
                    return true;
            }

            switch (service)
            {
                case ItemClass.Service.Industrial:
                case ItemClass.Service.Tourism:
                case ItemClass.Service.Electricity:
                case ItemClass.Service.Water:
                case ItemClass.Service.HealthCare when level <= ItemClass.Level.Level3:
                case ItemClass.Service.PoliceDepartment when subService != ItemClass.SubService.PoliceDepartmentBank:
                case ItemClass.Service.FireDepartment:
                case ItemClass.Service.PublicTransport when subService != ItemClass.SubService.PublicTransportPost:
                case ItemClass.Service.Disaster:
                case ItemClass.Service.Natural:
                case ItemClass.Service.Garbage:
                case ItemClass.Service.Road:
                case ItemClass.Service.Hotel:
                case ItemClass.Service.ServicePoint:
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsBuildingActiveOnWeekend(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            switch (subService)
            {
                case ItemClass.SubService.CommercialTourist:
                case ItemClass.SubService.CommercialLeisure:
                    return true;
            }

            switch (service)
            {
                case ItemClass.Service.PlayerIndustry:
                case ItemClass.Service.Industrial:
                case ItemClass.Service.Tourism:
                case ItemClass.Service.Electricity:
                case ItemClass.Service.Water:
                case ItemClass.Service.Beautification:
                case ItemClass.Service.HealthCare:
                case ItemClass.Service.PoliceDepartment when subService != ItemClass.SubService.PoliceDepartmentBank:
                case ItemClass.Service.FireDepartment:
                case ItemClass.Service.PublicTransport when subService != ItemClass.SubService.PublicTransportPost:
                case ItemClass.Service.Disaster:
                case ItemClass.Service.Monument:
                case ItemClass.Service.Garbage:
                case ItemClass.Service.Road:
                case ItemClass.Service.Museums:
                case ItemClass.Service.VarsitySports:
                case ItemClass.Service.Fishing:
                case ItemClass.Service.ServicePoint:
                case ItemClass.Service.Hotel:
                case ItemClass.Service.Commercial when ShouldOccur(RealTimeMod.configProvider.Configuration.OpenCommercialAtWeekendsQuota):
                    return true;

                default:
                    return false;
            }
        }

        private static bool HasExtendedFirstWorkShift(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            switch (service)
            {
                case ItemClass.Service.Commercial when ShouldOccur(50):
                case ItemClass.Service.Beautification:
                case ItemClass.Service.Education:
                case ItemClass.Service.PlayerIndustry:
                case ItemClass.Service.PlayerEducation:
                case ItemClass.Service.Fishing:
                case ItemClass.Service.Industrial
                    when subService == ItemClass.SubService.IndustrialFarming || subService == ItemClass.SubService.IndustrialForestry:
                    return true;

                default:
                    return false;
            }
        }

        private static bool HasContinuousWorkShift(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, bool extendedWorkShift)
        {
            switch (subService)
            {
                case ItemClass.SubService.CommercialLow when !extendedWorkShift && ShouldOccur(50):
                    return true;
            }

            switch (service)
            {
                case ItemClass.Service.HealthCare when level <= ItemClass.Level.Level3:
                case ItemClass.Service.PoliceDepartment when subService != ItemClass.SubService.PoliceDepartmentBank:
                case ItemClass.Service.FireDepartment:
                case ItemClass.Service.Disaster:
                    return true;

                default:
                    return false;
            }
        }

        private static int GetBuildingWorkShiftCount(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, BuildingAI ai, bool activeAtNight, bool continuousWorkShift)
        {
            if(activeAtNight)
            {
                if(continuousWorkShift)
                {
                    return 2;
                }
                return 3;
            }

            switch (service)
            {
                case ItemClass.Service.Office:
                case ItemClass.Service.Education when level == ItemClass.Level.Level1 || level == ItemClass.Level.Level2:
                case ItemClass.Service.PlayerIndustry
                    when subService == ItemClass.SubService.PlayerIndustryForestry || subService == ItemClass.SubService.PlayerIndustryFarming:
                case ItemClass.Service.Industrial
                    when subService == ItemClass.SubService.IndustrialForestry || subService == ItemClass.SubService.IndustrialFarming:
                case ItemClass.Service.PoliceDepartment when subService == ItemClass.SubService.PoliceDepartmentBank:
                case ItemClass.Service.PublicTransport when subService == ItemClass.SubService.PublicTransportPost:
                    return 1;

                case ItemClass.Service.Beautification:
                case ItemClass.Service.Monument:
                case ItemClass.Service.Citizen:
                case ItemClass.Service.VarsitySports:
                case ItemClass.Service.PlayerEducation:
                case ItemClass.Service.Education when level == ItemClass.Level.Level3:
                case ItemClass.Service.Commercial when ShouldOccur(RealTimeMod.configProvider.Configuration.OpenCommercialSecondShiftQuota):
                case ItemClass.Service.HealthCare when ai is SaunaAI:
                case ItemClass.Service.Fishing when level == ItemClass.Level.Level1 && ai is MarketAI:
                    return 2;

                default:
                    return 1;
            }
        }

    }

}
