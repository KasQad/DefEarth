using Interfaces;
using UnityEngine;

namespace Rockets
{
	public class Entity : MonoBehaviour
	{		
		public string title;

		public bool Enemy { get; protected set; }

		public delegate void DestroyEntityDelegate(Entity entity);
		public static event DestroyEntityDelegate DestroyEntity;
		
		public void Destroy()
		{
			print($"{title} Destroyed");
			DestroyEntity?.Invoke(this);
		}
	}
}
