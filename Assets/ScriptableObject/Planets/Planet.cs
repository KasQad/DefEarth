
namespace ScriptableObject.Planets
{
	[UnityEngine.CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Planet", order = 0)]
	public class Planet : UnityEngine.ScriptableObject
	{

		
		public string title;
		public float radiusPlanet;
		public float radiusOrbitSpaceFragments;
		public float radiusOrbitSputniks;
		public float radiusOrbitCaptureGravity;
		public float speedRotate;
	}
}
