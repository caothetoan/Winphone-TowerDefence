using System;
using System.IO.IsolatedStorage;
using Coding4Fun.ScriptTD.Engine;
using Coding4Fun.ScriptTD.Engine.Data.Weapons;
using Coding4Fun.ScriptTD.Engine.Logic;
using Coding4Fun.ScriptTD.Engine.Profile;
using Coding4Fun.ScriptTD.Sample.Screens;
using Microsoft.Xna.Framework;

namespace Coding4Fun.ScriptTD.Sample
{
    public class SampleGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private ScreenManager _screens;

        public SampleGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            _graphics.IsFullScreen = true;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Settings.Save();
            using (var bw = ProfileManager.CurrentProfile.Write(ProgressTracker.ProgressFile))
            {
                ProgressTracker.Save(bw);
            }

            ProfileManager.CurrentProfile.CloseStream();
            HighScores.Save();

            _screens.CloseScreen();

            base.OnExiting(sender, args);
        }

        protected override void Initialize()
        {
            RegisterWeapons();

            ProfileManager.LoadProfiles();
            if (ProfileManager.Profiles.Count == 0)
                ProfileManager.CurrentProfile = new Profile("default");

            Settings.Load();

            Audio.Initialize(Services);

            try
            {
                using (var br = ProfileManager.CurrentProfile.Read(ProgressTracker.ProgressFile))
                {
                    ProgressTracker.Load(br);
                }
                ProfileManager.CurrentProfile.CloseStream();
                HighScores.Load();
            }
            catch (IsolatedStorageException) { }

            _screens = new ScreenManager(Services, GraphicsDevice, this);

            MainMenuScreen scr = new MainMenuScreen(Content);

            _screens.AddScreen(MainMenuScreen.Name, scr);
            _screens.CurrentScreen = scr;

            // Preload the victory and defeat screens
            var vs = new ContinueScreen(true) { Content = Content };
            _screens.AddScreen(ContinueScreen.VictoryName, vs);

            var ds = new ContinueScreen(false) { Content = Content };
            _screens.AddScreen(ContinueScreen.DefeatName, ds);

            base.Initialize();
        }

        private static void RegisterWeapons()
        {
            Armory.AddWeaponType("DumbProjectile", typeof(DumbProjectile));
            Armory.AddWeaponType("SmartProjectile", typeof(SmartProjectile));
            Armory.AddWeaponType("Laser", typeof(Laser));
            Armory.AddWeaponType("RadialShockwave", typeof(RadialShockwave));
        }

        protected override void LoadContent()
        {
            _screens.LoadAllContent(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Audio.UnloadContent();
            _screens.UnloadAllContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Audio.Update();

            _screens.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _screens.Draw(GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
