namespace RealTime.Managers
{
    using System.Collections.Generic;
    using ColossalFramework;

    internal static class HotelManager
    {
        private static List<ushort> HotelsList;

        public static void Init() => HotelsList = [];

        public static bool HotelExist(ushort buidlingId)
        {
            int index = HotelsList.FindIndex(item => item == buidlingId);
            return index != -1;
        }

        public static void AddHotel(ushort buidlingId) => HotelsList.Add(buidlingId);

        public static void RemoveHotel(ushort buidlingId) => HotelsList.Remove(buidlingId);

        public static ushort FindRandomHotel()
        {
            if (HotelsList.Count == 0)
            {
                return 0;
            }

            for (int i = HotelsList.Count - 1; i >= 0; i--)
            {
                var hotelBuilding = Singleton<BuildingManager>.instance.m_buildings.m_buffer[HotelsList[i]];
                if (hotelBuilding.m_roomUsed >= hotelBuilding.m_roomMax)
                {
                    HotelsList.RemoveAt(i);
                }
            }

            if (HotelsList.Count == 0)
            {
                return 0;
            }

            int index = Singleton<SimulationManager>.instance.m_randomizer.Int32(0, HotelsList.Count - 1);
            return HotelsList[index];
        }
    }
}
