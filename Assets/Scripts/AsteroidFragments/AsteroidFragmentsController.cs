using System.Collections.Generic;
using Asteroids;
using Planets;
using ScriptableObject.AsteroidFragments;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AsteroidFragments
{
	public class AsteroidFragmentsController : MonoBehaviour
	{
		[SerializeField] private GameObject asteroidFragmentsContainer;
		[SerializeField] private PlanetController planetController;
		public AsteroidFragment asteroidFragment;
		private readonly List<BaseAsteroidFragment> _asteroidFragmentsList = new List<BaseAsteroidFragment>();
		private readonly List<BaseAsteroidFragment> _prefabAsteroidFragmentsList = new List<BaseAsteroidFragment>();

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

		private void FixedUpdate()
		{
			CheckEntryToOrbitCaptureGravity();
		}

		private void CreateRandomAsteroidFragments(Vector2 spawnPosition)
		{
			var asteroidFragmentsCount = Random.Range(0, 2);
			if (asteroidFragmentsCount == 0) return;
			for (var i = 0; i < asteroidFragmentsCount; i++)
			{
				var randomAngle = Random.Range(0f, 360f);
				var targetPosition =
					MathFunctions.GetXYCoordsOnBorderCircleByAngle(spawnPosition, GameConfig.RadiusDeactivationAsteroid,
						randomAngle);
				var baseAsteroidFragment = CreateAsteroidFragment(spawnPosition, targetPosition);

				if (_asteroidFragmentsList.Count > GameConfig.AsteroidCountLimit ||
				    CheckAsteroidFragmentIntersectWithPlanets(baseAsteroidFragment))
					baseAsteroidFragment.Destroy(4f);
			}
		}

		private BaseAsteroidFragment CreateAsteroidFragment(Vector2 spawnPosition, Vector2 targetPosition,
			bool enemy = true)
		{
			if (_prefabAsteroidFragmentsList.Count == 0) return null;

			var pathPointsList = new List<Vector2> { spawnPosition, targetPosition };

			var index = Random.Range(0, _prefabAsteroidFragmentsList.Count);
			var prefabAsteroidFragment = _prefabAsteroidFragmentsList[index];
			var baseAsteroidFragment = Instantiate(prefabAsteroidFragment, asteroidFragmentsContainer.transform);
			baseAsteroidFragment.Initialize(pathPointsList, enemy);
			_asteroidFragmentsList.Add(baseAsteroidFragment);
			return baseAsteroidFragment;
		}

		private void DestroyAsteroidFragment(BaseAsteroidFragment baseAsteroidFragment, float time = 0)
		{
			_asteroidFragmentsList.Remove(baseAsteroidFragment);
			Destroy(baseAsteroidFragment.gameObject, time);
		}

		private bool CheckAsteroidFragmentIntersectWithPlanets(BaseAsteroidFragment baseAsteroidFragment)
		{
			var planetList = planetController.GetPlanetList();
			foreach (var planetItem in planetList)
			{
				if (!MathFunctions.LineCrossingCircle(
					    baseAsteroidFragment.pathPointsList[0], baseAsteroidFragment.pathPointsList[1],
					    planetItem.Value.GetPosition(), planetItem.Value.radiusOrbitCaptureGravity))
					return true;
			}
			return false;
		}

		private void CheckEntryToOrbitCaptureGravity()
		{
			var planetList = planetController.GetPlanetList();
			if (planetList.Count == 0 ||
			    _asteroidFragmentsList.Count == 0 ||
			    _asteroidFragmentsList.Count > GameConfig.AsteroidCountLimit) return;

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
