using System;
using UnityEngine;

namespace CustomTimers
{
	public class CustomTimer : MonoBehaviour
	{
		protected float _timeStart;
		protected float _timeCount;
		protected bool _isStarted;
		protected bool _autoReset;
		protected bool _runTasksBeforeTimer;

		private Action _taskAction;
		private bool _taskIsCompleted;

		internal void AddTask(Action taskAction) => _taskAction += taskAction;
		internal void DelTask(Action taskAction) => _taskAction -= taskAction;
		internal void ClearTasks() => _taskAction = null;

		private void CheckTask()
		{
			if (!_isStarted) return;
			if (_taskAction == null) return;
			if (_runTasksBeforeTimer)
			{
				if (!_taskIsCompleted)
				{
					_taskAction?.Invoke();
					_taskIsCompleted = true;
				}

				if (CheckTime()) _taskIsCompleted = false;
			}
			else if (CheckTime()) _taskAction?.Invoke();
		}

		private void Update()
		{
			if (CheckRun()) CheckTask();
		}

		internal void InitTimer(float timeCount, bool autoReset = false, bool runTasksBeforeTimer = false)
		{
			_timeCount = timeCount;
			_autoReset = autoReset;
			_runTasksBeforeTimer = runTasksBeforeTimer;
		}

		internal void Run()
		{
			if (_timeCount == 0)
			{
				print($"error: timeCount = 0");
				return;
			}

			_timeStart = Time.time;
			_isStarted = true;
		}

		internal void Reset() => Run();
		internal void Stop() => _isStarted = false;
		internal bool AutoReset(bool autoReset) => _autoReset = autoReset;
		internal void RunTasksBeforeTimer(bool runTasksBeforeTimer) => _runTasksBeforeTimer = runTasksBeforeTimer;

		internal bool CheckRun() => _isStarted;

		internal float GetTime()
		{
			if (!_isStarted) return 0;
			return Time.time - _timeStart;
		}

		internal float GetLeftTime()
		{
			if (!_isStarted) return 0;
			return _timeCount - GetTime();
		}

		internal bool GetStatus() => _isStarted;

		internal virtual bool CheckTime()
		{
			if (!_isStarted) return false;
			if (!(Time.time - _timeStart > _timeCount)) return false;
			if (_autoReset) Reset();
			else Stop();
			return true;
		}
	}
}
