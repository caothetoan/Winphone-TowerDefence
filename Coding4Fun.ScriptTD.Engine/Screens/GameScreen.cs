using System;
using System.Collections.Generic;
using System.Linq;
using Coding4Fun.ScriptTD.Engine.Data;
using Coding4Fun.ScriptTD.Engine.GUI;
using Coding4Fun.ScriptTD.Engine.GUI.Visuals;
using Coding4Fun.ScriptTD.Engine.Logic;
using Coding4Fun.ScriptTD.Engine.Profile;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Coding4Fun.ScriptTD.Engine.Screens
{
    public class GameScreen : IScreen
    {
        public const string Name = "game";

        public ScreenManager Manager { get; private set; }

        public bool IsLoaded { get; private set; }

        public bool IsInitialized { get; private set; }

        public GameSession Game;

        private readonly Random _rng = new Random();

        private BackgroundLoader _bgLoader;
        private RenderEngine _renderer, _tmRenderer;
        private Window _gui;
        private SpriteBatch _sb;

        private Window _towerMenu;

        private bool _backDown;

        private readonly Vector2 _gridTopLeft = new Vector2(67.5f, 47.5f);
        private const float GridCellSize = 35f;

        private float _timeMult = 1f;

        private Action<GameSession, bool> _completeHandler;

        private readonly Dictionary<string, TowerButton> _towerButtons = new Dictionary<string, TowerButton>();

        private string _selectedTowerId;
        private Texture2D _selectedTowerTex;
        private float _selectedTowerCost;
        private Vector2 _dragPos;
        private readonly Vector2 _dragOffset = new Vector2(-75f);
        private Vector2 _drawOffset;
        private bool _canDrop;
        private Rectangle _testRect;

        private readonly List<Path> _testPaths = new List<Path>();

        private CircleVisual _dragRangeCircle, _towerRangeCircle;

        private TowerInstanceButton _selectedBtn;

        private bool _isLoading;

        public void Initialize(ScreenManager manager)
        {
            Manager = manager;
            _bgLoader = new BackgroundLoader(manager.Services);
            _completeHandler = OnGameComplete;
            IsInitialized = true;
        }

        public void LoadGraphics(GraphicsDevice device)
        {
            if (_isLoading)
                return;

            if (!Manager.HasScreen(OptionsScreen.Name))
                Manager.AddScreen(OptionsScreen.Name, new OptionsScreen());

            _bgLoader.BeginLoad(c =>
                                    {
                                        _gui = c.Load<Window>(@"Data\GUI\Ingame");
                                        _renderer = new RenderEngine();

                                        _towerMenu = c.Load<Window>(@"Data\GUI\TowerMenu");
                                        _towerMenu.Visible = false;
                                        _towerMenu.Enabled = false;
                                        _tmRenderer = new RenderEngine();

                                        _gui.RegisterVisuals(_renderer);
                                        _towerMenu.RegisterVisuals(_tmRenderer);

                                        if (_gui.NamedVisuals.ContainsKey("rangeCircle") && _gui.NamedVisuals["rangeCircle"] is CircleVisual)
                                        {
                                            _dragRangeCircle = (CircleVisual)_gui.NamedVisuals["rangeCircle"];
                                            _dragRangeCircle.ScreenWidth = (int)_gui.Size.X;
                                            _dragRangeCircle.ScreenHeight = (int)_gui.Size.Y;
                                            _dragRangeCircle.Visible = false;

                                            _towerRangeCircle = new CircleVisual
                                                                    {
                                                                        Density = _dragRangeCircle.Density,
                                                                        Color = _dragRangeCircle.Color,
                                                                        ScreenWidth = _dragRangeCircle.ScreenWidth,
                                                                        ScreenHeight = _dragRangeCircle.ScreenHeight,
                                                                        Visible = false,
                                                                        Owner = _gui
                                                                    };

                                            _gui.Visuals.Add(_towerRangeCircle);
                                            _renderer.Visuals.Add(_towerRangeCircle);
                                        }

                                        _renderer.LoadGraphics(c, device);
                                        _tmRenderer.LoadGraphics(c, device);

                                        AssociateGuiCommands();

                                        if (Game != null && !Game.Loaded)
                                        {
                                            Game.Grid.CellSize = GridCellSize;
                                            Game.Grid.TopLeft = _gridTopLeft;
                                            Game.LoadMapData(c);

                                            if (_gui.NamedVisuals.ContainsKey("mapImg"))
                                                ((ImageVisual)_gui.NamedVisuals["mapImg"]).Texture = Game.Map.Texture;

                                            if (_gui.NamedVisuals.ContainsKey("gameplay"))
                                                ((GameplayVisual)_gui.NamedVisuals["gameplay"]).Session = Game;

                                            _drawOffset = new Vector2(-GridCellSize / 2);
                                            _testRect.X = (int)Game.Grid.TopLeft.X;
                                            _testRect.Y = (int)Game.Grid.TopLeft.Y;
                                            _testRect.Width = (int)(Game.Grid.Columns * Game.Grid.CellSize);
                                            _testRect.Height = (int)(Game.Grid.Rows * Game.Grid.CellSize);
                                        }

                                        _sb = new SpriteBatch(device);

                                        Manager.GetScreen<OptionsScreen>(OptionsScreen.Name).LoadGraphics(device);

                                        Audio.RegisterPlayList(Name, ref _gui.Playlist);

                                        IsLoaded = true;
                                    });
            _isLoading = true;
        }

        private void AssociateGuiCommands()
        {
            if (_gui.Controls.ContainsKey("forceWave"))
                _gui.Controls["forceWave"].Tapped += (c, v) => Game.ForceWaveStart();

            if (_gui.Controls.ContainsKey("pause"))
                _gui.Controls["pause"].Tapped += (c, v) => SetSpeed(0);

            if (_gui.Controls.ContainsKey("play"))
                _gui.Controls["play"].Tapped += (c, v) => SetSpeed(1);

            if (_gui.Controls.ContainsKey("increaseSpeed"))
                _gui.Controls["increaseSpeed"].Tapped += (c, v) => SetSpeed(((_timeMult) % 3) + 1);

            if (_gui.Controls.ContainsKey("mainmenu"))
                _gui.Controls["mainmenu"].Tapped += (c, v) => Manager.TransitionTo(MainMenuScreen.Name);

            _gui.Dragging += TowerDragging;
            _gui.DragFinished += TowerDragFinished;

            _gui.Tapped += (c, v) =>
                               {
                                   _selectedBtn = null;
                                   _towerMenu.Visible = false;
                                   _towerMenu.Enabled = false;
                                   _towerRangeCircle.Visible = false;
                               };

            if (_towerMenu.Controls.ContainsKey("sell"))
                _towerMenu.Controls["sell"].Tapped += (c, v) =>
                    {
                        Game.SellTower(_selectedBtn.Instance);
                        _gui.Controls.Remove(_selectedBtn.Name);
                        _selectedBtn = null;
                        _towerMenu.Visible = false;
                        _towerMenu.Enabled = false;
                        _towerRangeCircle.Visible = false;
                    };

            if (_towerMenu.Controls.ContainsKey("upgrade"))
            {
                _towerMenu.Controls["upgrade"].Tapped += (c, v) =>
                    {
                        if (Game.UpgradeTower(_selectedBtn.Instance, false))
                        {
                            _selectedBtn.CanUpgrade = Game.CanUpgrade(_selectedBtn.Instance.Data.TowerId, _selectedBtn.Instance.Data.TowerLevel + 1, out _selectedBtn.UpgradeCost);
                        }
                    };
            }

            _towerButtons.Clear();
            foreach (TowerButton ctrl in _gui.Controls.Values.OfType<TowerButton>())
            {
                _towerButtons.Add((ctrl).TowerId, ctrl);
                ((IControl)ctrl).Dragging += TowerDragging;
            }

            if (_gui.Controls.ContainsKey("options"))
            {
                _gui.Controls["options"].Tapped += (c, v) =>
                                                       {
                                                           var s = Manager.GetScreen<OptionsScreen>(OptionsScreen.Name);
                                                           s.PrevScreen = this;
                                                           Manager.TransitionTo(s);
                                                       };
            }
        }

        private void SetSpeed(float speed)
        {
            _timeMult = speed;
            IControl btn;
            if (_gui.Controls.TryGetValue("pause", out btn))
                btn.Enabled = _timeMult != 0;
            if (_gui.Controls.TryGetValue("play", out btn))
                btn.Enabled = _timeMult == 0;
            if (_gui.Controls.TryGetValue("speed1", out btn))
                btn.Enabled = _timeMult == 1;
            if (_gui.Controls.TryGetValue("speed2", out btn))
                btn.Enabled = _timeMult == 2;
            if (_gui.Controls.TryGetValue("speed3", out btn))
                btn.Enabled = _timeMult == 3;
        }

        private void TowerDragging(IControl sender, Vector2 position)
        {
            if (sender is TowerButton)
            {
                if (_selectedTowerId == null)
                {
                    var btn = (TowerButton)sender;
                    _selectedTowerId = btn.TowerId;
                    _selectedTowerTex = btn.TowerDragTex;
                    _selectedTowerCost = btn.TowerCost;
                    _dragPos = position + _dragOffset;

                    if (_dragRangeCircle != null)
                    {
                        _dragRangeCircle.Location = _dragPos;
                        _dragRangeCircle.InnerRadius = btn.InnerRadius;
                        _dragRangeCircle.OuterRadius = btn.OuterRadius;
                        _dragRangeCircle.Visible = true;
                    }
                }
            }
            else if (sender is Window)
            {
                if (_selectedTowerId != null)
                {
                    _dragPos = position + _dragOffset;

                    if (_dragRangeCircle != null)
                        _dragRangeCircle.Location = _dragPos;

                    if (_testRect.Contains((int)_dragPos.X, (int)_dragPos.Y))
                    {
                        int x, y;
                        Game.Grid.GetCell(_dragPos, out x, out y);

                        if (Game.Grid.IsCellValid(x, y) && Game.Grid[x, y].CellType == MapData.SpecialCellType.None && Game.Grid[x, y].Tower == null)
                        {
                            _canDrop = true;
                        }
                        else
                        {
                            _canDrop = false;
                        }
                    }
                    else
                    {
                        _canDrop = false;
                    }
                }
            }
        }

        private void TowerDragFinished(IControl sender, Vector2 position)
        {
            if (_selectedTowerId != null && sender is Window)
            {
                if (_canDrop)
                {
                    int x, y;
                    Game.Grid.GetCell(_dragPos, out x, out y);

                    bool drop = true;
                    Game.Grid[x, y].InvalidCell = true;

                    foreach (Path t in _testPaths)
                    {
                        if (!t.IsPathValid())
                        {
                            var p = t;
                            if (!(drop &= Game.Paths.CalculateLandPath(ref p)))
                                break;
                        }
                    }
                    Game.Grid[x, y].InvalidCell = false;

                    if (drop)
                    {
                        var t = Game.BuildTower(_selectedTowerId, 1, x, y, false);

                        if (t != null)
                        {
                            var btn = new TowerInstanceButton { Instance = t };
                            btn.CanUpgrade = Game.CanUpgrade(t.Data.TowerId, t.Data.TowerLevel + 1, out btn.UpgradeCost);
                            btn.Location = Game.Grid.GetCellCenter(x, y) - new Vector2(GridCellSize / 2);
                            btn.Size = new Vector2(GridCellSize);
                            btn.Parent = _gui;
                            btn.Enabled = true;
                            btn.RecalculateBounds();
                            btn.Name = t.Data.TowerId + t.Data.TowerLevel + _rng.Next();
                            btn.Tapped += (c, v) =>
                                              {
                                                  _selectedBtn = (TowerInstanceButton)c;
                                                  _towerMenu.Enabled = true;
                                                  _towerMenu.Visible = true;
                                                  _towerMenu.Location = _selectedBtn.Location + new Vector2(GridCellSize / 2) - (_towerMenu.Size / 2);
                                                  _towerMenu.RecalculateBounds();

                                                  foreach (var ctrl in _towerMenu.Controls.Values)
                                                  {
                                                      ctrl.RecalculateBounds();
                                                  }

                                                  _towerRangeCircle.Location = _selectedBtn.Location + new Vector2(GridCellSize / 2);
                                                  _towerRangeCircle.InnerRadius = _selectedBtn.Instance.Data.MinRange * GridCellSize;
                                                  _towerRangeCircle.OuterRadius = _selectedBtn.Instance.Data.MaxRange * GridCellSize;
                                                  _towerRangeCircle.Visible = true;
                                              };
                            _gui.Controls.Add(btn.Name, btn);
                        }
                    }
                }
                _selectedTowerId = null;

                if (_dragRangeCircle != null)
                    _dragRangeCircle.Visible = false;
            }
        }

        public void UnloadContent()
        {
            _bgLoader.UnloadContent();

            IsLoaded = false;
            _isLoading = false;
        }

        public void Update(float elapsedSeconds)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();
                _towerMenu.BubbleGesture(ref gesture);
                _gui.BubbleGesture(ref gesture);
            }

            _gui.Update(elapsedSeconds);
            _towerMenu.Update(elapsedSeconds);

            _renderer.Update(elapsedSeconds);
            _tmRenderer.Update(elapsedSeconds);

            Game.Update(elapsedSeconds * _timeMult);

            if (_gui.Controls.ContainsKey("cash"))
                _gui.Controls["cash"].Value = Game.RemainingCash;

            if (_gui.Controls.ContainsKey("lives"))
                _gui.Controls["lives"].Value = Game.RemainingLives;

            if (_gui.Controls.ContainsKey("wave"))
                _gui.Controls["wave"].Value = Game.WaveNumber;

            if (_gui.Controls.ContainsKey("points"))
                _gui.Controls["points"].Value = Game.CurrentPoints;

            if (_gui.Controls.ContainsKey("forceWave"))
                _gui.Controls["forceWave"].Enabled = Game.TimeToWaveSpawn > 0;

            if (_gui.Controls.ContainsKey("costDisplay"))
            {
                if (_selectedTowerId != null)
                    _gui.Controls["costDisplay"].Value = _selectedTowerCost;
                _gui.Controls["costDisplay"].Visible = _selectedBtn != null || _selectedTowerId != null;
            }

            foreach (var tower in Game.Map.AvailableTowers)
            {
                if (tower.TowerLevel == 1 && _towerButtons.ContainsKey(tower.TowerId))
                {
                    _towerButtons[tower.TowerId].Enabled = Game.RemainingCash >= tower.Cost;
                }
            }

            if (_selectedBtn != null && _towerMenu.Controls.ContainsKey("upgrade"))
            {
                _towerMenu.Controls["upgrade"].Enabled = _selectedBtn.CanUpgrade && (_selectedBtn.UpgradeCost <= Game.RemainingCash);
                if (_towerMenu.Controls["upgrade"].Enabled && _gui.Controls.ContainsKey("costDisplay") && _selectedTowerId == null)
                    _gui.Controls["costDisplay"].Value = _selectedBtn.UpgradeCost;
            }

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                _backDown = true;
            }
            else if (_backDown)
            {
                _backDown = false;
                Manager.TransitionTo(MainMenuScreen.Name);
            }
        }

        public void Draw(GraphicsDevice device)
        {
            device.Clear(Color.Black);
            _renderer.Draw(device, _sb);
            _tmRenderer.Draw(device, _sb);

            if (_selectedTowerId != null)
            {
                _sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                _sb.Draw(_selectedTowerTex, _dragPos + _drawOffset, _canDrop ? Color.White : Color.Red);
                _sb.End();
            }
        }

        public void OnNavigateTo()
        {
            Game.GameComplete += _completeHandler;
            _selectedTowerId = null;

            if (!Game.IsInProgress)
            {
                Game.InitializeNewMap();
            }
            SetSpeed(1);
            SetTowerButtons();
            AcquirePaths();
            Audio.PlayPlaylist(Name, true);
        }

        private void AcquirePaths()
        {
            for (int i = 0; i < Game.Map.CellCombos.Count; i++)
            {
                var cell1 = Game.Map.CellCombos[i].Cell1;
                var cell2 = Game.Map.CellCombos[i].Cell2;
                var path = Game.Paths.GetPath(Game.Grid[cell1.X, cell1.Y], Game.Grid[cell2.X, cell2.Y], false);
                _testPaths.Add(path);
            }
        }

        private void SetTowerButtons()
        {
            foreach (var btn in _towerButtons)
            {
                btn.Value.Visible = false;
                btn.Value.Enabled = false;
            }

            foreach (var tower in Game.Map.AvailableTowers)
            {
                if (tower.TowerLevel == 1 && _towerButtons.ContainsKey(tower.TowerId))
                {
                    TowerButton btn = _towerButtons[tower.TowerId];
                    btn.Visible = true;
                    btn.TowerDragTex = tower.Texture;
                    btn.InnerRadius = tower.MinRange * GridCellSize;
                    btn.OuterRadius = tower.MaxRange * GridCellSize;
                    btn.TowerCost = tower.Cost;
                }
            }
        }

        public void OnNavigateAway()
        {
            Game.GameComplete -= _completeHandler;

            foreach (var path in _testPaths)
                Game.Paths.ReleasePath(path);

            _testPaths.Clear();

            if (Game.IsInProgress)
            {
                using (var bw = ProfileManager.CurrentProfile.Write("snapshot.dat"))
                {
                    Game.SaveState(bw);
                }
                ProfileManager.CurrentProfile.CloseStream();
            }
        }

        private void OnGameComplete(GameSession session, bool isVictory)
        {
            if (ProfileManager.CurrentProfile.FileExists("snapshot.dat"))
            {
                ProfileManager.CurrentProfile.DeleteFile("snapshot.dat");
            }

            if (isVictory)
            {
                ProgressTracker.CompleteLevel(Game.Map.Id);
                HighScores.SubmitScore(Game.Map.Id, Game.CurrentPoints);
                Manager.TransitionTo(ContinueScreen.VictoryName);
            }
            else
            {
                Manager.TransitionTo(ContinueScreen.DefeatName);
            }
        }
    }
}
