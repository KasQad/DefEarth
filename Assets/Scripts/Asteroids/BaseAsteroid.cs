using System;
using System.Collections.Generic;
using AsteroidFragments;
using Interfaces;
using Rockets;
using ScriptableObject.Asteroids;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
	public class BaseAsteroid : Entity, IDamageable
	{
		public float speed;
		public float speedRotate;
		public Asteroid asteroid;
		public int currentPointIndex;
		public List<Vector2> pathPointsList;


		public void Initialize(List<Vector2> newPathPointsList, bool isEnemy = true)
		{
			title = gameObject.name = $"{asteroid.title} #{Random.Range(0, 100000)}";
			gameObject.transform.position = newPathPointsList[0];
			speed = Random.Range(asteroid.speedMin, asteroid.speedMax) / 1000;
			speedRotate = Random.Range(asteroid.speedRotateMin, asteroid.speedRotateMax);
			health = asteroid.health;
			damage = asteroid.damage;
			pathPointsList = newPathPointsList;
			IsEnemy = isEnemy;
			entityType = EntityType.Asteroid;
		}

		private void FixedUpdate()
		{
			transform.Rotate(Vector3.forward * (Time.deltaTime * speedRotate));
			Move();
		}

		private void Move()
		{
			if(currentPointIndex >= pathPointsList.Count || pathPointsList.Count == 0)
			{
				Destroy();
				return;
			}

			var position = transform.position;
			position = Vector3.MoveTowards(position, pathPointsList[currentPointIndex], speed);
			gameObject.transform.position = position;

			if(position.Equals(pathPointsList[currentPointIndex]))
				currentPointIndex++;
		}


		public void ApplyDamage(Entity entity)
		{
			if(entity.IsEnemy == IsEnemy) return;

			AsteroidFragmentsController.createRandomFragmentsAsteroid?.Invoke(GetPosition());
			health -= entity.damage;
			// print($"{title} applyDamage: {entity.damage}");
			if(health <= 0)
				Destroy();
		}

		private void Destroy()
		{
			AsteroidController.destroyAsteroidAction?.Invoke(this);
		}
	}
}
