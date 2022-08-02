using System.Collections.Generic;
using Rockets;
using ScriptableObject.AsteroidFragments;
using UnityEngine;

namespace AsteroidFragments
{
	public class BaseAsteroidFragment : Entity
	{
		public float health;
		public float speed;
		public float damage;
		public float speedRotate;
		public AsteroidFragment asteroidFragment;

		private List<Vector2> pathPointsList = new List<Vector2>();
		private int currentPointIndex;
		private bool _onOrbit;
		
		private void FixedUpdate()
		{
			gameObject.transform.Rotate(Vector3.back * speedRotate);
			Move();
		}
		public void Initialize(List<Vector2> newPathPointsList, float newSpeed,  bool enemy = true)
		{
			gameObject.name = $"{asteroidFragment.title} #{Random.Range(0, 100000)}";
			gameObject.transform.position = newPathPointsList[0];
			speed = newSpeed / 1000;
			speedRotate = asteroidFragment.speedRotate;
			health = asteroidFragment.health;
			damage = asteroidFragment.damage;
			pathPointsList = newPathPointsList;
			Enemy = enemy;
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

	}
}
