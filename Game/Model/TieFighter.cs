using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShooterGame.View;
namespace ShooterGame.Model
{
	public class TieFighter
	{

		// Animation representing the enemy
		private Animation tieAnimation;
		public Animation TieAnimation
		{
		get { return tieAnimation; }
		set { tieAnimation = value; }
		}

		// The position of the enemy ship relative to the top left corner of thescreen
		public Vector2 Position;

		// The state of the Enemy Ship
		private bool active;
		public bool Active
		{
		get { return active; }
		set { active = value; }
		}

		// The hit points of the enemy, if this goes to zero the enemy dies
		private int health;
		public int Health
		{
		get { return health; }
		set { health = value; }
		}

		// The amount of damage the enemy inflicts on the player ship
		private int damage;
		public int Damage
		{
		get { return damage; }
		set { damage = value; }
		}

		// The amount of score the enemy will give to the player
		public int Value;

		// Get the width of the enemy ship
		public int Width
		{
		get { return TieAnimation.FrameWidth; }
		}

		// Get the height of the enemy ship
		public int Height
		{
		get { return TieAnimation.FrameHeight; }
		}

		// The speed at which the enemy moves
		float tieMoveSpeed;

		public void Initialize(Animation animation, Vector2 position)
		{
		// Load the enemy ship texture
		TieAnimation = animation;

		// Set the position of the enemy
		Position = position;

		// We initialize the enemy to be active so it will be update in the game
		active = true;


		// Set the health of the enemy
		health = 10;

		// Set the amount of damage the enemy can do
		damage = 10;

		// Set how fast the enemy moves
		tieMoveSpeed = 6f;


		// Set the score value of the enemy
		Value = 100;

		}


		public void Update(GameTime gameTime)
		{
		// The enemy always moves to the left so decrement it's xposition
		Position.X -= tieMoveSpeed;

		// Update the position of the Animation
		TieAnimation.Position = Position;

		// Update Animation
		TieAnimation.Update(gameTime);

		// If the enemy is past the screen or its health reaches 0 then deactivateit
		if (Position.X < -Width || Health <= 0)
		{
		// By setting the Active flag to false, the game will remove this objet fromthe
		// active game list
		Active = false;
		}
		}


		public void Draw(SpriteBatch spriteBatch)
		{
			// Draw the animation
			TieAnimation.Draw(spriteBatch);
		}
		//public tieFighter()
		//{
		//}
	}
}
