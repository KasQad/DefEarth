namespace ScriptableObject.Rockets
{
	[UnityEngine.CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Rocket", order = 0)]
	public class Rocket : UnityEngine.ScriptableObject
	{
		public enum Type
		{
			RocketModel1,
			RocketModel2,
			RocketModel3
		}
		
		public string title;
		public float health;
		public float damage;
		public float speed;
	}
}
