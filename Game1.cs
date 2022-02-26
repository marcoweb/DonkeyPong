using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DonkeyPong
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Tela de Fundo
        private Texture2D backgroundSprinte;

        // Jogador 1
        private Texture2D playerOneSprite;
        private Vector2 playerOnePosition;
        private Vector2 playerOneVelocity;
        private int playerOneScore;

        // Jogador 2
        private Texture2D playerTwoSprite;
        private Vector2 playerTwoPosition;
        private Vector2 playerTwoVelocity;
        private int playerTwoScore;

        // Bola
        private Texture2D cannonBallSprite;
        private Vector2 cannonBallPosition;
        private Vector2 cannonBallVelocity;

        // Tela de Jogo
        private int ScreenTop = 190;
        private int screenBotton = 500;

        // Explosão
        private Texture2D explosionSprite;
        private Vector2 explosionPosition;
        private Vector2 explosionFrame;
        private bool explosionVisible;
        enum Players { PlayerOne, PlayerTwo }
        private Players sacking;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            this.playerOnePosition = new Vector2(10, (Window.ClientBounds.Height / 2) - 25);
            this.playerOneVelocity = new Vector2(4,4);
            this.playerOneScore = 0;

            this.playerTwoPosition = new Vector2((Window.ClientBounds.Width - 72), (Window.ClientBounds.Height / 2) - 25);
            this.playerTwoVelocity = new Vector2(4, 4);
            this.playerTwoScore = 0;

            this.cannonBallPosition = new Vector2(50, 200);
            this.cannonBallVelocity = new Vector2(4, 4);

            this.explosionPosition = new Vector2(50, 200);
            this.explosionFrame = new Vector2(0, 0);
            this.explosionVisible = true;

            sacking = Players.PlayerOne;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            this.backgroundSprinte = Content.Load<Texture2D>("dkpongBkg");
            this.playerOneSprite = Content.Load<Texture2D>("dkpongBarrel");
            this.playerTwoSprite = Content.Load<Texture2D>("dkpongBarrel");
            this.cannonBallSprite = Content.Load<Texture2D>("dkpongCannonball");
            this.explosionSprite = Content.Load<Texture2D>("dkpongExplosion");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Tecla De Saída (Esc)
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Explosão
            if (this.explosionVisible)
            {
                if (this.explosionFrame.X == 3 && explosionFrame.Y < 3)
                {
                    this.explosionFrame.Y++;
                    this.explosionFrame.X = 0;
                }
                else if (this.explosionFrame.X < 3)
                {
                    this.explosionFrame.X++;
                }
                else
                {
                    this.explosionVisible = false;
                    this.explosionFrame.X = 0;
                    this.explosionFrame.Y = 0;
                }
            }

            // Movimentação do Jogador 1 (W, S)
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (this.playerOneVelocity.Y < 0)
                    this.playerOneVelocity.Y *= -1;
                if(this.playerOnePosition.Y < this.screenBotton)
                    this.playerOnePosition.Y += this.playerOneVelocity.Y;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (this.playerOneVelocity.Y > 0)
                    this.playerOneVelocity.Y *= -1;
                if (this.playerOnePosition.Y > this.ScreenTop + 15)
                    this.playerOnePosition.Y += this.playerOneVelocity.Y;
            }

            // Movimentação do Jogador 2 (UP, DOWN)
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (this.playerTwoVelocity.Y < 0)
                    this.playerTwoVelocity.Y *= -1;
                if (this.playerTwoPosition.Y < this.screenBotton)
                    this.playerTwoPosition.Y += this.playerTwoVelocity.Y;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (this.playerTwoVelocity.Y > 0)
                    this.playerTwoVelocity.Y *= -1;
                if (this.playerTwoPosition.Y > this.ScreenTop + 15)
                    this.playerTwoPosition.Y += this.playerTwoVelocity.Y;
            }

            // Movimentação da Bola
            // - Movimentação horizontal (Bola)
            // -- Verifica se a bola toca nos barris laterais
            Rectangle nextPosition = new Rectangle((int)this.cannonBallPosition.X + (int)this.cannonBallVelocity.X, (int)this.cannonBallPosition.Y, this.cannonBallSprite.Width, this.cannonBallSprite.Height);
            Rectangle barrel = (this.cannonBallVelocity.X > 0) ?
                new Rectangle((int)this.playerTwoPosition.X, (int)this.playerTwoPosition.Y, this.playerTwoSprite.Width, this.playerTwoSprite.Height) :
                new Rectangle((int)this.playerOnePosition.X, (int)this.playerOnePosition.Y, this.playerOneSprite.Width, this.playerOneSprite.Height);
            if (nextPosition.Intersects(barrel))
                this.cannonBallVelocity.X *= -1;
            this.cannonBallPosition.X += this.cannonBallVelocity.X;
            // Verifica se a bola saiu da tela de jogo
            if (this.cannonBallPosition.X < 0 || this.cannonBallPosition.X > Window.ClientBounds.Width)
            {
                if (this.cannonBallPosition.X < 0)
                    this.playerTwoScore++;
                else
                    this.playerOneScore++;
                this.cannonBallPosition.Y = 200;
                this.cannonBallVelocity.X *= -1;
                this.sacking = (this.sacking == Players.PlayerOne) ? Players.PlayerTwo : Players.PlayerOne;
                this.cannonBallPosition.X = (this.sacking == Players.PlayerOne) ? 50 : 710;
                this.explosionPosition.X = this.cannonBallPosition.X;
                this.explosionVisible = true;
            }
            // - Movimentação vertical (Bola)
            if (this.cannonBallPosition.Y > this.screenBotton || this.cannonBallPosition.Y < this.ScreenTop)
                this.cannonBallVelocity.Y *= -1;
            this.cannonBallPosition.Y += this.cannonBallVelocity.Y;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(this.backgroundSprinte, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(this.playerOneSprite, this.playerOnePosition, Color.White);
            _spriteBatch.Draw(this.playerTwoSprite, this.playerTwoPosition, null, Color.White, 0.0f, Vector2.Zero, 1f,SpriteEffects.FlipHorizontally, 0f);
            _spriteBatch.Draw(this.cannonBallSprite, this.cannonBallPosition, Color.White);
            if (this.explosionVisible)
            {
                _spriteBatch.Draw(this.explosionSprite, this.explosionPosition, new Rectangle(
                    (int)this.explosionFrame.X * (this.explosionSprite.Width / 4),
                    (int)this.explosionFrame.Y * (this.explosionSprite.Width / 4),
                    this.explosionSprite.Width / 4, this.explosionSprite.Height / 4), Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
