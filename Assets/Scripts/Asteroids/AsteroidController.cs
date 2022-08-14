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
		
		public static Action<Entity> destroyAsteroidAction;
		
		
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
			destroyAsteroidAction += DestroyAsteroid;
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.W))
			{
				TestStartAsteroids();
			}
		}

		private void TestStartAsteroids()
		{
			List<Vector2> tempPathPointsList = new List<Vector2>();
			foreach (var point in pathPointsList)
				tempPathPointsList.Add(point.position);

			CreateAsteroid(AsteroidType.Small, tempPathPointsList, true);
			CreateAsteroid(AsteroidType.Middle, tempPathPointsList, true);
			CreateAsteroid(AsteroidType.Big, tempPathPointsList, true);
		}

		public void CreateAsteroid(AsteroidType asteroidType, List<Vector2> newPathPointsList, bool enemy)
		{
			if(newPathPointsList.Count == 0) return;
			if(!_prefabAsteroidList.TryGetValue(asteroidType, out var prefabAsteroid)) return;
			var asteroid = Instantiate(prefabAsteroid, asteroidContainer.transform);
			asteroid.Initialize(newPathPointsList, enemy);
			_asteroidsList.Add(asteroid);
			PlayerRocketLauncherController.addEntityToHashSetAction?.Invoke(asteroid);
		}

		private void DestroyAsteroid(Entity entity)
		{
			PlayerRocketLauncherController.delEntityFromHashSetAction?.Invoke(entity);
			_asteroidsList.Remove(entity);
			Destroy(entity.gameObject);
		}

		public HashSet<Entity> GetAsteroidsList() => _asteroidsList;
	}
}
