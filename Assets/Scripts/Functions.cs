using UnityEngine;

public static class Functions
{
	public class Counter
	{
		private float _time;
		internal void Start() => _time = Time.time;
		internal void Reset() => Start();
		internal void Stop() => _time = -1;
		internal float CheckTime() => _time != 0 ? Time.time - _time : -1;
	}

	public class Timer
	{
		private float _time;
		internal void Start(float time) => _time = Time.time + time;
		internal void Stop() => _time = 0;
		internal bool CheckStop() => _time != 0 && Time.time >= _time;
		internal float CheckTime() => _time == 0 ? 0 : _time - Time.time;
	}
}
