using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
	public class GameUiManager : MonoBehaviour
	{
		[SerializeField] private WaveEnemyController waveEnemyController;
		[SerializeField] private PlayerRocketLauncherController playerRocketLauncherController;
	
		[SerializeField] private Button buttonNextWave;
		[SerializeField] private Button buttonUpgrade;
	
		[SerializeField] private Button buttonStartRocket;
		[SerializeField] private Button buttonSmall;
		[SerializeField] private Button buttonMiddle;
		[SerializeField] private Button buttonBig;

		[SerializeField] private GameObject canvasUpgrade;

		private void Awake()
		{
			buttonUpgrade.onClick.AddListener(() =>
			{
				canvasUpgrade.SetActive(true);
				GameConfig.GamePaused = true;
			});
		
			buttonStartRocket.onClick.AddListener(() => playerRocketLauncherController.LaunchRocketAtNearestFreeAsteroidFound());
			buttonSmall.onClick.AddListener(() => waveEnemyController.CreateAsteroid(AsteroidType.Small));
			buttonMiddle.onClick.AddListener(() => waveEnemyController.CreateAsteroid(AsteroidType.Middle));
			buttonBig.onClick.AddListener(() => waveEnemyController.CreateAsteroid(AsteroidType.Big));

			buttonNextWave.onClick.AddListener(() => waveEnemyController.NextWave());
		}
	}
}
