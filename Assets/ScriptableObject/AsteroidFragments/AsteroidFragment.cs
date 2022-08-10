using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObject.AsteroidFragments
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AsteroidFragment", order = 0)]
	public class AsteroidFragment : UnityEngine.ScriptableObject
	{
		public List<GameObject> asteroidFragmentsPrefabs = new List<GameObject>();

		public string title; 
		public float health;
		public float damage;
		public float speedMin;
		public float speedMax;
		public float speedRotateMin;
		public float speedRotateMax;
	}
}
