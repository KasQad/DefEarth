using System.Collections.Generic;
using Asteroids;
using CustomTimers;
using Planets;
using ScriptableObject.AsteroidFragments;
using UnityEngine;

namespace AsteroidFragments
{
	public class AsteroidFragmentsSpawner : MonoBehaviour
	{
		[SerializeField] private PlanetSpawner planetSpawner;
		public AsteroidFragment asteroidFragment;
		private readonly List<BaseAsteroidFragment> _asteroidFragmentsList = new List<BaseAsteroidFragment>();
		private readonly List<BaseAsteroidFragment> _prefabAsteroidFragmentsList = new List<BaseAsteroidFragment>();

		[SerializeField] private Transform _spawnAsteroidFragment;

		private void Awake()
		{
			foreach (var item in asteroidFragment.asteroidFragmentsPrefabs)
				_prefabAsteroidFragmentsList.Add(item.GetComponent<BaseAsteroidFragment>());
		}

		private void Start()
		{
			BaseAsteroidFragment.destroyAsteroidFragment += DestroyAsteroidFragment;
			BaseAsteroid.createRandomFragmentsAsteroid += CreateRandomAsteroidFragments;
		}

		private void OnDestroy()
		{
			BaseAsteroidFragment.destroyAsteroidFragment -= DestroyAsteroidFragment;
			BaseAsteroid.createRandomFragmentsAsteroid -= CreateRandomAsteroidFragments;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F)) CreateRandomAsteroidFragments(_spawnAsteroidFragment.position, 50);
			CheckEntryToOrbitCaptureGravity();
		}

		private void CreateRandomAsteroidFragments(Vector2 spawnPosition, int countAsteroidFragments = 0)
		{
			var freeSlotsOnOrbit = GameConfig.AsteroidCountLimitOnOrbit - GetCountAsteroidFragmentsOnOrbits();

			var asteroidFragmentsCount = countAsteroidFragments != 0 ? countAsteroidFragments : Random.Range(0, 2);
			if (asteroidFragmentsCount == 0) return;
			for (var i = 0; i < asteroidFragmentsCount; i++)
			{
				var randomAngle = Random.Range(0f, 360f);
				var targetPosition = MathFunctions.GetXYCoordsOnBorderCircleByAngle(spawnPosition,
					GameConfig.RadiusDeactivationAsteroid, randomAngle);

				if (_asteroidFragmentsList.Count > GameConfig.AsteroidCountLimitOnScene) continue;

				var baseAsteroidFragment = CreateAsteroidFragment(spawnPosition, targetPosition);

				freeSlotsOnOrbit--;

				if (GetCountAsteroidFragmentsOnOrbits() >= GameConfig.AsteroidCountLimitOnOrbit)
				{
					baseAsteroidFragment.Destroy(ImpactType.LiveTimeLimit, 3f);
				}
				else
				{
					if (!CheckAsteroidFragmentCrossingOrbitPlanets(baseAsteroidFragment))
					{
						baseAsteroidFragment.Destroy(ImpactType.LiveTimeLimit, 3f);
					}
					else
					{
						if (freeSlotsOnOrbit <= 0) baseAsteroidFragment.Destroy(ImpactType.LiveTimeLimit, 3f);
					}
				}
			}
		}


		private int GetCountAsteroidFragmentsOnOrbits()
		{
			if (_asteroidFragmentsList.Count == 0) return 0;
			var i = 0;
			foreach (var baseAsteroidFragment in _asteroidFragmentsList)
				if (baseAsteroidFragment.CheckIsInOrbitPlanet() != null)
					i++;
			return i;
		}

		private BaseAsteroidFragment CreateAsteroidFragment(Vector2 spawnPosition, Vector2 targetPosition,
			bool enemy = true)
		{
			if (_prefabAsteroidFragmentsList.Count == 0) return null;

			var pathPointsList = new List<Vector2> { spawnPosition, targetPosition };

			var index = Random.Range(0, _prefabAsteroidFragmentsList.Count);
			var prefabAsteroidFragment = _prefabAsteroidFragmentsList[index];
			var baseAsteroidFragment = Instantiate(prefabAsteroidFragment, transform);
			baseAsteroidFragment.Initialize(pathPointsList, enemy);
			_asteroidFragmentsList.Add(baseAsteroidFragment);
			return baseAsteroidFragment;
		}

		private void DestroyAsteroidFragment(BaseAsteroidFragment baseAsteroidFragment, float time = 0)
		{
			_asteroidFragmentsList.Remove(baseAsteroidFragment);
			Destroy(baseAsteroidFragment.gameObject, time);
		}

		private bool CheckAsteroidFragmentCrossingOrbitPlanets(BaseAsteroidFragment baseAsteroidFragment)
		{
			var planetList = planetSpawner.GetPlanetList();
			foreach (var planetItem in planetList)
			{
				if (MathFunctions.LineCrossingCircle(
					    baseAsteroidFragment.pathPointsList[0], baseAsteroidFragment.pathPointsList[1],
					    planetItem.Value.GetPosition(), planetItem.Value.radiusOrbitCaptureGravity))
					return true;
			}
			return false;
		}

		private void CheckEntryToOrbitCaptureGravity()
		{
			var planetList = planetSpawner.GetPlanetList();
			if (planetList.Count == 0) return;
			if (_asteroidFragmentsList.Count == 0) return;
			if (GetCountAsteroidFragmentsOnOrbits() > GameConfig.AsteroidCountLimitOnOrbit) return;
			foreach (var planetItem in planetList)
			{
				var radiusOrbitCaptureGravity = planetItem.Value.radiusOrbitCaptureGravity;
				var planetPosition = planetItem.Value.GetPosition();

				foreach (var asteroidFragmentItem in _asteroidFragmentsList)
				{
					if (asteroidFragmentItem.isInOrbitPlanet != null) continue;

					var asteroidFragmentPosition = asteroidFragmentItem.transform.position;
					var radiusOrbitSpaceFragments = planetItem.Value.radiusOrbitSpaceFragments;

					if (Mathf.Abs(Vector2.Distance(asteroidFragmentPosition, planetPosition)) >
					    radiusOrbitCaptureGravity ||
					    Mathf.Abs(Vector2.Distance(asteroidFragmentPosition, planetPosition)) <
					    radiusOrbitSpaceFragments)
						continue;

					asteroidFragmentItem.SetActiveCollider(true);

					var angleEntryToOrbitCapture = MathFunctions.GetAngleBetweenTwoLines(
						new Vector2(planetPosition.x + 100, planetPosition.y),
						asteroidFragmentPosition,
						new Vector2(planetPosition.x, planetPosition.y));

					if (planetPosition.y > asteroidFragmentPosition.y)
						angleEntryToOrbitCapture = 360 - angleEntryToOrbitCapture;

					var angleEntry1 = angleEntryToOrbitCapture - 325;
					var angleEntry2 = angleEntryToOrbitCapture - 55;

					Vector2 pointOnOrbitSpaceFragments1 = MathFunctions.GetXYCoordsOnBorderCircleByAngle(planetPosition,
						radiusOrbitSpaceFragments, angleEntry1);

					Vector2 pointOnOrbitSpaceFragments2 = MathFunctions.GetXYCoordsOnBorderCircleByAngle(planetPosition,
						radiusOrbitSpaceFragments, angleEntry2);

					var distance1 = Vector2.Distance(pointOnOrbitSpaceFragments1,
						asteroidFragmentItem.pathPointsList[0]);
					var distance2 = Vector2.Distance(pointOnOrbitSpaceFragments2,
						asteroidFragmentItem.pathPointsList[0]);

					asteroidFragmentItem.pathPointsList.Clear();
					asteroidFragmentItem.pathPointsList.Add(asteroidFragmentPosition);

					int directionMovingOnOrbitPlanet;
					if (distance1 > distance2)
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

		public List<BaseAsteroidFragment> GetAsteroidFragmentsList() => _asteroidFragmentsList;
	}
}
