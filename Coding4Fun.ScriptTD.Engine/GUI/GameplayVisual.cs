using Coding4Fun.ScriptTD.Common;
using Coding4Fun.ScriptTD.Common.GUI;
using Coding4Fun.ScriptTD.Engine.Logic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine.GUI
{
    public class GameplayVisual : IVisual
    {
        [ContentSerializerIgnore]
        public GameSession Session;

        public IControl Owner { get; set; }

        public string Name { get; set; }

        public Vector2 Location { get; set; }

        public Vector2 Size { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public VerticalAlignment VerticalAlignment { get; set; }

        public Visibility Visibility { get; set; }

        private Rectangle _cellRect;

        private readonly ProgressBar _towerProgBar = new ProgressBar();
        private readonly ProgressBar _enemyProgBar = new ProgressBar();

        public void LoadGraphics(ContentManager content, GraphicsDevice graphics)
        {
            _towerProgBar.BaseColor = Color.Gray;
            _towerProgBar.TopColor = Color.Green;
            _towerProgBar.LoadGraphics(graphics);

            _enemyProgBar.BaseColor = Color.Red;
            _enemyProgBar.TopColor = Color.Green;
            _enemyProgBar.LoadGraphics(graphics);
        }

        public void UnloadGraphics()
        {
            _towerProgBar.Unload();
            _enemyProgBar.Unload();
        }

        public void Update(float elapsedSeconds) { }

        public void Draw(GraphicsDevice device, SpriteBatch sb)
        {
            if (Session == null)
                return;

            int halfCell = (int)(Session.Grid.CellSize / 2);

            _cellRect.Width = (int)Session.Grid.CellSize;
            _cellRect.Height = (int)Session.Grid.CellSize;

            Rectangle progRect = new Rectangle(0, 0, (int)(Session.Grid.CellSize * 0.8f), 5);

            foreach (var tower in Session.Towers)
            {
                var t = tower;
                _cellRect.X = (int)tower.Position.X - halfCell;
                _cellRect.Y = (int)tower.Position.Y - halfCell;
                sb.Draw(t.Data.Texture, _cellRect, Color.White);

                var prog = t.BuildProgress;

                if (prog < 1)
                {
                    progRect.X = _cellRect.X + (int)(Session.Grid.CellSize * 0.1f);
                    progRect.Y = _cellRect.Y + 5;
                    _towerProgBar.Draw(sb, prog, progRect);
                }
            }

            foreach (var enemyInstance in Session.Enemies)
            {
                var e = enemyInstance;
                _cellRect.X = (int)enemyInstance.Position.X - halfCell;
                _cellRect.Y = (int)enemyInstance.Position.Y - halfCell;
                sb.Draw(e.Data.Texture, _cellRect, Color.White);

                if (e.ShowHealth)
                {
                    progRect.X = _cellRect.X + (int)(Session.Grid.CellSize * 0.1f);
                    progRect.Y = _cellRect.Y + 5;
                    _enemyProgBar.Draw(sb, e.Health / e.ModifiedTotalHealth, progRect);
                }
            }

            foreach (var tower in Session.Towers)
            {
                if (tower.Weapon != null)
                    tower.Weapon.Draw(device, sb);
            }
        }
    }
}
