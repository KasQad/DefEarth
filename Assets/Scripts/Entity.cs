using Types;
using Ui;
using UnityEngine;

public class Entity : MonoBehaviour
{
	public bool isEnemy;

	public EntityType entityType;
	public string title;
	public float damage;

	public Vector2 GetPosition() => transform.position;

	public bool IsActive() => transform.gameObject.activeInHierarchy;

	public void GetReward(long newReward)
	{
		GameConfig.Money += newReward;
		MainUiManager.Instance.UpdateUi();
	}
}
