using Planets;
using UnityEngine;

public class TestDrawLines : MonoBehaviour
{
	[SerializeField] private LinesManager linesManager;
	[SerializeField] private PlanetSpawner planetSpawner;

	private void Start()
	{
		var planetCenter = planetSpawner.GetPlanetByType(PlanetType.Earth).GetPosition();
		linesManager.DrawCircle(transform, planetCenter, GameConfig.ObjectObservationRadius[0], 0.03f, Color.gray);
		linesManager.DrawCircle(transform, planetCenter, GameConfig.ObjectObservationRadius[1], 0.03f, Color.gray);
		linesManager.DrawCircle(transform, planetCenter, GameConfig.ObjectObservationRadius[2], 0.03f, Color.gray);
		linesManager.DrawCircle(transform, new Vector2(0, 0), GameConfig.RadiusDeactivationAsteroid, 0.03f, Color.red);
	}
}
