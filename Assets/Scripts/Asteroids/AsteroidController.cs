using System;
using System.Collections.Generic;
using AsteroidFragments;
using JetBrains.Annotations;
using Rockets;
using UnityEngine;

namespace Asteroids
{
	public class AsteroidController : MonoBehaviour
	{
		[SerializeField] private GameObject asteroidContainer;
		[SerializeField] private List<Transform> pathPointsList = new List<Transform>();
		[SerializeField] private AsteroidFragmentsController asteroidFragmentsController;

		private readonly Dictionary<AsteroidType, BaseAsteroid> _prefabAsteroidList =
			new Dictionary<AsteroidType, BaseAsteroid>();

		private readonly HashSet<Entity> _asteroidsList = new HashSet<Entity>();
		
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
			float newSpeed = 0, float newSpeedRotate = 0)
		{
			if(newPathPointsList.Count == 0) return;
			if(!_prefabAsteroidList.TryGetValue(asteroidType, out var prefabAsteroid)) return;
			var asteroid = Instantiate(prefabAsteroid, asteroidContainer.transform);
			asteroid.Initialize(newPathPointsList, enemy, newSpeed, newSpeedRotate);
			_asteroidsList.Add(asteroid);
			addEntityToHashSetAction?.Invoke(asteroid);
		}

		private void DestroyAsteroid(Entity entity)
		{
			delEntityFromHashSetAction?.Invoke(entity);
			_asteroidsList.Remove(entity);
			Destroy(entity.gameObject);
		}

		public HashSet<Entity> GetAsteroidsList() => _asteroidsList;
	}
}
