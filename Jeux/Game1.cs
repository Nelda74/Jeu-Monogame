﻿using Jeux.Perso;
using Jeux.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using System;

namespace Jeux
{
    public enum Ecran { Home, Level1 };
        public enum TypeAnimation { walkRight, walkLeft, climb, hitLeft, hitRight, jumpLeft, jumpRight, idleLeft, idleRight };
        public class Game1 : Game
        {
            private GraphicsDeviceManager _graphics;

            private SpriteBatch _spriteBatch;

            private AnimatedSprite _perso;

            private TypeAnimation _animation;

            private Vector2 _positionPerso;

            private Level1 _screenLevel1;

            private Home _screenHome;

            private Ecran _currentScreen;
        
            public Rectangle mSelectionBox;

            public MouseState mPreviousMouseState;

        private Joueur _joueur;



        private readonly ScreenManager _screenManager;

            public SpriteBatch SpriteBatch
            {
                get
                {
                    return this._spriteBatch;
                }

                set
                {
                    this._spriteBatch = value;
                }
            }

            public AnimatedSprite Perso
            {
                get
                {
                    return this._perso;
                }

                set
                {
                    this._perso = value;
                }
            }

            public Vector2 PositionPerso
            {
                get
                {
                    return this._positionPerso;
                }

                set
                {
                    this._positionPerso = value;
                }
            }

            public TypeAnimation Animation
            {
                get
                {
                    return this._animation;
                }

                set
                {
                    this._animation = value;
                }
            }

            public GraphicsDeviceManager Graphics
            {
                get
                {
                    return this._graphics;
                }

                set
                {
                    this._graphics = value;
                }
            }

            public Game1()
            {
                Graphics = new GraphicsDeviceManager(this);
                Content.RootDirectory = "Content";
                IsMouseVisible = true;
                _screenManager = new ScreenManager();
                Components.Add(_screenManager);

            }

            protected override void Initialize()
            {
            // TODO: Add your initialization logic here
            var _positionPerso = Vector2.Zero; //new Vector2(Level1.WIDTH_FENETRE / 2, Level1.HEIGHT_FENETRE / 2);
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 992;
            //Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();

            mSelectionBox = new Rectangle(-1, -1, 0, 0);
            //Initialize the previous mouse state. This stores the current state of the mouse
            mPreviousMouseState = Mouse.GetState(); 

            base.Initialize();
            }

            protected override void LoadContent()
            {
                SpriteBatch = new SpriteBatch(GraphicsDevice);

                SpriteSheet animation = Content.Load<SpriteSheet>("perso.sf", new JsonContentLoader());
                Perso = new AnimatedSprite(animation);

                _screenHome = new Home(this);
                _screenLevel1 = new Level1(this);
                _screenManager.LoadScreen(_screenHome, new FadeTransition(GraphicsDevice, Color.Black));
                _currentScreen = Ecran.Home;

                // TODO: use this.Content to load your game content here
            }

            protected override void Update(GameTime gameTime)
            {

            if(_screenHome.start)
                _screenManager.LoadScreen(_screenLevel1, new FadeTransition(GraphicsDevice, Color.Black));

            if(_screenHome.exit)
                Exit();

            if (_screenHome.settings)
                Exit();

            base.Update(gameTime);
            }

            protected override void Draw(GameTime gameTime)
            {
                GraphicsDevice.Clear(Color.Black);

                // TODO: Add your drawing code here

                base.Draw(gameTime);
            }
        }
}