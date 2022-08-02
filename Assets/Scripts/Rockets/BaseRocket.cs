using System.Collections.Generic;
using Interfaces;
using ScriptableObject.Rockets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rockets
{
	public class BaseRocket : Entity, IDamageable
	{
		public float health;
		public float speed;
		public float damage;
		public Rocket rocket;
		public List<Vector2> pathPointsList;
		private int _currentPointIndex;
		
		public void Initialize(List<Vector2> newPathPointsList, bool enemy = false)
		{
			title = gameObject.name = $"{rocket.title} #{Random.Range(0, 100000)}";
			Rotate(gameObject, newPathPointsList[0]);
			gameObject.transform.position = newPathPointsList[0];
			speed = rocket.speed / 1000;
			health = rocket.health;
			damage = rocket.damage;
			pathPointsList = newPathPointsList;
			Enemy = enemy;
		}

		private void FixedUpdate()
		{
			Move();
		}

		private void Move()
		{
			if (_currentPointIndex >= pathPointsList.Count || pathPointsList.Count == 0)
			{
				Destroy();
				return;
			}

			var position = transform.position;

			Rotate(gameObject, pathPointsList[_currentPointIndex]);

			position = Vector3.MoveTowards(position, pathPointsList[_currentPointIndex], speed);
			gameObject.transform.position = position;

			if (position.Equals(pathPointsList[_currentPointIndex])) _currentPointIndex++;
		}

		private void Rotate(GameObject _gameObject, Vector2 vectorPoint)
		{
			var y = _gameObject.transform.position.y - vectorPoint.y;
			var x = _gameObject.transform.position.x - vectorPoint.x;
			var angel = Mathf.Atan2(y, x) / Mathf.PI * 180 + 90;
			_gameObject.transform.rotation = Quaternion.Euler(0, 0, angel);
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

		public void ApplyDamage(float applyDamage)
		{
			health -= applyDamage;
			print($"{title} applyDamage: {applyDamage}");
			if (health <= 0)
				Destroy();
		}


	}
}
