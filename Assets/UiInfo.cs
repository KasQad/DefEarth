using System;
using System.Collections;
using System.Collections.Generic;
using AsteroidFragments;
using Asteroids;
using Rockets;
using TMPro;
using UnityEngine;

public class UiInfo : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private AsteroidController _asteroidController;
	[SerializeField] private RocketController _rocketController;
	[SerializeField] private AsteroidFragmentsController _asteroidFragmentsController;


	private void FixedUpdate()
	{
		var asteroidsList = _asteroidController.GetAsteroidsList();
		var rocketsList = _rocketController.GetRocketsList();
		var asteroidFragmentsController = _asteroidFragmentsController.GetAsteroidFragmentsList();


		text.text = $"Asteroids: {asteroidsList?.Count} \n" +
		            $"Rockets: {rocketsList?.Count} \n " +
		            $"AsteroidFragments: {asteroidFragmentsController?.Count}";
	}
}
