using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Common.GUI
{
    public class RenderEngine
    {
        public readonly List<IVisual> Visuals = new List<IVisual>();

        public void LoadGraphics(ContentManager content, GraphicsDevice device)
        {
            foreach (var guiVisual in Visuals)
            {
                guiVisual.LoadGraphics(content, device);
            }
        }

        public void Update(float elapsedSeconds)
        {
        	foreach (var visual in Visuals)
        	{
        		visual.Update(elapsedSeconds);
        	}
        }

    	public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            foreach (var visual in Visuals)
            {
            	if (!visual.Owner.Visible)
					continue;

            	bool draw = true;
            	bool isChecked = visual.Owner is CheckBox && ((CheckBox)visual.Owner).Checked;

            	switch (visual.Visibility)
            	{
            		case Visibility.Enabled:
            			draw = visual.Owner.Enabled;
            			break;
            		case Visibility.Disabled:
            			draw = !visual.Owner.Enabled;
            			break;

            		case Visibility.Checked:
            			draw = isChecked;
            			break;

            		case Visibility.Unchecked:
            			draw = !isChecked;
            			break;
            	}

            	if (draw)
            		visual.Draw(device, spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
