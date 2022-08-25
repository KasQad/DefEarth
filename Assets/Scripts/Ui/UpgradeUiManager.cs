using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
	public class UpgradeUiManager : MonoBehaviour
	{
		[SerializeField] private Button buttonObservation;
		[SerializeField] private Button buttonRockets;
		[SerializeField] private Button buttonSputniks;
		[SerializeField] private Button buttonPlanets;
		[SerializeField] private Button buttonClose;
		[Space]
		[SerializeField] private GameObject canvasUpgrade;
		[Space]
		[SerializeField] private GameObject contentObservation;
		[SerializeField] private GameObject contentRockets;
		[SerializeField] private GameObject contentSputniks;
		[SerializeField] private GameObject contentPlanets;

		private void Awake()
		{
			buttonObservation.onClick.AddListener(() => ChangeContent(contentObservation));
			buttonRockets.onClick.AddListener(() => ChangeContent(contentRockets));
			buttonSputniks.onClick.AddListener(() => ChangeContent(contentSputniks));
			buttonPlanets.onClick.AddListener(() => ChangeContent(contentPlanets));
			buttonClose.onClick.AddListener(() => CloseUpgradeCanvas(canvasUpgrade));
		}

		private void ChangeContent(GameObject content)
		{
			contentObservation.SetActive(false);
			contentRockets.SetActive(false);
			contentSputniks.SetActive(false);
			contentPlanets.SetActive(false);
			content.SetActive(true);
		}

		private void CloseUpgradeCanvas(GameObject content)
		{
			content.SetActive(false);
			GameConfig.GamePaused = false;
		}
	}
}
