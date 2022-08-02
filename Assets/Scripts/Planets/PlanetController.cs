using System.Collections.Generic;
using Rockets;
using ScriptableObject.Planets;
using UnityEngine;

namespace Planets
{
	public class PlanetController : MonoBehaviour
	{
		[SerializeField] private GameObject planetContainer;


		private readonly Dictionary<Planet.Type, BasePlanet> _prefabPlanetList =
			new Dictionary<Planet.Type, BasePlanet>();

		private readonly List<Entity> planetsList = new List<Entity>();

		private void Awake()
		{
			_prefabPlanetList.Add(Planet.Type.Earth, Resources.Load<BasePlanet>("Prefabs/Planets/Earth"));
		}


		private void Start()
		{
			CreatePlanet(Planet.Type.Earth);
		}

		private void CreatePlanet(Planet.Type planetType)
		{
			if (!_prefabPlanetList.TryGetValue(planetType, out var prefabPlanet)) return;

			var planet = Instantiate(prefabPlanet, planetContainer.transform);

			planet.Initialize();

			planetsList.Add(planet);
		}

	}
}
