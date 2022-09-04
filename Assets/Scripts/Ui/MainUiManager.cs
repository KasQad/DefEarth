using AsteroidFragments;
using Asteroids;
using CustomTimers;
using Rockets;
using Sputniks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
	public class MainUiManager : MonoBehaviour
	{
		[SerializeField] private AsteroidSpawner asteroidSpawner;
		[SerializeField] private RocketSpawner rocketSpawner;
		[SerializeField] private AsteroidFragmentsSpawner asteroidFragmentsSpawner;
		[SerializeField] private WaveEnemyController waveEnemyController;
		[SerializeField] private SputnikSpawner sputnikSpawner;

		[SerializeField] private TextMeshProUGUI textTestInfo;
		[SerializeField] private TextMeshProUGUI textMoney;

		[SerializeField] private Button buttonNextWave;
		[SerializeField] private Button buttonUpgrade;

		[SerializeField] private Button buttonTest;
		[SerializeField] private Button buttonStartSputnik;
		[SerializeField] private Button buttonSmall;
		[SerializeField] private Button buttonMiddle;
		[SerializeField] private Button buttonBig;

		[SerializeField] private GameObject canvasUpgrade;

		private CustomTimer _customTimerUpdateUi;

		private const float UpdateInterval = 0.5F;
		private double _lastInterval;
		private int _frames;
		private float _fps;

		private void Awake()
		{
			_customTimerUpdateUi = gameObject.AddComponent<CustomTimer>();

			buttonUpgrade.onClick.AddListener(() =>
			{
				canvasUpgrade.SetActive(true);
				GameConfig.GamePaused = true;
			});

			buttonTest.onClick.AddListener(() => Test());

			buttonStartSputnik.onClick.AddListener(() => sputnikSpawner.CreateSputnik());

			buttonSmall.onClick.AddListener(() => waveEnemyController.CreateAsteroid(AsteroidType.Small));
			buttonMiddle.onClick.AddListener(() => waveEnemyController.CreateAsteroid(AsteroidType.Middle));
			buttonBig.onClick.AddListener(() => waveEnemyController.CreateAsteroid(AsteroidType.Big));

			buttonNextWave.onClick.AddListener(() => waveEnemyController.NextWave());
		}

		private void Start()
		{
			_customTimerUpdateUi.InitTimer(0.1f, true, true);
			_customTimerUpdateUi.AddTask(UpdateUi);
			_customTimerUpdateUi.Run();

			Entity.reDrawUiAction += UpdateUi;
		}

		private void OnDestroy()
		{
			Entity.reDrawUiAction -= UpdateUi;
		}

		private void Update()
		{
			Fps();
		}

		private void UpdateUi()
		{
			var asteroidsList = asteroidSpawner.GetAsteroidsList();
			var rocketsList = rocketSpawner.GetRocketsList();
			var asteroidFragmentsControllerList = asteroidFragmentsSpawner.GetAsteroidFragmentsList();
			var sputniksList = sputnikSpawner.GetSputniksList();

			textTestInfo.text = $"Pause: {GameConfig.GamePaused} | FPS: {_fps:N1}\n" +
			                    $"Wn: {GameConfig.GetWaveNumber()} | " +
			                    $"Wt: {waveEnemyController.GetWaveTimeLeft():N1}\n" +
			                    $"Ast: {asteroidsList?.Count} \n" +
			                    $"Rckt: {rocketsList?.Count} \n" +
			                    $"Sptnk: {sputniksList?.Count} \n" +
			                    $"AstF: {asteroidFragmentsControllerList?.Count}";

			textMoney.text = string.Format($"{GameConfig.Money:### ### ### ###}");
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

		private void Test()
		{
			print($"Test()");
		}
	}
}
