using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.ContentPipeline.Helpers
{
    public static class XMLElementHelper
    {
        public static string Attribute(this XElement element, XName name, string def)
        {
            try
            {
                return element.Attribute(name).Value;
            }
            catch (NullReferenceException)
            {
                return def;
            }
        }

        public static float Attribute(this XElement element, XName name, float def)
        {
            try
            {
                return float.Parse(element.Attribute(name).Value, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
            catch (Exception)
            {
                return def;
            }
        }

        public static int Attribute(this XElement element, XName name, int def)
        {
            try
            {
                return int.Parse(element.Attribute(name).Value, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
            catch (Exception)
            {
                return def;
            }
        }

        public static XElement Element(this XElement element, XName name)
        {
            try
            {
                return element.Element(name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool Attribute(this XElement element, XName name, bool def)
        {
            try
            {
                return bool.Parse(element.Attribute(name).Value);
            }
            catch (Exception)
            {
                return def;
            }
        }

        public static Vector2 Attribute(this XElement element, XName name, Vector2 def)
        {
            try
            {
                var s = element.Attribute(name).Value;
                var parts = s.Split(',');
                if (parts.Length >= 2)
                {
                    float x = float.Parse(parts[0], CultureInfo.GetCultureInfo("en-US").NumberFormat);
                    float y = float.Parse(parts[1], CultureInfo.GetCultureInfo("en-US").NumberFormat);
                    return new Vector2(x, y);
                }

                return def;
            }
            catch (Exception)
            {
                return def;
            }
        }
    }
}
