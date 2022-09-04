using UnityEngine;

namespace CustomTimers
{
	public class CustomTimerProcess : CustomTimer
	{
		private float _timeCountOn;
		private float _timeCountOff;

		internal void InitTimerProcess(float timeCountOn, float timeCountOff, bool autoReset = false,
			bool runTasksBeforeTimer = false)
		{
			if (timeCountOn == 0 || timeCountOff == 0)
			{
				print($"error: timeCountOn = 0 or timeCountOff = 0");
				return;
			}

			_timeCountOn = timeCountOn;
			_timeCountOff = timeCountOff;
			_timeCount = timeCountOn + timeCountOff;
			_autoReset = autoReset;
			_runTasksBeforeTimer = runTasksBeforeTimer;
		}

		internal override bool CheckTime()
		{
			if (!_isStarted) return false;
			var currentTime = Time.time - _timeStart;
			if (currentTime > 0 && currentTime < _timeCountOn) return true;
			if (currentTime >= _timeCountOff && currentTime < _timeCount) return false;
			if (!(currentTime >= _timeCount)) return false;
			if (_autoReset) Reset();
			else Stop();
			return false;
		}
	}
}
