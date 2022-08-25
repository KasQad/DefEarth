using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
	public class HpBar : MonoBehaviour
	{
		[SerializeField] protected Image imageValue;
		private Camera _camera;
		private float _valueMax;
		private readonly Vector3 _offsetImageValue = new Vector3(0, -0.8f, 0);
		private Vector2 _imageValueSizeDelta;

		private void Awake()
		{
			_camera = Camera.main;
			_imageValueSizeDelta = imageValue.rectTransform.sizeDelta;
		}

		internal void Initialize(ref Action<float> changeHp, float value, float valueMax)
		{
			_valueMax = valueMax;
			SetValue(value);
			changeHp += SetValue;
		}

		private void SetValue(float value)
		{
			imageValue.rectTransform.sizeDelta =
				new Vector2(_imageValueSizeDelta.x * (value / _valueMax), _imageValueSizeDelta.y);
		}

		private void LateUpdate()
		{
			transform.rotation = _camera.transform.rotation;
			transform.position = _offsetImageValue + transform.parent.position;
		}
	}
}
