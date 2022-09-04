using System.Collections.Generic;
using Interfaces;
using Modules;
using ScriptableObject.Sputnik;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sputniks
{
	public class BaseSputnik : Entity, IDamageable
	{
		[SerializeField] private ModuleCreator moduleCreator;
		private List<BaseModule> _modules = new List<BaseModule>();
		public Sputnik sputnik;

		public float Health { get; set; }

		public float speed;
		private Vector2 _positionPlanet;
		private float _radiusOrbitSputniks;
		private float _angleOnOrbitPlanet;

		public void Initialize(Vector2 positionPlanet, float radiusOrbitSputniks, bool newIsEnemy = false)
		{
			_positionPlanet = positionPlanet;
			_radiusOrbitSputniks = radiusOrbitSputniks;
			title = gameObject.name = $"{sputnik.title} #{Random.Range(0, 100000)}";
			speed = Random.Range(sputnik.speedMin, sputnik.speedMax) / 1000;
			Health = sputnik.health;
			isEnemy = newIsEnemy;
			
			var moduleLaserDrill = moduleCreator.CreateModule(ModuleType.LaserDrill, transform);
			moduleLaserDrill.Run();
			
			var moduleLaserGun = moduleCreator.CreateModule(ModuleType.LaserGun, transform);
			moduleLaserGun.Run();
			
			_modules.Add(moduleLaserDrill);
			_modules.Add(moduleLaserGun);
			
			transform.position = MoveOnOrbit();
		}

		private void FixedUpdate()
		{
			MoveOnOrbit();
		}

		private Vector2 MoveOnOrbit()
		{
			const int directionMovingOnOrbitPlanet = 1;

			var stepMove = speed * 15 * directionMovingOnOrbitPlanet;
			var radians = Mathf.PI * (_angleOnOrbitPlanet + stepMove) / 180.0f;
			var x = _positionPlanet.x + _radiusOrbitSputniks * Mathf.Cos(radians);
			var y = _positionPlanet.y + _radiusOrbitSputniks * Mathf.Sin(radians);
			var position= transform.position = new Vector2(x, y);
			_angleOnOrbitPlanet += stepMove;
			if (_angleOnOrbitPlanet > 360) _angleOnOrbitPlanet = 0;
			return position;
		}

		public List<BaseModule> GetModules() => _modules;
		
		public void ApplyDamage(float damageValue, ImpactType impactType)
		{
			print($"BaseSputnik: ApplyDamage() : newDamage: {damageValue}");
		}
	}
}
