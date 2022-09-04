using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
	public bool isEnemy;

	public EntityType entityType;
	public string title;
	public float damage;

	public Vector2 GetPosition() => transform.position;


	public static Action reDrawUiAction;

	public void GetReward(long newReward)
	{
		GameConfig.Money += newReward;
		reDrawUiAction?.Invoke();
	}
}
