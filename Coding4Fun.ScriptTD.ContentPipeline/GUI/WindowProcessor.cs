using System;
using System.Collections.Generic;
using System.Globalization;
using Coding4Fun.ScriptTD.Common;
using Coding4Fun.ScriptTD.Common.GUI;
using Coding4Fun.ScriptTD.Common.GUI.Visuals;
using Coding4Fun.ScriptTD.ContentPipeline.Helpers;
using Coding4Fun.ScriptTD.Engine.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Design;

namespace Coding4Fun.ScriptTD.ContentPipeline.GUI
{
    [ContentProcessor(DisplayName = "Coding4Fun - Window Processor")]
    public class WindowProcessor : ContentProcessor<WindowContent, Window>
    {
        private readonly Vector2 _screenSize = new Vector2(800, 480);

        private readonly ColorConverter _colorConv = new ColorConverter();

        public override Window Process(WindowContent input, ContentProcessorContext context)
        {
            var window = new Window();

            if (input.Attributes.ContainsKey("MUSIC"))
                window.Playlist.AddRange(input.Attributes["MUSIC"].Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            ProcessAttributes(ref window, input.Attributes, _screenSize, _screenSize);

            window.Visuals.AddRange(ProcessVisuals(input.Visuals, window, context));

            ProcessControls(ref window, input.Templates, input.Controls, context);

            return window;
        }

        private List<IVisual> ProcessVisuals(List<VisualContent> visuals, IControl owner, ContentProcessorContext context)
        {
            var output = new List<IVisual>();

            if (visuals != null)
            {
                foreach (var visual in visuals)
                {
                    IVisual data;
                    // Switch statement for object creation
                    switch (visual.Type)
                    {
                        case "TEXT":
                            data = new TextVisual();
                            break;

                        case "IMAGE":
                            data = new ImageVisual();
                            break;

                        case "GRIDOVERLAY":
                            data = new GridOverlayVisual();
                            break;

                        case "GAMEPLAY":
                            data = new GameplayVisual();
                            break;

                        case "CIRCLE":
                            data = new CircleVisual();
                            break;

                        default:
                            throw new ArgumentException("Visual Type \"" + visual.Type + "\" does not exist.");
                    }
                    output.Add(data);

                    var attribs = visual.Attributes;
                    data.Location = attribs.ContainsKey("LOCATION") ? ParseVector2(attribs["LOCATION"], owner.Size) : Vector2.Zero;
                    data.Size = attribs.ContainsKey("SIZE") ? ParseVector2(attribs["SIZE"], owner.Size) : owner.Size;

                    data.HorizontalAlignment = attribs.ContainsKey("ALIGN")
                                         ? (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), attribs["ALIGN"])
                                         : HorizontalAlignment.None;

                    data.VerticalAlignment = attribs.ContainsKey("VALIGN")
                                                 ? (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), attribs["VALIGN"], true)
                                                 : VerticalAlignment.None;

                    data.Visibility = attribs.ContainsKey("VISIBILITY") ? (Visibility)Enum.Parse(typeof(Visibility), attribs["VISIBILITY"], true) : Visibility.EnabledDisabled;

                    data.Name = attribs.ContainsKey("NAME") ? attribs["NAME"] : null;

                    // Switch statement for custom attributes
                    switch (visual.Type)
                    {
                        case "TEXT":
                            ProcessTextVisual(ref data, attribs, context);
                            break;

                        case "IMAGE":
                            ProcessImageVisual(ref data, attribs, context);
                            break;

                        case "GRIDOVERLAY":
                            ProcessGridVisual(ref data, attribs, context);
                            break;

                        case "CIRCLE":
                            ProcessCircleVisual(ref data, attribs, context);
                            break;
                    }
                }
            }
            return output;
        }

        private void ProcessControls(ref Window window, Dictionary<string, TemplateContent> templates, List<ControlContent> list, ContentProcessorContext context)
        {
            foreach (var control in list)
            {
                IControl data;
                switch (control.Type)
                {
                    case "BUTTON":
                        data = new Button();
                        break;
                    case "LABEL":
                        data = new Label();
                        break;

                    case "TOWERBUTTON":
                        data = new TowerButton();
                        ((TowerButton)data).TowerId = control.Attributes["TOWERID"];
                        break;

                    case "CHECKBOX":
                        data = new CheckBox();
                        break;

                    case "SLIDESHOW":
                        data = new Slideshow();
                        break;

                    default:
                        throw new ArgumentException("GUI Element \"" + control.Type + "\" does not exist.");
                }

                data.Parent = window;

                ProcessAttributes(ref data, control.Attributes, window.Size, Vector2.Zero);

                // Note: If templates are added, process them here instead of the following (if defined & requested)
                if (!string.IsNullOrWhiteSpace(control.Template) && templates != null && templates.ContainsKey(control.Template))
                {
                    control.Visuals.AddRange(templates[control.Template].Visuals);
                }

                List<IVisual> visuals = ProcessVisuals(control.Visuals, data, context);

                if (visuals == null || visuals.Count == 0)
                    throw new NullReferenceException("No visuals have been defined for \"" + data.Name + "\"");

                data.Visuals.AddRange(visuals);

                window.Controls.Add(!string.IsNullOrWhiteSpace(data.Name) ? data.Name : window.Controls.Count.ToString(), data);
            }
        }

        private void ProcessAttributes<T>(ref T control, Dictionary<string, string> attribs, Vector2 parentSize, Vector2 defaultSize) where T : IControl
        {
            control.Location = attribs.ContainsKey("LOCATION") ? ParseVector2(attribs["LOCATION"], parentSize) : Vector2.Zero;
            control.Size = attribs.ContainsKey("SIZE") ? ParseVector2(attribs["SIZE"], parentSize) : defaultSize;
            control.RecalculateBounds();
            control.Name = attribs.ContainsKey("NAME") ? attribs["NAME"] : "";
            control.Text = attribs.ContainsKey("TEXT") ? attribs["TEXT"] : "";
            control.Value = attribs.ContainsKey("VALUE") ? float.Parse(attribs["VALUE"]) : float.NaN;
            control.Enabled = attribs.ContainsKey("ENABLED") ? bool.Parse(attribs["ENABLED"]) : true;
            control.Visible = attribs.ContainsKey("VISIBLE") ? bool.Parse(attribs["VISIBLE"]) : true;
        }

        private Vector2 ParseVector2(string str, Vector2 relativeTo)
        {
            var parts = str.Split(',');
            if (parts.Length != 2)
                throw new FormatException("Size or Location is not in the correct format. Expected: \"x,y\"");

            Vector2 result = Vector2.Zero;
            result.X = HandleFloat(parts[0], relativeTo.X / 100);
            result.Y = HandleFloat(parts[1], relativeTo.Y / 100);

            return result;
        }

        private static float HandleFloat(string str, float percentMult)
        {
            str = str.Trim();

            return str.Contains("%") ? float.Parse(str.Replace("%", ""), CultureInfo.GetCultureInfo("en-US").NumberFormat) * percentMult : float.Parse(str.Replace("%", ""), CultureInfo.GetCultureInfo("en-US").NumberFormat);
        }


        private void ProcessTextVisual(ref IVisual data, Dictionary<string, string> attribs, ContentProcessorContext context)
        {
            if (!(data is TextVisual))
                throw new ArgumentException("Incorrect type created, expected TextVisual.");

            var text = (TextVisual)data;

            text.ManualText = attribs["TEXT"];

            if (text.ManualText.Equals("{TEXT}"))
                text.Source = TextSource.ControlText;
            else if (text.ManualText.Equals("{VALUE}"))
                text.Source = TextSource.ControlValue;
            else
                text.Source = TextSource.Manual;

            text.FontPath = LoadingHelper.BuildExternalFont(context, attribs["FONT"]);

            text.TextColor = attribs.ContainsKey("COLOR") ? (Color)_colorConv.ConvertFromInvariantString(attribs["COLOR"]) : text.TextColor;

            text.AllowParentResize = attribs.ContainsKey("ALLOWPARENTRESIZE")
                                         ? bool.Parse(attribs["ALLOWPARENTRESIZE"])
                                         : text.AllowParentResize;

            text.DisabledColor = attribs.ContainsKey("DISABLEDCOLOR")
                                     ? (Color)_colorConv.ConvertFromInvariantString(attribs["DISABLEDCOLOR"])
                                     : text.DisabledColor;

            text.StrokeColor = attribs.ContainsKey("STROKECOLOR")
                                    ? (Color)_colorConv.ConvertFromInvariantString(attribs["STROKECOLOR"])
                                    : text.StrokeColor;

            text.StrokeOffset = attribs.ContainsKey("STROKEOFFSET")
                                    ? new Vector2(float.Parse(attribs["STROKEOFFSET"]))
                                    : text.StrokeOffset;
        }

        private void ProcessImageVisual(ref IVisual data, Dictionary<string, string> attribs, ContentProcessorContext context)
        {
            if (!(data is ImageVisual))
                throw new ArgumentException("Incorrect type created, expected ImageVisual.");

            ImageVisual image = (ImageVisual)data;

            image.DisabledTint = attribs.ContainsKey("DISABLEDTINT")
                                     ? (Color)_colorConv.ConvertFromInvariantString(attribs["DISABLEDTINT"])
                                     : image.DisabledTint;

            image.Tint = attribs.ContainsKey("TINT") ? (Color)_colorConv.ConvertFromInvariantString(attribs["TINT"]) : image.Tint;

            image.ImagePath = attribs.ContainsKey("SRC") ? LoadingHelper.BuildTexture(context, attribs["SRC"]) : "";

            image.UseParentSize = attribs.ContainsKey("USEPARENTSIZE")
                                      ? bool.Parse(attribs["USEPARENTSIZE"])
                                      : image.UseParentSize;
        }

        private void ProcessGridVisual(ref IVisual data, Dictionary<string, string> attribs, ContentProcessorContext context)
        {
            if (!(data is GridOverlayVisual))
                throw new ArgumentException("Incorrect type created, expected GridOverlayVisual.");

            GridOverlayVisual grid = (GridOverlayVisual)data;

            grid.GridSpacing = attribs.ContainsKey("SPACING") ? float.Parse(attribs["SPACING"]) : 35f;

            grid.GridColor = attribs.ContainsKey("COLOR") ? (Color)_colorConv.ConvertFromInvariantString(attribs["COLOR"]) : Color.Black;
        }

        private void ProcessCircleVisual(ref IVisual data, Dictionary<string, string> attribs, ContentProcessorContext context)
        {
            if (!(data is CircleVisual))
                throw new ArgumentException("Incorrect type created, expected CircleVisual.");

            CircleVisual circle = (CircleVisual)data;

            circle.Density = attribs.ContainsKey("DENSITY") ? int.Parse(attribs["DENSITY"]) : circle.Density;

            circle.Color = attribs.ContainsKey("COLOR")
                               ? (Color)_colorConv.ConvertFromInvariantString(attribs["COLOR"])
                               : circle.Color;
        }
    }
}