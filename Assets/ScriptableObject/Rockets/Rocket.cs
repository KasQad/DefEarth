namespace ScriptableObject.Rockets
{
	[UnityEngine.CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Rocket", order = 0)]
	public class Rocket : UnityEngine.ScriptableObject
	{
		public string title;
		public float health;
		public float damage;
		public float speed;
		public float angularSpeed;
	}
}
