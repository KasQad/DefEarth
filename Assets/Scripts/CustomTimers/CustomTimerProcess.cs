using UnityEngine;

namespace CustomTimers
{
	public class CustomTimerProcess : CustomTimer
	{
		private float _durationOn;
		private float _durationOff;

		internal void InitTimerProcess(float newDurationOn, float newDurationOff, bool newAutoReset = false,
			bool newRunCallBacksBeforeBeginningTimer = false)
		{
			if (newDurationOn == 0 || newDurationOff == 0)
			{
				print($"error: durationOn = 0 or durationOff = 0");
				return;
			}

			_durationOn = newDurationOn;
			_durationOff = newDurationOff;
			duration = newDurationOn + newDurationOff;
			autoReset = newAutoReset;
			runCallBacksBeforeBeginningTimer = newRunCallBacksBeforeBeginningTimer;
		}

		internal override bool CheckTime()
		{
			if (!isStarted) return false;
			var currentTime = Time.time - timeStart;
			if (currentTime > 0 && currentTime < _durationOn) return true;
			if (currentTime >= _durationOff && currentTime < duration) return false;
			if (!(currentTime >= duration)) return false;
			if (autoReset) Reset();
			else Stop();
			return false;
		}
	}
}
