using System.Collections.Generic;
using Interfaces;
using Rockets;
using ScriptableObject.Asteroids;
using UnityEngine;

namespace Asteroids
{
	public class BaseAsteroid : Entity, IDamageable//, IMovable
	{
		public float health;
		public float speed;
		public float speedRotate;
		public float damage;
		public Asteroid asteroid;

		public int currentPointIndex;
		public List<Vector2> pathPointsList;

		public void Initialize(List<Vector2> newPathPointsList, bool enemy = true)
		{
			title = gameObject.name = $"{asteroid.title} #{Random.Range(0, 100000)}";
			gameObject.transform.position = newPathPointsList[0];
			speed = asteroid.speed / 1000;
			speedRotate = asteroid.speedRotate;
			health = asteroid.health;
			damage = asteroid.damage;
			pathPointsList = newPathPointsList;
			Enemy = enemy;
		}

		private void FixedUpdate()
		{
			transform.Rotate(Vector3.forward * (Time.deltaTime * speedRotate));
			Move();
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			if (col.transform.GetComponent<Entity>().Enemy == Enemy)
			{
				print($"OnCollisionEnter2D {title}: friendly object {col.transform.name}");
				return;
			}
			col.transform.GetComponent<IDamageable>()?.ApplyDamage(damage);
		}

		private void Move()
		{
			if (currentPointIndex >= pathPointsList.Count || pathPointsList.Count == 0) return;

			var position = transform.position;
			position = Vector3.MoveTowards(position, pathPointsList[currentPointIndex], speed);
			gameObject.transform.position = position;

			if (position.Equals(pathPointsList[currentPointIndex]))
				currentPointIndex++;
		}

		public void ApplyDamage(float applyDamage)
		{
			health -= applyDamage;
			print($"{title} applyDamage: {applyDamage}");
			if (health <= 0)
				Destroy();
		}
	}
}
