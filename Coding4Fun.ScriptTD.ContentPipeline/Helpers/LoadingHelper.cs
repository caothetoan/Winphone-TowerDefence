using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Coding4Fun.ScriptTD.ContentPipeline.Helpers
{
    public static class LoadingHelper
    {
        public static string BuildExternalFont(ContentProcessorContext context, string filename)
        {
            if (!Path.HasExtension(filename))
                filename = Path.Combine(filename, ".spritefont");

            var path = Path.GetDirectoryName(filename);

            var font = context.BuildAsset<FontDescription, SpriteFontContent>(new ExternalReference<FontDescription>(filename), "FontDescriptionProcessor");

            return Path.Combine(path, Path.GetFileNameWithoutExtension(font.Filename));
        }

        public static string BuildTexture(ContentProcessorContext context, string filename)
        {
            var path = Path.GetDirectoryName(filename);

            var tex = context.BuildAsset<Texture2DContent, Texture2DContent>(
                new ExternalReference<Texture2DContent>(filename), "");

            return Path.Combine(path, Path.GetFileNameWithoutExtension(tex.Filename));
        }

        public static Texture2DContent LoadTexture(ContentProcessorContext context, string filename)
        {
            return context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(filename), "");
        }
    }
}
