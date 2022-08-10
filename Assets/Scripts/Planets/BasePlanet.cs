using Interfaces;
using Rockets;
using ScriptableObject.Planets;
using UnityEngine;

namespace Planets
{
	public abstract class BasePlanet : Entity, IDamageable
	{
		public Planet planet;
		public float radiusOrbitSpaceFragments;
		public float radiusOrbitSputniks;
		public float radiusOrbitCaptureGravity;
		public float speedRotate;

		public void Initialize()
		{
			title = gameObject.name = planet.title;
			entityType = EntityType.Planet;
			radiusOrbitSpaceFragments = planet.radiusOrbitSpaceFragments;
			radiusOrbitSputniks = planet.radiusOrbitSputniks;
			radiusOrbitCaptureGravity = planet.radiusOrbitCaptureGravity;
			damage = 1000000f;

			var drawLine = gameObject.AddComponent<LinesManager>();
			drawLine.DrawCircle(gameObject.transform, radiusOrbitSpaceFragments, 0.02f, Color.gray);
			drawLine.DrawCircle(gameObject.transform, radiusOrbitSputniks, 0.02f, Color.blue);
			drawLine.DrawCircle(gameObject.transform, radiusOrbitCaptureGravity, 0.02f, Color.yellow);
		}


		private void Update()
		{
			gameObject.transform.Rotate(Vector3.back * (Time.deltaTime * speedRotate));
		}

		public Vector2 GetPosition()
		{
			return transform.position;
		}

		public void ApplyDamage(Entity entity)
		{
			if (entity.IsEnemy==IsEnemy) return;
			
			if(entity.entityType == EntityType.Asteroid ||
			   entity.entityType == EntityType.AsteroidFragment ||
			   entity.entityType == EntityType.Rocket)
				print($"\"{title}\" take damage ({entity.damage}) from \"{entity.title}\"");
		}
	}
}
