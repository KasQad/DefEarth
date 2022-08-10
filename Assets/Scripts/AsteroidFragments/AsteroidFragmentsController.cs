using System;
using System.Collections.Generic;
using Planets;
using ScriptableObject.AsteroidFragments;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AsteroidFragments
{
	public class AsteroidFragmentsController : MonoBehaviour
	{
		[SerializeField] private Transform spawnAsteroidFragments;
		[SerializeField] private GameObject AsteroidFragmentsContainer;
		public AsteroidFragment asteroidFragment;
		private List<BaseAsteroidFragment> _asteroidFragmentsList = new List<BaseAsteroidFragment>();
		private readonly List<BaseAsteroidFragment> _prefabAsteroidFragmentsList = new List<BaseAsteroidFragment>();
		[SerializeField] private PlanetController _planetController;

		private void Awake()
		{
			foreach (var item in asteroidFragment.asteroidFragmentsPrefabs)
				_prefabAsteroidFragmentsList.Add(item.GetComponent<BaseAsteroidFragment>());

			print($"_prefabAsteroidFragmentsList.count: {_prefabAsteroidFragmentsList.Count}");
		}

		private void Start()
		{
			BaseAsteroidFragment.destroyAsteroidFragment += DestroyAsteroidFragment;
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.A))
				CreateRandomAsteroidFragments(spawnAsteroidFragments.position);
		}

		private void FixedUpdate()
		{
			CheckEntryToOrbitCaptureGravity();
		}

		public void CreateRandomAsteroidFragments(Vector2 position)
		{
			for (var i = 0; i < Random.Range(5, 20); i++)
			{
				const float radius = 10;
				var randomAngle = Random.Range(0f, 2 * Mathf.PI - float.Epsilon);
				var xy = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * radius;
				CreateAsteroidFragment(position, xy);
			}
		}

		private void CreateAsteroidFragment(Vector2 spawnPosition, Vector2 targetPosition, bool enemy = true)
		{
			if(_prefabAsteroidFragmentsList.Count == 0) return;

			List<Vector2> pathPointsList = new List<Vector2>();
			pathPointsList.Add(spawnPosition);
			pathPointsList.Add(targetPosition);

			var index = Random.Range(0, _prefabAsteroidFragmentsList.Count);
			var prefabAsteroidFragment = _prefabAsteroidFragmentsList[index];
			var asteroidFragmentPrefab = Instantiate(prefabAsteroidFragment, AsteroidFragmentsContainer.transform);
			asteroidFragmentPrefab.Initialize(pathPointsList, enemy);
			_asteroidFragmentsList.Add(asteroidFragmentPrefab);
		}


		private void DestroyAsteroidFragment(BaseAsteroidFragment baseAsteroidFragment)
		{
			_asteroidFragmentsList.Remove(baseAsteroidFragment);
			Destroy(baseAsteroidFragment.gameObject);
			print($"DestroyAsteroidFragment: {baseAsteroidFragment.title}");
		}

		private void CheckEntryToOrbitCaptureGravity()
		{
			var planetList = _planetController.GetPlanetList();
			if(planetList.Count == 0 || _asteroidFragmentsList.Count == 0) return;

			foreach (var planetItem in planetList)
			{
				var radiusOrbitCaptureGravity = planetItem.Value.radiusOrbitCaptureGravity;
				var planetPosition = planetItem.Value.GetPosition();

				foreach (var asteroidFragmentItem in _asteroidFragmentsList)
				{
					if(asteroidFragmentItem.isInOrbitPlanet != null) continue;

					var asteroidFragmentPosition = asteroidFragmentItem.transform.position;
					var radiusOrbitSpaceFragments = planetItem.Value.radiusOrbitSpaceFragments;

					if(Mathf.Abs(Vector2.Distance(asteroidFragmentPosition, planetPosition)) > radiusOrbitCaptureGravity ||
					   Mathf.Abs(Vector2.Distance(asteroidFragmentPosition, planetPosition)) < radiusOrbitSpaceFragments )
						continue;

					var angleEntryToOrbitCapture = Functions.GetAngleBetweenTwoLines(
						new Vector2(planetPosition.x + 100, planetPosition.y),
						asteroidFragmentPosition,
						new Vector2(planetPosition.x, planetPosition.y));

					if(planetPosition.y > asteroidFragmentPosition.y)
						angleEntryToOrbitCapture = 360 - angleEntryToOrbitCapture;


					var angleEntry1 = angleEntryToOrbitCapture - 315;
					var angleEntry2 = angleEntryToOrbitCapture - 45;

					Vector2 pointOnOrbitSpaceFragments1 = Functions.GetXYCoordsOnCircleByAngle(planetPosition,
						radiusOrbitSpaceFragments, angleEntry1);

					Vector2 pointOnOrbitSpaceFragments2 = Functions.GetXYCoordsOnCircleByAngle(planetPosition,
						radiusOrbitSpaceFragments, angleEntry2);

					var distance1 = Vector2.Distance(pointOnOrbitSpaceFragments1, asteroidFragmentItem.pathPointsList[0]);
					var distance2 = Vector2.Distance(pointOnOrbitSpaceFragments2, asteroidFragmentItem.pathPointsList[0]);

					asteroidFragmentItem.pathPointsList.Clear();
					asteroidFragmentItem.pathPointsList.Add(asteroidFragmentPosition);

					int directionMovingOnOrbitPlanet;
					if(distance1 > distance2)
					{
						asteroidFragmentItem.pathPointsList.Add(pointOnOrbitSpaceFragments1);
						directionMovingOnOrbitPlanet = 1;
						asteroidFragmentItem.InitializeMovingOnOrbitPlanet(planetItem.Key, planetPosition,
							radiusOrbitSpaceFragments, angleEntry1, directionMovingOnOrbitPlanet);
					}
					else
					{
						asteroidFragmentItem.pathPointsList.Add(pointOnOrbitSpaceFragments2);
						directionMovingOnOrbitPlanet = -1;
						asteroidFragmentItem.InitializeMovingOnOrbitPlanet(planetItem.Key, planetPosition,
							radiusOrbitSpaceFragments, angleEntry2, directionMovingOnOrbitPlanet);
					}

				}

			}
		}

	}
}
