using AsteroidFragments;
using Asteroids;
using Rockets;
using TMPro;
using UnityEngine;

namespace Ui
{
	public class UiInfo : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI text;
		[SerializeField] private AsteroidController asteroidController;
		[SerializeField] private RocketController rocketController;
		[SerializeField] private AsteroidFragmentsController asteroidFragmentsController;
		[SerializeField] private WaveEnemyController waveEnemyController;

		private const float UpdateInterval = 0.5F;
		private double _lastInterval;
		private int _frames;
		private float _fps;

		private void Update()
		{
			Fps();
		}

		private void FixedUpdate()
		{
			var asteroidsList = asteroidController.GetAsteroidsList();
			var rocketsList = rocketController.GetRocketsList();
			var asteroidFragmentsControllerList = asteroidFragmentsController.GetAsteroidFragmentsList();

			text.text = $"Pause: {GameConfig.GamePaused} | FPS: {_fps:N1}\n" +

			            $"Wave number: {GameConfig.GetWaveNumber()} | " +
			            $"Wave time: {waveEnemyController.GetWaveTimeLeft():N1}\n" +

			            $"Asteroids: {asteroidsList?.Count} \n" +
			            $"Rockets: {rocketsList?.Count} \n" +
			            $"AsteroidFragments: {asteroidFragmentsControllerList?.Count}";
		}

		private void Fps()
		{
			++_frames;
			var timeNow = Time.realtimeSinceStartup;
			if (!(timeNow > _lastInterval + UpdateInterval)) return;
			_fps = (float)(_frames / (timeNow - _lastInterval));
			_frames = 0;
			_lastInterval = timeNow;
		}
	}
}
