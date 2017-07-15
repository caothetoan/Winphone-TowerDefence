using Coding4Fun.ScriptTD.Common.Abstracts;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Coding4Fun.ScriptTD.ContentPipeline.Maps
{
	[ContentSerializerRuntimeType("Coding4Fun.ScriptTD.Engine.Data.MapListingData, Coding4Fun.ScriptTD.Engine")] 
    public class MapListingContent : MapListing
    {
        public Texture2DContent Thumbnail;
    }
}
