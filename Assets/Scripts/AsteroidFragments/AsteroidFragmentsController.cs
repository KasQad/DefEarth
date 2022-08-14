﻿using System;
using System.Collections.Generic;
using Planets;
using Rockets;
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

		public static Action<Vector2> createRandomFragmentsAsteroid;

		
		private const float RadiusDeactivationAsteroid = 20;

		private void Awake()
		{
			foreach (var item in asteroidFragment.asteroidFragmentsPrefabs)
				_prefabAsteroidFragmentsList.Add(item.GetComponent<BaseAsteroidFragment>());
		}

		private void Start()
		{
			BaseAsteroidFragment.destroyAsteroidFragment += DestroyAsteroidFragment;
			createRandomFragmentsAsteroid += CreateRandomAsteroidFragments;

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

		private void CreateRandomAsteroidFragments(Vector2 spawnPosition)
		{
			var asteroidFragmentsCount = Random.Range(1, 3);
			if(asteroidFragmentsCount == 0) return;
			for (var i = 0; i < asteroidFragmentsCount; i++)
			{
				var randomAngle = Random.Range(0f, 360f);
				var targetPosition = MathFunctions.GetXYCoordsOnBorderCircleByAngle(spawnPosition, RadiusDeactivationAsteroid, randomAngle);
				var baseAsteroidFragment = CreateAsteroidFragment(spawnPosition, targetPosition);

				DestroyAsteroidFragmentNotIntersectWithPlanets(baseAsteroidFragment);
			}
		}

		private BaseAsteroidFragment CreateAsteroidFragment(Vector2 spawnPosition, Vector2 targetPosition, bool enemy = true)
		{
			if(_prefabAsteroidFragmentsList.Count == 0) return null;

			var pathPointsList = new List<Vector2> { spawnPosition, targetPosition };

			var index = Random.Range(0, _prefabAsteroidFragmentsList.Count);
			var prefabAsteroidFragment = _prefabAsteroidFragmentsList[index];
			var baseAsteroidFragment = Instantiate(prefabAsteroidFragment, AsteroidFragmentsContainer.transform);
			baseAsteroidFragment.Initialize(pathPointsList, enemy);
			_asteroidFragmentsList.Add(baseAsteroidFragment);
			return baseAsteroidFragment;
		}


		private void DestroyAsteroidFragment(BaseAsteroidFragment baseAsteroidFragment, float time = 0)
		{
			_asteroidFragmentsList.Remove(baseAsteroidFragment);
			Destroy(baseAsteroidFragment.gameObject, time);
		}

		private void DestroyAsteroidFragmentNotIntersectWithPlanets(BaseAsteroidFragment baseAsteroidFragment)
		{
			var planetList = _planetController.GetPlanetList();
			foreach (var planetItem in planetList)
			{
				if(!MathFunctions.CircleLineIntersect(
					   baseAsteroidFragment.pathPointsList[0], baseAsteroidFragment.pathPointsList[1], 
					   planetItem.Value.GetPosition(), planetItem.Value.radiusOrbitCaptureGravity))
				{
					baseAsteroidFragment.Destroy(2f);
				}
			}
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

					if(Mathf.Abs(Vector2.Distance(asteroidFragmentPosition, planetPosition)) >
					   radiusOrbitCaptureGravity ||
					   Mathf.Abs(Vector2.Distance(asteroidFragmentPosition, planetPosition)) <
					   radiusOrbitSpaceFragments)
						continue;

					asteroidFragmentItem.collider2D.enabled = true;
					// print($"asteroidFragmentItem.title: {asteroidFragmentItem.title}");

					var angleEntryToOrbitCapture = MathFunctions.GetAngleBetweenTwoLines(
						new Vector2(planetPosition.x + 100, planetPosition.y),
						asteroidFragmentPosition,
						new Vector2(planetPosition.x, planetPosition.y));

					if(planetPosition.y > asteroidFragmentPosition.y)
						angleEntryToOrbitCapture = 360 - angleEntryToOrbitCapture;

					var angleEntry1 = angleEntryToOrbitCapture - 315;
					var angleEntry2 = angleEntryToOrbitCapture - 45;

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
		
		public List<BaseAsteroidFragment> GetAsteroidFragmentsList() => _asteroidFragmentsList;

	}
}
