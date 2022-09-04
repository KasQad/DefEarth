namespace ScriptableObject.Sputnik
{
	[UnityEngine.CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Sputnik", order = 0)]
	public class Sputnik : UnityEngine.ScriptableObject
	{
		public string title;
		public float health;
		public float speedMin;
		public float speedMax;
	}
}
