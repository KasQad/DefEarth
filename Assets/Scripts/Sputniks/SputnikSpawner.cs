using System.Collections.Generic;
using Planets;
using UnityEngine;

namespace Sputniks
{
	public class SputnikSpawner : MonoBehaviour
	{
		[SerializeField] private PlanetSpawner planetSpawner;

		private readonly Dictionary<SputnikType, BaseSputnik> _prefabSputnikList =
			new Dictionary<SputnikType, BaseSputnik>();

		private readonly HashSet<Entity> _sputniksList = new HashSet<Entity>();


		private void Awake()
		{
			_prefabSputnikList.Add(SputnikType.Sputnik, Resources.Load<BaseSputnik>("Prefabs/Sputniks/Sputnik"));
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.S)) CreateSputnik();
			if (Input.GetKeyDown(KeyCode.R)) RunAllModules();
		}

		private void RunAllModules()
		{
			print($"RunAllModules()");
			if (_sputniksList.Count == 0) return;
			foreach (var entity in _sputniksList)
			{
				var sputnik = (BaseSputnik)entity;
				var modules = sputnik.GetModules();
				if (modules.Count == 0) continue;
				foreach (var module in modules)
				{
					module.Use();
				}
			}
		}

		public void CreateSputnik(SputnikType sputnikType=SputnikType.Sputnik, bool enemy = false)
		{
			if (!_prefabSputnikList.TryGetValue(sputnikType, out var prefBaseSputnik)) return;
			var sputnik = Instantiate(prefBaseSputnik, transform);

			const PlanetType planetType = PlanetType.Earth;
			var positionPlanet = planetSpawner.GetPlanetByType(planetType).GetPosition();
			var radiusOrbitSputniks = planetSpawner.GetPlanetByType(planetType).radiusOrbitSputniks;

			sputnik.Initialize(positionPlanet, radiusOrbitSputniks, enemy);
			_sputniksList.Add(sputnik);
		}

		public HashSet<Entity> GetSputniksList() => _sputniksList;
	}
}
