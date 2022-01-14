﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using System.Diagnostics;
using Jeux.Perso;

namespace Jeux.Screen
{
    class Level1 : GameScreen
    {
        private Game1 _game1; // pour récupérer la fenêtre de jeu principale

        private TiledMap _map;

        private TiledMapRenderer _renduMap;

        private OrthographicCamera _camera;

        private Vector2 _cameraPosition;

        bool idleRight = true;

        private int coef, speed = 10;

        Vector2 velocity;

        readonly Vector2 gravity = new Vector2(0, 200f);

        bool jump = false, ecran;

        private Joueur _joueur;

        // private TypeCollision _collision;

        private static int WIDTH_FENETRE = 1920;
        private static int HEIGHT_FENETRE = 992;

        public Level1(Game1 game) : base(game)
        {
            _game1 = game;
        }

        public override void Initialize()
        {
            // TODO: Add your initialization logic here

            _game1.Graphics.PreferredBackBufferWidth = WIDTH_FENETRE;
            _game1.Graphics.PreferredBackBufferHeight = HEIGHT_FENETRE;
            _game1.Graphics.ApplyChanges();

            var viewportadapter = new BoxingViewportAdapter(_game1.Window, GraphicsDevice, WIDTH_FENETRE, HEIGHT_FENETRE);
            _camera = new OrthographicCamera(viewportadapter);
            _cameraPosition = new Vector2(WIDTH_FENETRE/2, HEIGHT_FENETRE/2);

            _joueur = new Joueur("perso", Vector2.Zero);

            _game1.IsMouseVisible = false;

            base.Initialize();

        }
        public override void LoadContent()
        {

            _map = Content.Load<TiledMap>("level1/1Eta");
            _renduMap = new TiledMapRenderer(GraphicsDevice, _map);
           // _joueur.Create(_game1);            

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float walkSpeed = elapsedTime * 300, walkSpeedVirtuel = elapsedTime * 300;

            KeyboardState keyboardState = Keyboard.GetState();
            
            Vector2 deplacement = Vector2.Zero;

            Vector2 positionVirtuelle = Vector2.Zero;
            
            float positionColonnePerso = (_game1.PositionPerso.X / _map.TileWidth);

            float positionLignePerso = ((_game1.PositionPerso.Y + _game1.Perso.TextureRegion.Height/2) / _map.TileHeight);

            velocity.X = 0;

            bool toucheBordFenetre = false; 
            
            ecran = false;

            //animation idle
            if (idleRight)
                _game1.Animation = TypeAnimation.idleRight;
            else
                _game1.Animation = TypeAnimation.idleLeft;

            /*if (_game1.PositionPerso.X > WIDTH_FENETRE / 2)
                walkSpeed = 0;*/

            //touche du haut + echelle
            if (keyboardState.IsKeyDown(Keys.Up) && IsCollision(positionColonnePerso, positionLignePerso - 3, "echelles"))
            {
                _game1.Animation = TypeAnimation.climb;
                toucheBordFenetre = _game1.PositionPerso.Y - _game1.Perso.TextureRegion.Height / 2 <= 0;
                //Collision = IsCollision(positionColonnePerso, positionLignePerso - 1);
                deplacement = -Vector2.UnitY;
            } // touche du bas + echelle
            else if (keyboardState.IsKeyDown(Keys.Down) && IsCollision(positionLignePerso, positionColonnePerso, "echelles"))
            {
                _game1.Animation = TypeAnimation.climb;
                toucheBordFenetre = _game1.PositionPerso.Y + _game1.Perso.TextureRegion.Height / 2 >= GraphicsDevice.Viewport.Height;
               //Collision = IsCollision(positionColonnePerso, positionLignePerso + 1);
                deplacement = Vector2.UnitY;
            }//touche de droite + pas de saut
            else if (keyboardState.IsKeyDown(Keys.Left) && jump)
            {
                _game1.Animation = TypeAnimation.walkLeft;
                toucheBordFenetre = _game1.PositionPerso.X - _game1.Perso.TextureRegion.Width / 2 <= 0;
                //Collision = IsCollision(positionColonnePerso - 1, positionLignePerso);
                deplacement = -Vector2.UnitX;
                idleRight = false;
                ecran = true;
            } //touche de gauche + pas de saut
            else if (keyboardState.IsKeyDown(Keys.Right) && jump)
            {
                _game1.Animation = TypeAnimation.walkRight;
                toucheBordFenetre = _game1.PositionPerso.X + _game1.Perso.TextureRegion.Width / 2 >= GraphicsDevice.Viewport.Width ;
                //Collision = IsCollision(positionColonnePerso + 1, positionLignePerso);
                deplacement = Vector2.UnitX;
                idleRight = true;
                ecran = true;

            } // saut + pas de saut
            else if (keyboardState.IsKeyDown(Keys.Space) && jump)
            {
                _game1.Animation = TypeAnimation.jumpLeft;
                velocity.Y = 100f;

            }
            else if(keyboardState.IsKeyDown(Keys.X))
            {
                if (idleRight) _game1.Animation = TypeAnimation.hitRight;
                else _game1.Animation = TypeAnimation.hitLeft;
            }

            //deplacement
            if (IsCollision(positionColonnePerso, positionLignePerso, "sol") && !toucheBordFenetre)
            {
                _game1.PositionPerso += walkSpeed * deplacement;
                positionVirtuelle += walkSpeedVirtuel * deplacement;
            }
            

            // gravité si pas en colision avec le sol et pas de saut
            if (!jump || !IsCollision(positionColonnePerso, positionLignePerso, "sol"))
                velocity.Y += gravity.Y * elapsedTime;
            else
                velocity.Y = 0;

            //si en colision avec le sol, il peut sauter
            if (IsCollision(positionColonnePerso, positionLignePerso, "sol"))
                jump = true;

// velocity.Y = 0;

            //debug
            Debug.WriteLine($"Viwport : {_game1.GraphicsDevice.Viewport.Width}" +
                $"Perso X : {_game1.PositionPerso.X}" +
                $"En collision : {IsCollision(positionColonnePerso, positionLignePerso, "sol")}" +
                $"Echelle : {IsCollision(positionColonnePerso, positionLignePerso - 2, "echelles")}" +
                $"\nPerso x Virtuel : {positionVirtuelle}");

            _game1.PositionPerso += velocity * elapsedTime;

            // _joueur.Move(gameTime, _map, "sol", "echelles");


            MoveCamera(gameTime);
            _camera.LookAt(_cameraPosition);

            _game1.Perso.Play(_game1.Animation.ToString());
            _game1.Perso.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            _game1.GraphicsDevice.Clear(Color.Red);
            _game1.SpriteBatch.Begin();
            _game1.SpriteBatch.Draw(_game1.Perso, _game1.PositionPerso);
            //_game1.SpriteBatch.Draw(_joueur.JoueurP, _joueur.Position);
            _renduMap.Draw(_camera.GetViewMatrix());
            _game1.SpriteBatch.End();
        }

        private Vector2 GetMovementDirection()
        {
            var movementDirection = Vector2.Zero;
            var state = Keyboard.GetState();

            if (!idleRight && ecran)
            {
                movementDirection -= speed * Vector2.UnitX;
            }
            if (idleRight && ecran)
            {
                movementDirection += new Vector2((float)0.5,0);
            }

/*               if (state.IsKeyDown(Keys.Right))
            {
                movementDirection += Vector2.UnitX;
            }
              */
            // Can't normalize the zero vector so test for it before normalizing

            if (movementDirection != Vector2.Zero)
            {
                movementDirection.Normalize();
            }

            return movementDirection;
        }

        private void MoveCamera(GameTime gameTime)
        {
            var speed = gameTime.GetElapsedSeconds() * 300;
            var seconds = gameTime.GetElapsedSeconds();
            var movementDirection = GetMovementDirection();

            if (_game1.PositionPerso.X < WIDTH_FENETRE / 2)
                _cameraPosition.X = WIDTH_FENETRE / 2;
            else if (_game1.PositionPerso.X > _map.WidthInPixels - (WIDTH_FENETRE/2))
                _cameraPosition.X = _map.WidthInPixels - (WIDTH_FENETRE / 2);
            else
                _cameraPosition += speed * movementDirection;
        }

        private bool IsCollision(float x, float y, string layer)
        {
            TiledMapTile? tile;
            TiledMapTileLayer _obstacleLayer;
            _obstacleLayer = _map.GetLayer<TiledMapTileLayer>(layer);
            if (_obstacleLayer.TryGetTile((ushort)x, (ushort)y, out tile) == false)
            {
                return false;
            }
            if (!tile.Value.IsBlank)
            {
                return true;
            }
            return false;
        }
    }
}