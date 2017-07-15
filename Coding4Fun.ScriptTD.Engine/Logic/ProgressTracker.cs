using System.Collections.Generic;
using System.IO;

namespace Coding4Fun.ScriptTD.Engine.Logic
{
    public static class ProgressTracker
    {
        public const string ProgressFile = "progress.dat";

        private static readonly Dictionary<string, bool> Tracking = new Dictionary<string, bool>();

        public static void CompleteLevel(string id)
        {
            if (!Tracking.ContainsKey(id))
                Tracking.Add(id, true);
            else
                Tracking[id] = true;
        }

        public static bool GetStatus(string id)
        {
            return Tracking.ContainsKey(id) ? Tracking[id] : false;
        }

        public static void ClearTracking()
        {
            Tracking.Clear();
        }

        public static void Save(BinaryWriter bw)
        {
            bw.Write(Tracking.Count);

            foreach (var status in Tracking)
            {
                bw.Write(status.Key);
                bw.Write(status.Value);
            }
            
			bw.Flush();
        }

        public static void Load(BinaryReader br)
        {
            ClearTracking();
            int numItems = br.ReadInt32();

            for (int i = 0; i < numItems; i++)
            {
                string id = br.ReadString();
                bool status = br.ReadBoolean();

                Tracking.Add(id, status);
            }
        }
    }
}
