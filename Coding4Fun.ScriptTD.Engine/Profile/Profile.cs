using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace Coding4Fun.ScriptTD.Engine.Profile
{
    public class Profile : IDisposable
    {
        public string DisplayName;
        public string ProfileFolder;

        private IsolatedStorageFile _isf;
        private IsolatedStorageFileStream _str;

        public Profile(string displayName)
        {
            DisplayName = displayName;
            ProfileFolder = displayName.Replace(" ", "").ToUpper();
            ProfileManager.Profiles.Add(this);
        }

        internal Profile() { }

        public void WriteBytes(string filename, byte[] data)
        {
            var path = Path.Combine(ProfileManager.ProfilePath, ProfileFolder);
            CreateFile();

            if (!_isf.DirectoryExists(path))
                _isf.CreateDirectory(path);

            using (var file = _isf.OpenFile(Path.Combine(path, filename), FileMode.Create, FileAccess.Write))
            {
                file.Write(data, 0, data.Length);
                file.Flush();
            }
        }

        public void WriteBytes(string filename, MemoryStream memStream)
        {
            WriteBytes(filename, memStream.ToArray());
        }

        public BinaryReader Read(string filename)
        {
            var path = Path.Combine(ProfileManager.ProfilePath, ProfileFolder);
            CreateFile();
            CloseStream();
            _str = _isf.OpenFile(Path.Combine(path, filename), FileMode.Open, FileAccess.Read);
            return new BinaryReader(_str);
        }

        public BinaryWriter Write(string filename)
        {
            var path = Path.Combine(ProfileManager.ProfilePath, ProfileFolder);
            CreateFile();
            CloseStream();
            _str = _isf.OpenFile(Path.Combine(path, filename), FileMode.Create, FileAccess.Write);
            return new BinaryWriter(_str);
        }

        public bool FileExists(string filename)
        {
            var path = Path.Combine(ProfileManager.ProfilePath, ProfileFolder);
            CreateFile();
            return _isf.FileExists(Path.Combine(path, filename));
        }

        public void DeleteFile(string filename)
        {
            var path = Path.Combine(ProfileManager.ProfilePath, ProfileFolder);
            CreateFile();
            _isf.DeleteFile(Path.Combine(path, filename));
        }

        private void CreateFile()
        {
            if (_isf == null)
            {
                _isf = IsolatedStorageFile.GetUserStoreForApplication();
                var path = Path.Combine(ProfileManager.ProfilePath, ProfileFolder);
                if (!_isf.DirectoryExists(path))
                    _isf.CreateDirectory(path);
            }
        }

        private void CloseFile()
        {
            if (_isf != null)
            {
                _isf.Dispose();
                _isf = null;
            }
        }

        public void CloseStream()
        {
            if (_str != null)
            {
                _str.Dispose();
                _str = null;
            }
        }

        public void Dispose()
        {
            CloseStream();
            CloseFile();
        }
    }
}
