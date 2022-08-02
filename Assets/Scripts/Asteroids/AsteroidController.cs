using System.Collections.Generic;
using Rockets;
using ScriptableObject.Asteroids;
using UnityEngine;

namespace Asteroids
{
	public class AsteroidController : MonoBehaviour
	{
		[SerializeField] private GameObject asteroidContainer;

		[SerializeField] private List<Transform> pathPointsList = new List<Transform>();

		private readonly Dictionary<Asteroid.Type, BaseAsteroid> _prefabAsteroidList =
			new Dictionary<Asteroid.Type, BaseAsteroid>();

		private readonly List<Entity> _asteroidsList = new List<Entity>();

		private void Awake()
		{
			_prefabAsteroidList.Add(Asteroid.Type.Small,
				Resources.Load<BaseAsteroid>("Prefabs/Asteroids/SmallAsteroid"));
			_prefabAsteroidList.Add(Asteroid.Type.Middle,
				Resources.Load<BaseAsteroid>("Prefabs/Asteroids/MiddleAsteroid"));
			_prefabAsteroidList.Add(Asteroid.Type.Big,
				Resources.Load<BaseAsteroid>("Prefabs/Asteroids/BigAsteroid"));
		}

		private void Start()
		{
			Entity.DestroyEntity += DestroyAsteroid;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.W))
			{
				TestStartAsteroids();
			}
		}

		private void TestStartAsteroids()
		{
			List<Vector2> tempPathPointsList = new List<Vector2>();
			foreach (var point in pathPointsList)
				tempPathPointsList.Add(point.position);

			CreateAsteroid(Asteroid.Type.Small, tempPathPointsList, true);
			CreateAsteroid(Asteroid.Type.Middle, tempPathPointsList, true);
			CreateAsteroid(Asteroid.Type.Big, tempPathPointsList, true);
		}

		private void CreateAsteroid(Asteroid.Type asteroidType, List<Vector2> newPathPointsList, bool enemy = true)
		{
			if (newPathPointsList.Count == 0) return;

			if (!_prefabAsteroidList.TryGetValue(asteroidType, out var prefabAsteroid)) return;

			var asteroid = Instantiate(prefabAsteroid, asteroidContainer.transform);
			asteroid.Initialize(newPathPointsList, enemy);
			_asteroidsList.Add(asteroid);
		}

		private void DestroyAsteroid(Entity entity)
		{
			_asteroidsList.Remove(entity);
			Destroy(entity.gameObject);
			// print($"_asteroidsList.Count: {_asteroidsList.Count}");
		}

	}
}
