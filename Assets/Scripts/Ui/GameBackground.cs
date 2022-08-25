using DG.Tweening;
using UnityEngine;

namespace Ui
{
	public class GameBackground : MonoBehaviour
	{
		[SerializeField] private GameObject smallStars;
		[SerializeField] private SpriteRenderer bigStarsSpriteRenderer;

		private void Start()
		{
			bigStarsSpriteRenderer.DOColor(new Color(255, 255, 255, 0.3f), 10f)
				.SetEase(Ease.InQuart).SetLoops(-1, LoopType.Yoyo);
		}

		private void FixedUpdate()
		{
			smallStars.transform.Rotate(0,0 , 0.5f * Time.deltaTime);
		}
	}
}
