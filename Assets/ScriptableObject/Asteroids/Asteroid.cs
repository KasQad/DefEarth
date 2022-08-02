namespace ScriptableObject.Asteroids
{
	[UnityEngine.CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Asteroid", order = 0)]
	public class Asteroid : UnityEngine.ScriptableObject
	{
		public enum Type
		{
			Small,
			Middle,
			Big
		}

		public string title;
		public float health;
		public float speed;
		public float damage;
		public float speedRotate;
	}
}
