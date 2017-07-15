using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Common.GUI
{
    public static class LayoutHelper
    {
        public static void DoLayout(HorizontalAlignment align, VerticalAlignment valign, ref Vector2 ownerPos,
            ref Vector2 ownerSize, ref Vector2 pos, ref Vector2 size, out Vector2 finalPos)
        {
            finalPos = Vector2.Zero;
            switch (align)
            {
                case HorizontalAlignment.None:
                    finalPos.X = ownerPos.X + pos.X;
                    break;

                case HorizontalAlignment.Left:
                    finalPos.X = ownerPos.X + pos.X;
                    break;

                case HorizontalAlignment.Center:
                    finalPos.X = CenterAlign(ownerPos.X, ownerSize.X, pos.X, size.X);
                    break;

                case HorizontalAlignment.Right:
                    finalPos.X = RightBottomAlign(ownerPos.X, ownerSize.X, pos.X, size.X);
                    break;
            }
            switch (valign)
            {
                case VerticalAlignment.None:
                    finalPos.Y = ownerPos.Y + pos.Y;
                    break;

                case VerticalAlignment.Top:
                    finalPos.Y = ownerPos.Y + pos.Y;
                    break;

                case VerticalAlignment.Middle:
                    finalPos.Y = CenterAlign(ownerPos.Y, ownerSize.Y, pos.Y, size.Y);
                    break;

                case VerticalAlignment.Bottom:
                    finalPos.Y = RightBottomAlign(ownerPos.Y, ownerSize.Y, pos.Y, size.Y);
                    break;
            }
        }

        private static float CenterAlign(float ownerLoc, float ownerSize, float thisLoc, float thisSize)
        {
            float ownerCenter = ownerLoc + (ownerSize / 2);
            float textCenter = thisSize / 2;
            return (ownerCenter - textCenter) + thisLoc;
        }

        private static float RightBottomAlign(float ownerLoc, float ownerSize, float thisLoc, float thisSize)
        {
            float ownerRight = ownerLoc + ownerSize;
            return (ownerRight - thisSize) + thisLoc;
        }
    }
}
