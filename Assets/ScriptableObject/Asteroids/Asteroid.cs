namespace ScriptableObject.Asteroids
{
	[UnityEngine.CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Asteroid", order = 0)]
	public class Asteroid : UnityEngine.ScriptableObject
	{
		public string title;
		public float health;
		public float speedMin;
		public float speedMax;
		public float damage;
		public int reward;
	}
}
