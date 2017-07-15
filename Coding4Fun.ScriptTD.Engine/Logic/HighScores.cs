using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace Coding4Fun.ScriptTD.Engine.Logic
{
    public static class HighScores
    {
        public class MapScores
        {
            public const int MaxScores = 5;
            public readonly float[] Scores = new float[MaxScores];

            public bool AddScore(float score)
            {
                for (int i = 0; i < MaxScores; i++)
                {
                    if (Scores[i] < score)
                    {
                        for (int j = MaxScores - 1; j > i; j--)
                        {
                            Scores[j] = Scores[j - 1];
                        }

                        Scores[i] = score;
                        return true;
                    }
                }

                return false;
            }
        }

        private const string DataFile = "highscores.dat";

        private static readonly Dictionary<string, MapScores> Maps = new Dictionary<string, MapScores>();

        public static bool SubmitScore(string mapId, float score)
        {
            MapScores map;

            if (!Maps.TryGetValue(mapId, out map))
            {
                map = new MapScores();
                Maps.Add(mapId, map);
            }

            return map.AddScore(score);
        }

        public static bool GetScores(string mapId, out float[] scores)
        {
            if (Maps.ContainsKey(mapId))
            {
                scores = Maps[mapId].Scores;
                return true;
            }
            
			scores = new float[MapScores.MaxScores];

            return false;
        }

        public static void Save()
        {
			using (var file = IsolatedStorageFile.GetUserStoreForApplication())
			{
				using (var stream = file.OpenFile(DataFile, FileMode.Create, FileAccess.Write))
				{
					using (var writer = new BinaryWriter(stream))
					{
						writer.Write(Maps.Count);

						foreach (var map in Maps)
						{
							writer.Write(map.Key);

							for (int i = 0; i < MapScores.MaxScores; i++)
							{
								writer.Write(map.Value.Scores[i]);
							}
						}
					}
				}
			}
        }

        public static void Load()
        {
            using (var file = IsolatedStorageFile.GetUserStoreForApplication())
            {
            	if (!file.FileExists(DataFile))
					return;

            	using (var stream = file.OpenFile(DataFile, FileMode.Open, FileAccess.Read))
            	{
            		using (var reader = new BinaryReader(stream))
            		{
            			Maps.Clear();
            			int numMaps = reader.ReadInt32();

            			for (int i = 0; i < numMaps; i++)
            			{
            				string id = reader.ReadString();
            				var ms = new MapScores();

            				for (int j = 0; j < MapScores.MaxScores; j++)
            				{
            					ms.Scores[j] = reader.ReadSingle();
            				}

            				Maps.Add(id, ms);
            			}
            		}
            	}
            }
        }
    }
}
