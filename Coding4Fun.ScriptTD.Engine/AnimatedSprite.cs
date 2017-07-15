using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coding4Fun.ScriptTD.Engine
{
    public class AnimatedSprite
    {
    	private Texture2D _tex;
        private Rectangle _source;

		private int _maxFrames = 1;
		private double _frameTime = 1;

        private int _curFrame;
        private double _timeAccum;
        
		private readonly Vector2 _origin;

        public bool IsLooping
        {
            get;
            set;
        }

    	public bool IsPlaying { get; private set; }

    	public AnimatedSprite(Point frameSize, bool isLooping)
        {
        	IsPlaying = false;
        	_source = new Rectangle(0, 0, frameSize.X, frameSize.Y);
            _origin = new Vector2(frameSize.X / 2f, frameSize.Y / 2f);
            IsLooping = isLooping;
        }

        public void LoadContent(ContentManager content, string path)
        {
            SetTexture(content.Load<Texture2D>(path));
        }

        public void SetTexture(Texture2D texture)
        {
            _tex = texture;
            _maxFrames = _tex.Width / _source.Width;
        }

        public void Update(float elapsedSeconds)
        {
            if (IsPlaying)
            {
                _timeAccum += elapsedSeconds;

                if (_timeAccum >= _frameTime)
                {
                    _curFrame++;

                    if (_curFrame >= _maxFrames)
                    {
                        if (IsLooping)
                            _curFrame = 0;
                        else
                            Stop();
                    }

                    UpdateSourceRect();

                    _timeAccum -= _frameTime;
                }
            }
        }

        private void UpdateSourceRect()
        {
            _source.X = _source.Width * _curFrame;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float depth)
        {
            if (_tex != null)
                spriteBatch.Draw(_tex, position, _source, Color.White, 0f, _origin, 1f, SpriteEffects.None, depth);
        }

        public void Play(double framesPerSecond)
        {
            IsPlaying = true;
            _frameTime = 1 / framesPerSecond;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Reset()
        {
            _curFrame = 0;
            UpdateSourceRect();
        }
    }
}
