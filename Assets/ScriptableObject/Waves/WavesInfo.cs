using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObject.Waves
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WavesInfo", order = 0)]
	public class WavesInfo : UnityEngine.ScriptableObject
	{
		public List<WaveData> waveDataList = new List<WaveData>();
	}
}
