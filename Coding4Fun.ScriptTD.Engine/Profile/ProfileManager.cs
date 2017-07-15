using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace Coding4Fun.ScriptTD.Engine.Profile
{
    public static class ProfileManager
    {
        public const string ProfilePath = @"profiles\";
        public static readonly List<Profile> Profiles = new List<Profile>();
        public static Profile CurrentProfile;

        public static void LoadProfiles()
        {
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.DirectoryExists(ProfilePath) && isf.FileExists(ProfilePath + "manifest.dat"))
                {
					using (var manifest = isf.OpenFile(ProfilePath + "manifest.dat", FileMode.Open, FileAccess.Read))
					{
						using (var br = new BinaryReader(manifest))
						{
							Profiles.Clear();
							int numProfiles = br.ReadInt32();

							for (int i = 0; i < numProfiles; i++)
							{
								var prof = new Profile
								           	{
								           		DisplayName = br.ReadString(),
								           		ProfileFolder = br.ReadString()
								           	};

								Profiles.Add(prof);
							}
						}
					}
                }
            }
        }

        public static void SaveProfiles()
        {
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.DirectoryExists(ProfilePath))
                    isf.CreateDirectory(ProfilePath);

				using (var manifest = isf.OpenFile(ProfilePath + "manifest.dat", FileMode.Create, FileAccess.Write))
				{
					using (var bw = new BinaryWriter(manifest))
					{
						bw.Write(Profiles.Count);

						foreach (var profile in Profiles)
						{
							bw.Write(profile.DisplayName);
							bw.Write(profile.ProfileFolder);
						}

						bw.Flush();
					}
				}
            }
        }
    }
}
