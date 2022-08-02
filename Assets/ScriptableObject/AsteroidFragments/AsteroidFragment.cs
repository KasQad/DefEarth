using System.Collections.Generic;
using AsteroidFragments;
using UnityEngine;

namespace ScriptableObject.AsteroidFragments
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AsteroidFragment", order = 0)]
	public class AsteroidFragment : UnityEngine.ScriptableObject
	{
		public List<GameObject> asteroidFragmentsPrefabs = new List<GameObject>();

		public string title; 
		public float health;
		public float speed;
		public float damage;
		public float speedRotate;
	}
}
