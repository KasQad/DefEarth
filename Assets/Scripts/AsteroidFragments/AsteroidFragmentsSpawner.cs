using System.Collections.Generic;
using Planets;
using ScriptableObject.AsteroidFragments;
using Types;
using UnityEngine;

namespace AsteroidFragments
{
	public class AsteroidFragmentsSpawner : MonoBehaviour
	{
		private static AsteroidFragmentsSpawner _instance;

		public static AsteroidFragmentsSpawner Instance
		{
			get
			{
				if (_instance == null) _instance = FindObjectOfType<AsteroidFragmentsSpawner>();
				return _instance;
			}
		}

		private HashSet<BaseAsteroidFragment> _objectsPoolList;
		[SerializeField] private BaseAsteroidFragment _prefabAsteroidFragment;
		[SerializeField] private int _minPoolCapacity;
		[SerializeField] private int _maxPoolCapacity;


		[SerializeField] private int _poolCount;
		[SerializeField] private int _asteroidFragmentsListCount;
		[SerializeField] private int _asteroidCountOnOrbit;

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
			_objectsPoolList = new HashSet<BaseAsteroidFragment>();
			BaseAsteroidFragment temp;
			for (int i = 0; i < _minPoolCapacity; i++)
			{
				temp = Instantiate(_prefabAsteroidFragment, transform);
				temp.gameObject.SetActive(false);
				_objectsPoolList.Add(temp);
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F)) CreateRandomAsteroidFragments(_spawnAsteroidFragment.position, 50);
			if (Input.GetKeyDown(KeyCode.G)) CreateRandomAsteroidFragments(_spawnAsteroidFragment.position, 1);
			CheckEntryToOrbitCaptureGravity();

			_poolCount = _objectsPoolList.Count;
			_asteroidFragmentsListCount = _asteroidFragmentsList.Count;
			_asteroidCountOnOrbit = GetCountAsteroidFragmentsOnOrbits();
		}

		private BaseAsteroidFragment GetPooledObject()
		{
			foreach (var item in _objectsPoolList)
				if (!item.gameObject.activeInHierarchy)
				{
					item.gameObject.SetActive(true);
					return item;
				}

			return null;
		}

		private void ReturnObjectToPool(BaseAsteroidFragment poolObject)
		{
			poolObject.gameObject.SetActive(false);
		}

		public void CreateRandomAsteroidFragments(Vector2 spawnPosition, int countAsteroidFragments = -1)
		{
			var freeSlotsOnOrbit = GameConfig.AsteroidCountLimitOnOrbit - GetCountAsteroidFragmentsOnOrbits();

			var asteroidFragmentsCount = countAsteroidFragments != -1 ? countAsteroidFragments : Random.Range(0, 2);
			if (asteroidFragmentsCount == 0) return;
			for (var i = 0; i < asteroidFragmentsCount; i++)
			{
				var randomAngle = RandomAngle();
				var targetPosition = MathFunctions.GetXYCoordsOnBorderCircleByAngle(spawnPosition,
					GameConfig.RadiusDeactivationAsteroid, randomAngle);

				if (_asteroidFragmentsList.Count > GameConfig.AsteroidCountLimitOnScene) continue;

				var baseAsteroidFragment = CreateAsteroidFragment(spawnPosition, targetPosition);

				if (baseAsteroidFragment == null) return;

				freeSlotsOnOrbit--;

				if (GetCountAsteroidFragmentsOnOrbits() >= GameConfig.AsteroidCountLimitOnOrbit)
					baseAsteroidFragment.DestroyAsteroidFragment(ImpactType.LiveTimeLimit, 3f);
				else
				{
					if (!CheckAsteroidFragmentCrossingOrbitPlanets(baseAsteroidFragment))
						baseAsteroidFragment.DestroyAsteroidFragment(ImpactType.LiveTimeLimit, 3f);
					else if (freeSlotsOnOrbit <= 0)
						baseAsteroidFragment.DestroyAsteroidFragment(ImpactType.LiveTimeLimit, 3f);
				}
			}
		}


		private float RandomAngle()
		{
			var r = Random.Range(0, 4);
			return r <= 2 ? Random.Range(135f, 225f) : Random.Range(0f, 360f);
		}


		private BaseAsteroidFragment CreateAsteroidFragment(Vector2 spawnPosition, Vector2 targetPosition,
			bool isEnemy = true)
		{
			var pathPointsList = new List<Vector2> { spawnPosition, targetPosition };

			// var prefabAsteroidFragment = GetRandomAsteroidFragmentPrefab();
			// if (!prefabAsteroidFragment) return null;

			var pooledObject = GetPooledObject();
			if (pooledObject == null) return null;

			var baseAsteroidFragment = pooledObject; // Instantiate(prefabAsteroidFragment, transform);
			baseAsteroidFragment.Initialize(pathPointsList, isEnemy);
			_asteroidFragmentsList.Add(baseAsteroidFragment);
			return baseAsteroidFragment;
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

		private BaseAsteroidFragment GetRandomAsteroidFragmentPrefab()
		{
			if (_prefabAsteroidFragmentsList.Count == 0) return null;
			var index = Random.Range(0, _prefabAsteroidFragmentsList.Count);
			return _prefabAsteroidFragmentsList[index];
		}

		public void DestroyAsteroidFragment(BaseAsteroidFragment baseAsteroidFragment, float time = 0)
		{
			ReturnObjectToPool(baseAsteroidFragment);
			_asteroidFragmentsList.Remove(baseAsteroidFragment);
			// Destroy(baseAsteroidFragment.gameObject, time);
		}

		private bool CheckAsteroidFragmentCrossingOrbitPlanets(BaseAsteroidFragment baseAsteroidFragment)
		{
			var planetList = planetSpawner.GetPlanetList();
			foreach (var planetItem in planetList)
			{
				if (MathFunctions.LineCrossingCircle(
					    baseAsteroidFragment.GetPathPointsList()[0], baseAsteroidFragment.GetPathPointsList()[1],
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

					var pathPointsList = asteroidFragmentItem.GetPathPointsList();

					var asteroidFragmentPosition = asteroidFragmentItem.transform.position;
					var radiusOrbitSpaceFragments = planetItem.Value.radiusOrbitSpaceFragments;

					if (Mathf.Abs(Vector2.Distance(asteroidFragmentPosition, planetPosition)) >
					    radiusOrbitCaptureGravity ||
					    Mathf.Abs(Vector2.Distance(asteroidFragmentPosition, planetPosition)) <
					    radiusOrbitSpaceFragments)
						continue;

					// asteroidFragmentItem.SetActiveCollider(true);

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
						pathPointsList[0]);
					var distance2 = Vector2.Distance(pointOnOrbitSpaceFragments2,
						pathPointsList[0]);

					pathPointsList.Clear();
					pathPointsList.Add(asteroidFragmentPosition);

					int directionMovingOnOrbitPlanet;
					if (distance1 > distance2)
					{
						pathPointsList.Add(pointOnOrbitSpaceFragments1);
						directionMovingOnOrbitPlanet = 1;
						asteroidFragmentItem.InitializeMovingOnOrbitPlanet(planetItem.Key, planetPosition,
							radiusOrbitSpaceFragments, angleEntry1, directionMovingOnOrbitPlanet);
					}
					else
					{
						pathPointsList.Add(pointOnOrbitSpaceFragments2);
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
