using System;
using System.Collections.Generic;
using Coding4Fun.ScriptTD.Engine.Data;
using Coding4Fun.ScriptTD.Engine.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Coding4Fun.ScriptTD.Common.GUI;
using Coding4Fun.ScriptTD.Common.GUI.Visuals;

namespace Coding4Fun.ScriptTD.Sample.Screens
{
	public class HighScoreScreen : IScreen
	{
		public const string Name = "highscores";

		public ScreenManager Manager { get; private set; }

		public bool IsLoaded { get; private set; }

		public bool IsInitialized { get; private set; }

		private bool _listMode;
		private BackgroundLoader _bg;
		private SpriteBatch _sb;

		private Window _listWindow;
		private RenderEngine _listRenderer;

		private Window _itemWindow;
		private RenderEngine _itemRenderer;

		private MapListData _maps;
		private int _start;
		private readonly List<IControl> _thumbControls = new List<IControl>();

		private IControl _prevBtn, _nextBtn;

		private bool _backPressed;

		public void Initialize(ScreenManager manager)
		{
			Manager = manager;
			_bg = new BackgroundLoader(manager.Services);
			IsInitialized = true;
		}

		public void LoadGraphics(GraphicsDevice device)
		{
			_bg.BeginLoad((c) =>
			              	{
			              		_sb = new SpriteBatch(device);

			              		_listWindow = c.Load<Window>(@"Data\GUI\HighScoreMapList");
			              		RegisterListInput();
			              		_itemWindow = c.Load<Window>(@"Data\GUI\HighScoreMap");
			              		RegisterItemInput();

			              		_listRenderer = new RenderEngine();
			              		_listWindow.RegisterVisuals(_listRenderer);

			              		_itemRenderer = new RenderEngine();
			              		_itemWindow.RegisterVisuals(_itemRenderer);

			              		_listRenderer.LoadGraphics(c, device);
			              		_itemRenderer.LoadGraphics(c, device);

			              		_maps = c.Load<MapListData>(@"Data\Maps");
			              		UpdateButtons();

			              		IsLoaded = true;
			              	});
		}

		private void RegisterItemInput()
		{
			if (_itemWindow.Controls.ContainsKey("back"))
			{
				var back = _itemWindow.Controls["back"];
				back.Tapped += (c, v) => _listMode = true;
			}
		}

		private void RegisterListInput()
		{
			_listWindow.Controls["next"].Tapped += (c, p) => NextPage();
			_listWindow.Controls["prev"].Tapped += (c, p) => PrevPage();

			foreach (var control in _listWindow.Controls.Values)
			{
				if (control.Name.StartsWith("level"))
				{
					_thumbControls.Add(control);
					control.Tapped += (c, v) => PopulateItemView(c.Tag, c.Text);
				}
				else
				{
					if (control.Name.Equals("next"))
						_nextBtn = control;
					else if (control.Name.Equals("prev"))
						_prevBtn = control;
				}
			}
		}

		private void PopulateItemView(string mapId, string friendlyName)
		{
			float[] scores;
			if (!HighScores.GetScores(mapId, out scores))
			{
				scores = new float[HighScores.MapScores.MaxScores];
				for (int i = 0; i < scores.Length; i++)
				{
					scores[i] = 0;
				}
			}

			var e = _itemWindow.Controls.GetEnumerator();

			while (e.MoveNext())
			{
				if (e.Current.Key.StartsWith("score", StringComparison.InvariantCultureIgnoreCase))
				{
					var snum = e.Current.Key.Substring(5, e.Current.Key.Length - 5);
					int inum;
					if (int.TryParse(snum, out inum) && inum <= scores.Length)
					{
						e.Current.Value.Value = scores[inum - 1];
					}
					else
					{
						e.Current.Value.Value = 0;
					}
				}
			}

			_itemWindow.Text = friendlyName;
			_listMode = false;
		}

		private void NextPage()
		{
			if ((_start + _thumbControls.Count) < _maps.Maps.Count)
				_start += _thumbControls.Count;

			UpdateButtons();
		}

		private void PrevPage()
		{
			if (_start > 0)
			{
				if ((_start - _thumbControls.Count) < 0)
					_start = 0;
				else
					_start -= _thumbControls.Count;

				UpdateButtons();
			}
		}

		private void UpdateButtons()
		{
			var en = _maps.Maps.GetEnumerator();
			for (int i = 0; i < _start; i++)
			{
				en.MoveNext();
			}

			foreach (var control in _thumbControls)
			{
				if (!en.MoveNext())
				{
					control.Visible = false;
					control.Enabled = false;
					break;
				}

				control.Visible = true;
				control.Enabled = true;
				control.Tag = en.Current.Key;
				control.Text = en.Current.Value.FriendlyName;

				if (control.NamedVisuals.ContainsKey("thumb"))
				{
					var img = (ImageVisual)control.NamedVisuals["thumb"];
					img.Texture = en.Current.Value.Thumbnail;
				}
			}

			if (_nextBtn != null)
			{
				_nextBtn.Visible = (_start + _thumbControls.Count) < _maps.Maps.Count;
				_nextBtn.Enabled = _nextBtn.Visible;
			}

			if (_prevBtn != null)
			{
				_prevBtn.Visible = _start > 0;
			}
		}

		public void UnloadContent()
		{
			_bg.UnloadContent();
		}

		public void Update(float elapsedSeconds)
		{
			if (_listMode)
			{
				_listWindow.UpdateInput();
				_listWindow.Update(elapsedSeconds);
				_listRenderer.Update(elapsedSeconds);
			}
			else
			{
				_itemWindow.UpdateInput();
				_itemWindow.Update(elapsedSeconds);
				_itemRenderer.Update(elapsedSeconds);
			}

			if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
			{
				_backPressed = true;
			}
			else if (_backPressed)
			{
				_backPressed = false;

				if (_listMode)
				{
					Manager.TransitionTo(MainMenuScreen.Name);
				}
				else
				{
					_listMode = true;
				}
			}
		}

		public void Draw(GraphicsDevice device)
		{
			if (_listMode)
			{
				_listRenderer.Draw(device, _sb);
			}
			else
			{
				_itemRenderer.Draw(device, _sb);
			}
		}

		public void OnNavigateTo()
		{
			_listMode = true;
			_backPressed = false;
			UpdateButtons();
		}

		public void OnNavigateAway() { }
	}
}