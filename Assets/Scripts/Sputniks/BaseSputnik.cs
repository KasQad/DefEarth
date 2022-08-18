using ScriptableObject.Sputnik;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sputniks
{
	public class BaseSputnik : Entity
	{
		public Sputnik sputnik;
		public float speed;
		
		private PlanetType? _isInOrbitPlanet;
		private Vector2 _positionPlanet;		
		private float _angleOnOrbitPlanet;

		
		public void Initialize(PlanetType newIsInOrbitPlanet)
		{
			title = gameObject.name = $"{sputnik.title} #{Random.Range(0, 100000)}";
			speed = sputnik.speed / 1000;
			health = sputnik.health;
			entityType = EntityType.Sputnik;
			IsEnemy = false;
			_isInOrbitPlanet = newIsInOrbitPlanet;
		}

		private void FixedUpdate()
		{
			Move();
		}

		private void Move()
		{

		}
		
		private void MoveOnOrbit()
		{
			// const int directionMovingOnOrbitPlanet=1;
			//
			// var stepMove = speed * 15 * directionMovingOnOrbitPlanet;
			// var radians = Mathf.PI * (_angleOnOrbitPlanet + stepMove) / 180.0f;
			// var x = _positionPlanet.x + _radiusOrbitSpaceFragments * Mathf.Cos(radians);
			// var y = _positionPlanet.y + _radiusOrbitSpaceFragments * Mathf.Sin(radians);
			// transform.position = new Vector2(x, y);
			// _angleOnOrbitPlanet += stepMove;
			// if(_angleOnOrbitPlanet > 360) _angleOnOrbitPlanet = 0;
		}
	}
	
}
