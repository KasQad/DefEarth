using System.Collections.Generic;
using AsteroidFragments;
using Rockets;
using UnityEngine;

namespace Asteroids
{
	public class AsteroidController : MonoBehaviour
	{
		[SerializeField] private GameObject asteroidContainer;

		[SerializeField] private List<Transform> pathPointsList = new List<Transform>();

		[SerializeField] private AsteroidFragmentsController _asteroidFragmentsController;

		private readonly Dictionary<AsteroidType, BaseAsteroid> _prefabAsteroidList =
			new Dictionary<AsteroidType, BaseAsteroid>();

		private readonly List<Entity> _asteroidsList = new List<Entity>();
		
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
			BaseAsteroid.destroyAsteroid += DestroyAsteroid;
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

		private void CreateAsteroid(AsteroidType asteroidType, List<Vector2> newPathPointsList, bool enemy)
		{
			if(newPathPointsList.Count == 0) return;

			if(!_prefabAsteroidList.TryGetValue(asteroidType, out var prefabAsteroid)) return;

			var asteroid = Instantiate(prefabAsteroid, asteroidContainer.transform);
			asteroid.Initialize(newPathPointsList, enemy);
			_asteroidsList.Add(asteroid);
		}

		private void DestroyAsteroid(Entity entity)
		{
			_asteroidsList.Remove(entity);
			_asteroidFragmentsController.CreateRandomAsteroidFragments(entity.gameObject.transform.position);
			Destroy(entity.gameObject, 0.1f);
			print($"DestroyAsteroid: {entity.title}");
		}

	}
}
