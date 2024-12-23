namespace RealTime.CustomAI
{
    using System;
    using System.Collections.Generic;

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
            var rnd = new Random();
            int index = rnd.Next(HotelsList.Count);
            return HotelsList[index];
        }
    }
}
