﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShooterGame.Model;
using ShooterGame.View;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ShooterGame.Controller
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class ShooterGame : Game
	{
		

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		//private Player player;
		Player player;
		// Keyboard states used to determine key presses
		KeyboardState currentKeyboardState;
		KeyboardState previousKeyboardState;

		// Gamepad states used to determine button presses
		GamePadState currentGamePadState;
		GamePadState previousGamePadState;

		// A movement speed for the player
		float playerMoveSpeed;

		// Image used to display the static background
		Texture2D mainBackground;

		// Parallaxing Layers
		ParallaxingBackground bgLayer1;
		ParallaxingBackground bgLayer2;

		// Enemies
		Texture2D enemyTexture;
		List<Enemy> enemies;

		// The rate at which the enemies appear
		TimeSpan enemySpawnTime;
		TimeSpan previousSpawnTime;

		Texture2D tieTexture;
		List<TieFighter> tieFighters;

		TimeSpan tieSpawnTime;
		TimeSpan previousTieSpawnTime;

		// A random number generator
		Random random;

		Texture2D projectileTexture;
		List<Projectile> projectiles;

		Texture2D sineTexture;
		List<SineBeam> sineBeams;

		// The rate of fire of the player laser
		TimeSpan fireTime;
		TimeSpan previousFireTime;

		private Texture2D explosionTexture;
		private List<Animation> explosions;

		// The sound that is played when a laser is fired
		SoundEffect laserSound;

		// The sound used when the player or an enemy dies
		SoundEffect explosionSound;

		// The music played during gameplay
		Song gameplayMusic;

		//Number that holds the player score
		private int score;
		// The font used to display UI elements
		private SpriteFont font;

		public ShooterGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			player = new Player();

			// Set a constant player move speed
			playerMoveSpeed = 8.0f;

			bgLayer1 = new ParallaxingBackground();
			bgLayer2 = new ParallaxingBackground();

			// Initialize the enemies list
			enemies = new List<Enemy> ();

			tieFighters = new List<TieFighter>();
			previousTieSpawnTime = TimeSpan.Zero;
			tieSpawnTime = TimeSpan.FromSeconds(1.0f);

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(1.0f);

			// Initialize our random number generator
			random = new Random();

			projectiles = new List<Projectile>();

			sineBeams = new List<SineBeam>();

			// Set the laser to fire every quarter second
			fireTime = TimeSpan.FromSeconds(.15f);

			explosions = new List<Animation>();

			//Set player's score to zero
			score = 0;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//TODO: use this.Content to load your game content here 
			//Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			// Load the player resources
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animation/shipAnimation");
			playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

			Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
			+ GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			player.Initialize(playerAnimation, playerPosition);

			// Load the parallaxing background
			bgLayer1.Initialize(Content, "Texture/bgLayer1", GraphicsDevice.Viewport.Width, -1);
			bgLayer2.Initialize(Content, "Texture/bgLayer2", GraphicsDevice.Viewport.Width, -2);

			enemyTexture = Content.Load<Texture2D>("Animation/mineAnimation");
			tieTexture = Content.Load<Texture2D>("Animation/TieFighter");

			projectileTexture = Content.Load<Texture2D>("Texture/laser");
			sineTexture = Content.Load<Texture2D>("sineBeam");

			explosionTexture = Content.Load<Texture2D>("Animation/explosion");

			// Load the music
			gameplayMusic = Content.Load<Song>("sound/gameMusic");

			// Load the laser and explosion sound effect
			laserSound = Content.Load<SoundEffect>("sound/laserFire");
			explosionSound = Content.Load<SoundEffect>("sound/explosion");

			// Start the music right away
			PlayMusic(gameplayMusic);

			// Load the score font
			//font = Content.Load("Font/gameFont");
			font = Content.Load<SpriteFont>("Font/gameFont");

			mainBackground = Content.Load<Texture2D>("Texture/mainbackground");

		}

		private void AddProjectile(Vector2 position)
		{
			Projectile projectile = new Projectile();
			projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
			projectiles.Add(projectile);
		}

		private void AddSineBeam(Vector2 position)
		{
			//SineBeam sineBeam = new SineBeam();
			//sineBeam.Initialize(GraphicsDevice.Viewport, sineTexture, position);
			//sineBeam.Add(sineBeam);		
			Animation sineAnimation = new Animation();
			sineAnimation.Initialize(sineTexture, Vector2.Zero, 56, 61, 8, 30, Color.White, 1f, false);

			SineBeam sineBeam = new SineBeam();
			sineBeam.Initialize(GraphicsDevice.Viewport, sineAnimation, position);

			sineBeams.Add(sineBeam);	
		}


		private void PlayMusic(Song song)
		{
			// Due to the way the MediaPlayer plays music,
			// we have to catch the exception. Music will play when the game is not tethered
			try
			{
				// Play the music
				MediaPlayer.Play(song);

				// Loop the currently playing song
				MediaPlayer.IsRepeating = true;
			}
			catch { }
		}


		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif

			// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);


			//Update the player
			UpdatePlayer(gameTime);

			// Update the parallaxing background
			bgLayer1.Update();
			bgLayer2.Update();

			// Update the enemies
			UpdateEnemies(gameTime);
			UpdateTieFighters(gameTime);

			// Update the collision
			UpdateCollision();

			// Update the projectiles
			UpdateProjectiles();

			UpdateSineBeams();

			// Update the explosions
			UpdateExplosions(gameTime);

			base.Update(gameTime);
		}

		private void UpdatePlayer(GameTime gameTime)
		{
			player.Update(gameTime);

			// Get Thumbstick Controls
			player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.Left) ||
			currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) ||
			currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) ||
			currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) ||
			currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}

			// Make sure that the player does not go out of bounds
			player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
			player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

			if (currentKeyboardState.IsKeyDown(Keys.Space))
			{
				// Fire only every interval we set as the fireTime
				if (gameTime.TotalGameTime - previousFireTime > fireTime)
				{
					// Reset our current time
					previousFireTime = gameTime.TotalGameTime;

					// Add the projectile, but add it to the front and center of the player
					AddProjectile(player.Position + new Vector2(player.Width / 2, 0));

					//Play the laser sound
					laserSound.Play();
				}
			}

			//if(currentKeyboardState.IsKeyDown(Keys.LeftShift))
			//{
				if (gameTime.TotalGameTime - previousFireTime > fireTime)
				{
					previousFireTime = gameTime.TotalGameTime;

					AddSineBeam(player.Position + new Vector2(player.Width / 2, 0));
					laserSound.Play();
				}
			//}

			// reset score if player health goes to zero
			if (player.Health <= 0)
			{
			    player.Health = 100;
			    score = 0;
			}
		}

		private void UpdateProjectiles()
		{
			// Update the Projectiles
			for (int i = projectiles.Count - 1; i >= 0; i--)
			{
				projectiles[i].Update();

				if (projectiles[i].Active == false)
				{
					projectiles.RemoveAt(i);
				}

			}
		}

		private void UpdateSineBeams()
		{
			// Update the Projectiles
			for (int i = sineBeams.Count - 1; i >= 0; i--)
			{
				sineBeams[i].Update();

				if (sineBeams[i].Active == false)
				{
					sineBeams.RemoveAt(i);
				}

			}
		}

		private void AddExplosion(Vector2 position)
		{
			Animation explosion = new Animation();
			explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
			explosions.Add(explosion);		
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			// Start drawing
			spriteBatch.Begin();

			spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

			// Draw the moving background
			bgLayer1.Draw(spriteBatch);
			bgLayer2.Draw(spriteBatch);

			// Draw the Enemies
			for (int i = 0; i<enemies.Count; i++)
			{
				enemies[i].Draw(spriteBatch);
			}

			// Draw the Projectiles
			for (int i = 0; i<projectiles.Count; i++)
			{
				projectiles[i].Draw(spriteBatch);
			}

			// Draw the Player
			player.Draw(spriteBatch);

			// Draw the explosions
			for (int i = 0; i<explosions.Count; i++)
			{
				explosions[i].Draw(spriteBatch);
			}

			// Draw the score
			spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
			// Draw the player health
			spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

			// Stop drawing
			spriteBatch.End();


			base.Draw(gameTime);
		}

		private void AddEnemy()
		{
			// Create the animation object
			Animation enemyAnimation = new Animation();

			// Initialize the animation with the correct animation information
			enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

			// Create an enemy
			Enemy enemy = new Enemy();

			// Initialize the enemy
			enemy.Initialize(enemyAnimation, position);

			// Add the enemy to the active enemies list
			enemies.Add(enemy);
		}
		private void AddTieFighter()
		{
			// Create the animation object
			Animation tieAnimation = new Animation();

			// Initialize the animation with the correct animation information
			tieAnimation.Initialize(tieTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + tieTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

			// Create an enemy
			TieFighter tieFighter = new TieFighter();

			// Initialize the enemy
			tieFighter.Initialize(tieAnimation, position);

			// Add the enemy to the active enemies list
			tieFighters.Add(tieFighter);
		}

		private void UpdateEnemies(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
			{
				previousSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddEnemy();
			}
			// Update the Enemies
			for (int i = enemies.Count - 1; i >= 0; i--)
			{
				enemies[i].Update(gameTime);

				if (enemies[i].Active == false)
				{
					// If not active and health <= 0
					if (enemies[i].Health <= 0)
					{
						// Add an explosion
						AddExplosion(enemies[i].Position);

						// Play the explosion sound
						explosionSound.Play();

						//Add to the player's score
						score += enemies[i].Value;
					}

					enemies.RemoveAt(i);
				}

			}
		}

		private void UpdateTieFighters(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if (gameTime.TotalGameTime - previousTieSpawnTime > tieSpawnTime)
			{
				previousTieSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddTieFighter();
			}
			// Update the Enemies
			for (int i = tieFighters.Count - 1; i >= 0; i--)
			{
				tieFighters[i].Update(gameTime);

				if (tieFighters[i].Active == false)
				{
					// If not active and health <= 0
					if (tieFighters[i].Health <= 0)
					{
						// Add an explosion
						AddExplosion(tieFighters[i].Position);

						// Play the explosion sound
						explosionSound.Play();

						//Add to the player's score
						score += tieFighters[i].Value;
					}

					tieFighters.RemoveAt(i);
				}

			}
		}

		private void UpdateExplosions(GameTime gameTime)
		{
			for (int i = explosions.Count - 1; i >= 0; i--)
			{
				explosions[i].Update(gameTime);
				if (explosions[i].Active == false)
				{
					explosions.RemoveAt(i);
				}
			}
		}

		private void UpdateCollision()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;
			Rectangle rectangle3;

			// Only create the rectangle once for the player
			rectangle1 = new Rectangle((int)player.Position.X,
			(int)player.Position.Y,
			player.Width,
			player.Height);

			// Do the collision between the player and the enemies
			for (int i = 0; i < enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X,
				(int)enemies[i].Position.Y,
				enemies[i].Width,
				enemies[i].Height);

				// Determine if the two objects collided with each
				// other
				if (rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
						player.Active = false;
				}

			}

			for (int i = 0; i<tieFighters.Count; i++)
			{
				rectangle3 = new Rectangle((int)tieFighters[i].Position.X,
				(int)tieFighters[i].Position.Y,
				tieFighters[i].Width,
				tieFighters[i].Height);

				if (rectangle1.Intersects(rectangle3))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= tieFighters[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					tieFighters[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
						player.Active = false;
				}


			}

			// Projectile vs Enemy Collision
			for (int i = 0; i<projectiles.Count; i++)
			{
				for (int j = 0; j<enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)projectiles[i].Position.X - 
					projectiles[i].Width / 2,(int)projectiles[i].Position.Y - 
					projectiles[i].Height / 2,projectiles[i].Width, projectiles[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
					(int)enemies[j].Position.Y - enemies[j].Height / 2,
					enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= projectiles[i].Damage;
						projectiles[i].Active = false;
					}
				}
			}

			for (int i = 0; i<projectiles.Count; i++)
			{
				for (int j = 0; j<tieFighters.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)projectiles[i].Position.X - 
					projectiles[i].Width / 2,(int)projectiles[i].Position.Y - 
					projectiles[i].Height / 2,projectiles[i].Width, projectiles[i].Height);

					rectangle3 = new Rectangle((int)tieFighters[j].Position.X - tieFighters[j].Width / 2,
					(int)tieFighters[j].Position.Y - tieFighters[j].Height / 2,
					tieFighters[j].Width, tieFighters[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle3))
					{
						tieFighters[j].Health -= projectiles[i].Damage;
						projectiles[i].Active = false;
					}
				}
			}
		}


	}
}
