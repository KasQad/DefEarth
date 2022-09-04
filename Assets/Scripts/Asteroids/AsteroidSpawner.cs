using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids
{
	public class AsteroidSpawner : MonoBehaviour
	{
		private readonly Dictionary<AsteroidType, BaseAsteroid> _prefabAsteroidList =
			new Dictionary<AsteroidType, BaseAsteroid>();

		private readonly HashSet<BaseAsteroid> _asteroidsList = new HashSet<BaseAsteroid>();
		
		public static Action<Entity> addEntityToHashSetAction;
		public static Action<Entity> delEntityFromHashSetAction;
		
		
		private void Awake()
		{
			_prefabAsteroidList.Add(AsteroidType.Small,
				Resources.Load<BaseAsteroid>("Prefabs/Asteroids/SmallAsteroid"));
			_prefabAsteroidList.Add(AsteroidType.Middle,
				Resources.Load<BaseAsteroid>("Prefabs/Asteroids/MiddleAsteroid"));
			_prefabAsteroidList.Add(AsteroidType.Big,
				Resources.Load<BaseAsteroid>("Prefabs/Asteroids/BigAsteroid"));
		}

		private void Start()
		{
			BaseAsteroid.destroyAsteroidAction += DestroyAsteroid;
		}

		public void CreateAsteroid(AsteroidType asteroidType, List<Vector2> newPathPointsList, bool enemy,
			float newSpeed = 0)
		{
			if(newPathPointsList.Count == 0) return;
			if(!_prefabAsteroidList.TryGetValue(asteroidType, out var prefabAsteroid)) return;
			var asteroid = Instantiate(prefabAsteroid, transform);
			asteroid.Initialize(newPathPointsList, enemy, newSpeed);
			_asteroidsList.Add(asteroid);
			addEntityToHashSetAction?.Invoke(asteroid);
		}

		private void DestroyAsteroid(BaseAsteroid entity)
		{
			delEntityFromHashSetAction?.Invoke(entity);
			_asteroidsList.Remove(entity);
			Destroy(entity.gameObject);
		}

		public HashSet<BaseAsteroid> GetAsteroidsList() => _asteroidsList;
	}
}
