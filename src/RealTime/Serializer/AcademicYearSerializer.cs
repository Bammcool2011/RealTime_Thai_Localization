// AcademicYearSerializer.cs

namespace RealTime.Serializer
{
    using System;
    using RealTime.Managers;
    using UnityEngine;

    public class AcademicYearSerializer
    {
        // Some magic values to check we are line up correctly on the tuple boundaries
        private const uint uiTUPLE_START = 0xFEFEFEFE;
        private const uint uiTUPLE_END = 0xFAFAFAFA;

        private const ushort iACADEMIC_YEAR_DATA_VERSION = 1;

        public static void SaveData(FastList<byte> Data)
        {
            // Write out metadata
            StorageData.WriteUInt16(iACADEMIC_YEAR_DATA_VERSION, Data);

            StorageData.WriteUInt32(uiTUPLE_START, Data);

            StorageData.WriteBool(AcademicYearManager.AcademicYearData.DidLastYearEnd, Data);
            StorageData.WriteBool(AcademicYearManager.AcademicYearData.DidGraduationStart, Data);
            StorageData.WriteFloat(AcademicYearManager.AcademicYearData.GraduationStartTime, Data);
            StorageData.WriteUInt32(AcademicYearManager.AcademicYearData.ActualAcademicYearEndFrame, Data);

            StorageData.WriteUInt32(uiTUPLE_END, Data);

        }

        public static void LoadData(int iGlobalVersion, byte[] Data, ref int iIndex)
        {
            if (Data != null && Data.Length > iIndex)
            {
                int iacademicYearVersion = StorageData.ReadUInt16(Data, ref iIndex);
                Debug.Log("Global: " + iGlobalVersion + " BufferVersion: " + iacademicYearVersion + " DataLength: " + Data.Length + " Index: " + iIndex);

                CheckStartTuple($"Buffer", iacademicYearVersion, Data, ref iIndex);

                AcademicYearManager.AcademicYearData.DidLastYearEnd = StorageData.ReadBool(Data, ref iIndex);
                AcademicYearManager.AcademicYearData.DidGraduationStart = StorageData.ReadBool(Data, ref iIndex);
                AcademicYearManager.AcademicYearData.GraduationStartTime = StorageData.ReadFloat(Data, ref iIndex);
                AcademicYearManager.AcademicYearData.ActualAcademicYearEndFrame = StorageData.ReadUInt32(Data, ref iIndex);

                CheckEndTuple($"Buffer", iacademicYearVersion, Data, ref iIndex);
            }
        }

        private static void CheckStartTuple(string sTupleLocation, int iDataVersion, byte[] Data, ref int iIndex)
        {
            if (iDataVersion >= 1)
            {
                uint iTupleStart = StorageData.ReadUInt32(Data, ref iIndex);
                if (iTupleStart != uiTUPLE_START)
                {
                    throw new Exception($"AcademicYearData Buffer start tuple not found at: {sTupleLocation}");
                }
            }
        }

        private static void CheckEndTuple(string sTupleLocation, int iDataVersion, byte[] Data, ref int iIndex)
        {
            if (iDataVersion >= 1)
            {
                uint iTupleEnd = StorageData.ReadUInt32(Data, ref iIndex);
                if (iTupleEnd != uiTUPLE_END)
                {
                    throw new Exception($"AcademicYearData Buffer end tuple not found at: {sTupleLocation}");
                }
            }
        }

    }
}
