
namespace ScriptableObject.Planets
{
	[UnityEngine.CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Planet", order = 0)]
	public class Planet : UnityEngine.ScriptableObject
	{
		public enum Type
		{
			Earth,
			Moon,
			Mars
		}
		
		public string title;
		public float orbitSpaceFragments;
		public float orbitSputniks;
		public float orbitCaptureGravity;
		public float speedRotate;
	}
}
