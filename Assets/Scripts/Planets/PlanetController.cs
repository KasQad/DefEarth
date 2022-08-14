using System.Collections.Generic;
using UnityEngine;

namespace Planets
{
	public class PlanetController : MonoBehaviour
	{
		[SerializeField] private GameObject planetContainer;

		private readonly Dictionary<PlanetType, BasePlanet> _prefabPlanetList =
			new Dictionary<PlanetType, BasePlanet>();
		
		private readonly Dictionary<PlanetType, BasePlanet> _planetsList =
			new Dictionary<PlanetType, BasePlanet>();

		private void Awake()
		{
			_prefabPlanetList.Add(PlanetType.Earth, Resources.Load<BasePlanet>("Prefabs/Planets/Earth"));
			CreatePlanet(PlanetType.Earth);
		}

		private void CreatePlanet(PlanetType planetType)
		{
			if (!_prefabPlanetList.TryGetValue(planetType, out var prefabPlanet)) return;
			var planet = Instantiate(prefabPlanet, planetContainer.transform);
			planet.Initialize();
			_planetsList.Add(planetType, planet);
		}

		public BasePlanet GetPlanetByType(PlanetType planetType)
		{
			return !_planetsList.TryGetValue(planetType, out var entity) ? null : entity;
		}

		public Dictionary<PlanetType, BasePlanet> GetPlanetList()
		{
			return _planetsList;
		}
	}
}
