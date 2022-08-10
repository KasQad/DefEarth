using UnityEngine;

public class Functions : MonoBehaviour
{
	public static float GetAngleBetweenTwoLines(Vector2 a, Vector2 b, Vector2 c)
	{
		return Vector2.Angle(a - c, b - c);
	}

	public static Vector2 GetXYCoordsOnCircleByAngle(Vector2 center, float radius, float angle)
	{
		var radians = Mathf.PI * angle / 180.0f;
		var x = center.x + radius * Mathf.Cos(radians);
		var y = center.y + radius * Mathf.Sin(radians);
		return new Vector2(x, y);
	}


	public static void RotateTo(Transform transform, Vector2 vectorPoint)
	{
		var position = transform.position;
		var y = position.y - vectorPoint.y;
		var x = position.x - vectorPoint.x;
		var angel = Mathf.Atan2(y, x) / Mathf.PI * 180 + 90;
		transform.rotation = Quaternion.Euler(0, 0, angel);
	}
}
