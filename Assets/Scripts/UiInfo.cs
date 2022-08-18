using AsteroidFragments;
using Asteroids;
using Rockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiInfo : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private AsteroidController asteroidController;
	[SerializeField] private RocketController rocketController;
	[SerializeField] private AsteroidFragmentsController asteroidFragmentsController;
	[SerializeField] private WaveEnemyController waveEnemyController;

	private void FixedUpdate()
	{
		var asteroidsList = asteroidController.GetAsteroidsList();
		var rocketsList = rocketController.GetRocketsList();
		var asteroidFragmentsControllerList = asteroidFragmentsController.GetAsteroidFragmentsList();


		text.text = $"Pause: {GameConfig.GamePaused}\n" +
		            
		            $"Wave number: {waveEnemyController.GetWaveNumber()} | " +
		            $"Wave time: {waveEnemyController.GetWaveLeftTine():0.#}\n" +
		            
		            $"Asteroids: {asteroidsList?.Count} \n" +
		            $"Rockets: {rocketsList?.Count} \n" +
		            $"AsteroidFragments: {asteroidFragmentsControllerList?.Count}";
	}
}
