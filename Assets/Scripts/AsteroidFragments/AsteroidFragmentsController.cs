using System;
using System.Collections.Generic;
using Rockets;
using ScriptableObject.AsteroidFragments;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AsteroidFragments
{
	public class AsteroidFragmentsController : MonoBehaviour
	{
		[SerializeField] private Transform spawnAsteroidFragments;
		[SerializeField] private Transform targetAsteroidFragments;

		[SerializeField] private GameObject AsteroidFragmentsContainer;

		public AsteroidFragment asteroidFragment;

		private readonly List<Entity> _asteroidFragmentsList = new List<Entity>();

		private List<BaseAsteroidFragment> _prefabAsteroidFragmentsList = new List<BaseAsteroidFragment>();


		private void Awake()
		{
			foreach (var item in asteroidFragment.asteroidFragmentsPrefabs)
				_prefabAsteroidFragmentsList.Add(item.GetComponent<BaseAsteroidFragment>());

			print($"_prefabAsteroidFragmentsList.count: {_prefabAsteroidFragmentsList.Count}");
		}

		private void Start()
		{
			CreateRandomAsteroidFragments();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.A)) CreateRandomAsteroidFragments();
		}

		private void CreateRandomAsteroidFragments()
		{
			for (int i = 0; i < Random.Range(5, 20); i++)
			{
				var y = Random.Range(-30, 30);
				var x = Random.Range(-30, 30);
				CreateAsteroidFragment(spawnAsteroidFragments.position, new Vector2(x, y));
			}
		}

		private void CreateAsteroidFragment(Vector2 spawnPosition, Vector2 targetPosition, bool enemy = true)
		{
			if (_prefabAsteroidFragmentsList.Count == 0) return;

			List<Vector2> pathPointsList = new List<Vector2>();
			pathPointsList.Add(spawnPosition);
			pathPointsList.Add(targetPosition);

			int index = Random.Range(0, _prefabAsteroidFragmentsList.Count);

			var prefabAsteroidFragment = _prefabAsteroidFragmentsList[index];

			var asteroidFragmentPrefab = Instantiate(prefabAsteroidFragment, AsteroidFragmentsContainer.transform);

			asteroidFragmentPrefab.Initialize(pathPointsList, Random.Range(10,30), enemy);

			_asteroidFragmentsList.Add(asteroidFragmentPrefab);
		}

	}
}
