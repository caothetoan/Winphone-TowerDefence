using Microsoft.Xna.Framework.Content;

namespace Coding4Fun.ScriptTD.Engine.Data.Readers
{
    public class MapListDataReader : ContentTypeReader<MapListData>
    {
        protected override MapListData Read(ContentReader input, MapListData existingInstance)
        {
            var data = new MapListData();

            int numItems = input.ReadInt32();
            
			for (int i = 0; i < numItems; i++)
            {
                data.Maps.Add(input.ReadString(), input.ReadObject<MapListingData>());
            }
            
			return data;
        }
    }
}
