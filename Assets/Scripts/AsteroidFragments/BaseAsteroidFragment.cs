using System;
using System.Collections.Generic;
using Interfaces;
using Rockets;
using ScriptableObject.AsteroidFragments;
using ScriptableObject.Planets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AsteroidFragments
{
	public class BaseAsteroidFragment : Entity, IDamageable
	{
		public AsteroidFragment asteroidFragment;
		public float health;
		public float speed;
		public float speedRotate;

		public List<Vector2> pathPointsList = new List<Vector2>();
		private int _currentPointIndex;

		public PlanetType? isInOrbitPlanet = null;
		private float _angleOnOrbitPlanet;
		private Vector2 _positionPlanet;
		private float _radiusOrbitSpaceFragments;
		private int _directionMovingOnOrbitPlanet;

		public static Action<BaseAsteroidFragment> destroyAsteroidFragment;

		private void FixedUpdate()
		{
			gameObject.transform.Rotate(Vector3.back * speedRotate);
			Move();
		}

		public void Initialize(List<Vector2> newPathPointsList, bool enemy = true)
		{
			title = gameObject.name = $"{asteroidFragment.title} #{Random.Range(0, 100000)}";
			gameObject.transform.position = newPathPointsList[0];
			speed = Random.Range(asteroidFragment.speedMin, asteroidFragment.speedMax) / 1000;
			speedRotate = Random.Range(asteroidFragment.speedRotateMin, asteroidFragment.speedRotateMax);
			health = asteroidFragment.health;
			damage = asteroidFragment.damage;
			pathPointsList = newPathPointsList;
			IsEnemy = enemy;
			entityType = EntityType.AsteroidFragment;
		}

		public void InitializeMovingOnOrbitPlanet(PlanetType planetType, Vector2 positionPlanet,
			float radiusOrbitSpaceFragments, float angleOnOrbitPlanet, int directionMovingOnOrbitPlanet)
		{
			isInOrbitPlanet = planetType;
			_angleOnOrbitPlanet = angleOnOrbitPlanet;
			_positionPlanet = positionPlanet;
			_radiusOrbitSpaceFragments = radiusOrbitSpaceFragments;
			_directionMovingOnOrbitPlanet = directionMovingOnOrbitPlanet;
		}


		private void Move()
		{
			if(pathPointsList.Count == 0) return;
			if(_currentPointIndex >= pathPointsList.Count)
			{
				if(isInOrbitPlanet != null)
				{
					MoveOnOrbit();
					return;
				}

				Destroy();
				return;
			}

			var position = transform.position;
			position = Vector3.MoveTowards(position, pathPointsList[_currentPointIndex], speed);
			gameObject.transform.position = position;

			if(position.Equals(pathPointsList[_currentPointIndex]))
				_currentPointIndex++;
		}

		private void MoveOnOrbit()
		{
			var stepMove = speed * 15 * _directionMovingOnOrbitPlanet;
			var radians = Mathf.PI * (_angleOnOrbitPlanet + stepMove) / 180.0f;
			var x = _positionPlanet.x + _radiusOrbitSpaceFragments * Mathf.Cos(radians);
			var y = _positionPlanet.y + _radiusOrbitSpaceFragments * Mathf.Sin(radians);
			transform.position = new Vector2(x, y);
			_angleOnOrbitPlanet += stepMove;
			if(_angleOnOrbitPlanet > 360) _angleOnOrbitPlanet = 0;
		}


		public void ApplyDamage(Entity entity)
		{
			if(entity.IsEnemy == IsEnemy) return;
			if(isInOrbitPlanet == null) return;
			
			health -= entity.damage;
			print($"{title} applyDamage: {entity.damage}");
			if(health <= 0)
				Destroy();
		}

		private void Destroy()
		{
			destroyAsteroidFragment?.Invoke(this);
		}
	}
}
