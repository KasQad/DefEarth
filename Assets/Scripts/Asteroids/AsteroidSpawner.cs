using System;
using System.Collections.Generic;
using Types;
using UnityEngine;

namespace Asteroids
{
	public class AsteroidSpawner : MonoBehaviour
	{
		private static AsteroidSpawner _instance;
		public static AsteroidSpawner Instance
		{
			get
			{
				if (_instance == null) _instance = FindObjectOfType<AsteroidSpawner>();
				return _instance;
			}
		}

		private readonly Dictionary<AsteroidType, BaseAsteroid> _prefabAsteroidList =
			new Dictionary<AsteroidType, BaseAsteroid>();

		private readonly HashSet<BaseAsteroid> _asteroidsList = new HashSet<BaseAsteroid>();

		private void Awake()
		{
			_prefabAsteroidList.Add(AsteroidType.Small,
				Resources.Load<BaseAsteroid>("Prefabs/Asteroids/SmallAsteroid"));
			_prefabAsteroidList.Add(AsteroidType.Middle,
				Resources.Load<BaseAsteroid>("Prefabs/Asteroids/MiddleAsteroid"));
			_prefabAsteroidList.Add(AsteroidType.Big,
				Resources.Load<BaseAsteroid>("Prefabs/Asteroids/BigAsteroid"));
		}

		public void CreateAsteroid(AsteroidType asteroidType, List<Vector2> newPathPointsList, bool enemy,
			int waveNumber)
		{
			if (newPathPointsList.Count == 0) return;
			if (!_prefabAsteroidList.TryGetValue(asteroidType, out var prefabAsteroid)) return;
			var asteroid = Instantiate(prefabAsteroid, transform);
			asteroid.Initialize(newPathPointsList, enemy, waveNumber);
			_asteroidsList.Add(asteroid);
			PlayerRocketLauncherController.Instance.AddEntityToEntitiesAimedDictionary(asteroid);
		}

		public void DestroyAsteroid(BaseAsteroid entity)
		{
			PlayerRocketLauncherController.Instance.DelEntityFromEntitiesAimedDictionary(entity);
			_asteroidsList.Remove(entity);
			Destroy(entity.gameObject);
		}

		public HashSet<BaseAsteroid> GetAsteroidsList() => _asteroidsList;
	}
}
