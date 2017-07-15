using Coding4Fun.ScriptTD.Engine.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Coding4Fun.ScriptTD.Common.GUI;

namespace Coding4Fun.ScriptTD.Sample.Screens
{
	public class HelpScreen : IScreen
	{
		public const string Name = "help";

		public ScreenManager Manager { get; private set; }

		public bool IsLoaded { get; private set; }

		public bool IsInitialized { get; private set; }

		private BackgroundLoader _bg;
		private SpriteBatch _sb;

		private Window _window;
		private RenderEngine _renderer;

		private IControl _prevBtn, _nextBtn;
		private Slideshow _slides;

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

			              		_window = c.Load<Window>(@"Data\GUI\HelpScreen");
			              		RegisterControls();
			              		_renderer = new RenderEngine();
			              		_window.RegisterVisuals(_renderer);
			              		_renderer.LoadGraphics(c, device);

			              		UpdateButtons();

			              		IsLoaded = true;
			              	});
		}

		private void RegisterControls()
		{
			_window.Controls.TryGetValue("next", out _nextBtn);
			_window.Controls.TryGetValue("prev", out _prevBtn);

			if (_nextBtn != null)
				_nextBtn.Tapped += (c, p) => NextPage();
			
			if (_prevBtn != null)
				_prevBtn.Tapped += (c, p) => PrevPage();
			
			IControl tmp;
			if (_window.Controls.TryGetValue("slides", out tmp) && tmp is Slideshow)
				_slides = (Slideshow)tmp;
		}

		private void NextPage()
		{
			if (_slides != null)
			{
				_slides.NextSlide();
				UpdateButtons();
			}
		}

		private void PrevPage()
		{
			if (_slides != null)
			{
				_slides.PrevSlide();
				UpdateButtons();
			}
		}

		private void UpdateButtons()
		{
			if (_slides != null)
			{
				if (_nextBtn != null)
				{
					_nextBtn.Visible = _slides.CanAdvance;
					_nextBtn.Enabled = _nextBtn.Visible;
				}

				if (_prevBtn != null)
				{
					_prevBtn.Visible = _slides.CanGoBack;
					_prevBtn.Enabled = _prevBtn.Visible;
				}
			}
		}

		public void UnloadContent()
		{
			_bg.UnloadContent();
		}

		public void Update(float elapsedSeconds)
		{
			_window.UpdateInput();
			_window.Update(elapsedSeconds);
			_renderer.Update(elapsedSeconds);

			if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
			{
				_backPressed = true;
			}
			else if (_backPressed)
			{
				_backPressed = false;
				Manager.TransitionTo(MainMenuScreen.Name);
			}
		}

		public void Draw(GraphicsDevice device)
		{
			_renderer.Draw(device, _sb);
		}

		public void OnNavigateTo()
		{
			_backPressed = false;
			
			if (_slides != null)
				_slides.Reset();

			UpdateButtons();
		}

		public void OnNavigateAway() { }
	}
}