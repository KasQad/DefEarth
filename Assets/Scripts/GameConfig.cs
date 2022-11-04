using UnityEngine;

public static class GameConfig
{
	public static bool GamePaused { get; set; }

	public static long Money { get; set; }

	public const int AsteroidCountLimitOnOrbit = 50;
	public const int AsteroidCountLimitOnScene = 100;
	
	public const float RadiusDeactivationAsteroid = 20;

	public static readonly float[] ObjectObservationRadius = { 12, 15, 18 };
	
	
	//spawn enemy entity
	public const float RadiusSpawnEnemyEntity = 6f;
	public const float RadiusSpawnEnemyEntityShiftY = -10f;
	public const float RadiusSpawnEnemyEntityShiftX = 1f;
	
	public const float RadiusDeSpawnRelativeToPlanetPositionAndRadius = 1.5f;//0-2f

	public static int currentWaveNumber;
	
	//upgrade object observation
	private static int upgradeObjectObservationLevel = 2;
	// private static bool upgradeIgnoreAsteroidsFlyingPastThePlanet = false;
	
	
	//upgrade Asteroids DropChanceAsteroidFragments
	public static float upgradeDropChanceAsteroidFragmentGold = 0;
	public static float upgradeDropChanceAsteroidFragmentCrystal = 0;
	
	
	//upgrade rockets
	public static int upgradeRocketsSpeedLevel = 0;
	public static int upgradeRocketsManeuverabilityLevel = 0;
	public static int upgradeRocketsDamageLevel = 0;
	public static bool upgradeDodgingAsteroidFragments = false;
	
	//upgrade sputniks
	public static int upgradeSputniksSatelliteLaunchLevel = 0;
	public static int upgradeSputniksLaserMachineLevel = 0;
	public static int upgradeSputniksSaleOfResourcesLevel = 0;
	
	//upgrade planet
	public static int upgradePlanet = 0;

	public static float GetCurrentObjectObservationRadius() => ObjectObservationRadius[upgradeObjectObservationLevel];
	public static int GetWaveNumber() => currentWaveNumber;

}
