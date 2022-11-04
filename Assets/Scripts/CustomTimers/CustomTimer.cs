using System;
using UnityEngine;

namespace CustomTimers
{
	public class CustomTimer : MonoBehaviour
	{
		protected float timeStart;
		protected float duration;
		protected bool isStarted;
		protected bool autoReset;
		protected bool runCallBacksBeforeBeginningTimer;

		private Action _callBack;
		private bool _callBackIsCompleted;

		internal void AddCallBack(Action callBack) => _callBack += callBack;
		internal void DelCallBack(Action callBack) => _callBack -= callBack;
		internal void ClearCallBacks() => _callBack = null;

		private void CheckCallbacks()
		{
			if (!isStarted) return;
			if (_callBack == null) return;
			if (runCallBacksBeforeBeginningTimer)
			{
				if (!_callBackIsCompleted)
				{
					_callBack?.Invoke();
					_callBackIsCompleted = true;
				}

				if (CheckTime()) _callBackIsCompleted = false;
			}
			else if (CheckTime()) _callBack?.Invoke();
		}

		private void Update()
		{
			if (CheckRun()) CheckCallbacks();
		}

		internal void InitTimer(float duration, bool autoReset = false, bool runCallBacksBeforeBeginningTimer = false)
		{
			this.duration = duration;
			this.autoReset = autoReset;
			this.runCallBacksBeforeBeginningTimer = runCallBacksBeforeBeginningTimer;
		}

		internal void Run()
		{
			if (duration == 0)
			{
				print($"error: duration = 0");
				return;
			}

			timeStart = Time.time;
			isStarted = true;
		}

		internal void Reset() => Run();
		internal void Stop() => isStarted = false;
		internal bool AutoReset(bool newAutoReset) => autoReset = newAutoReset;

		internal void RunCallBacksBeforeBeginningTimer(bool runTasksBeforeTimer) =>
			runCallBacksBeforeBeginningTimer = runTasksBeforeTimer;

		internal bool CheckRun() => isStarted;

		internal float GetTime()
		{
			if (!isStarted) return 0;
			return Time.time - timeStart;
		}

		internal float GetLeftTime()
		{
			if (!isStarted) return 0;
			return duration - GetTime();
		}

		internal bool GetStatus() => isStarted;

		internal virtual bool CheckTime()
		{
			if (!isStarted) return false;
			if (!(Time.time - timeStart > duration)) return false;
			if (autoReset) Reset();
			else Stop();
			return true;
		}
	}
}
