using System.Collections.Generic;

namespace Coding4Fun.ScriptTD.Common.Abstracts
{
    public abstract class MapListing
    {
        public string Id;
        public string FriendlyName;
        public string DataFilePath;

        public List<string> Prerequisites;
    }
}
