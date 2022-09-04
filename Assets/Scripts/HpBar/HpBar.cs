using System;
using UnityEngine;
using UnityEngine.UI;

namespace HpBar
{
	public class HpBar : MonoBehaviour
	{
		[SerializeField] private Image imageValue;
		[SerializeField] private GameObject gameObjectImageValue;
		private float _valueMax;
		private float _valueCurrent;
		private Vector2 _imageValueSizeDelta;

		public void Awake()
		{
			_imageValueSizeDelta = imageValue.rectTransform.sizeDelta;
		}

		internal void SetValue(float valueCurrent, float valueMax)
		{
			_valueMax = valueMax;
			_valueCurrent = valueCurrent;
			SetValue(_valueCurrent);
		}

		internal void SetValue(float valueCurrent)
		{
			_valueCurrent = valueCurrent;
			imageValue.rectTransform.sizeDelta =
				new Vector2(_imageValueSizeDelta.x * (_valueCurrent / _valueMax), _imageValueSizeDelta.y);

			gameObject.SetActive(!(Math.Abs(valueCurrent - _valueMax) < 0.01f));
			gameObjectImageValue.SetActive(!(valueCurrent <= 0));
		}
	}
}
