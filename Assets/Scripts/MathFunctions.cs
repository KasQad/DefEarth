using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MathFunctions : MonoBehaviour
{
	public static float GetAngleBetweenTwoLines(Vector2 a, Vector2 b, Vector2 c)
	{
		return Vector2.Angle(a - c, b - c);
	}

	public static Vector2 GetXYCoordsOnBorderCircleByAngle(Vector2 center, float radius, float angle)
	{
		var radians = Mathf.PI * angle / 180.0f;
		var x = center.x + radius * Mathf.Cos(radians);
		var y = center.y + radius * Mathf.Sin(radians);
		return new Vector2(x, y);
	}

	public static Vector2 GetPointInsideCircle(Vector2 center, float radius = 1)
	{
		var randomAngle = Random.Range(0f, 2 * Mathf.PI - float.Epsilon);
		var pointOnCircle = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * radius + center;
		return pointOnCircle;
	}

	public static void RotateTo(Transform transform, Vector2 vectorPoint)
	{
		var position = transform.position;
		var y = position.y - vectorPoint.y;
		var x = position.x - vectorPoint.x;
		var angel = Mathf.Atan2(y, x) / Mathf.PI * 180 + 90;
		transform.rotation = Quaternion.Euler(0, 0, angel);
	}

	public static Vector2 LineExtension(Vector2 start, Vector2 end, float length)
	{
		var vector = new Vector2(end.x - start.x, end.y - start.y);
		var lengthVector = (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
		vector = new Vector2(vector.x / lengthVector, vector.y / lengthVector);
		return new Vector2(end.x + vector.x * length, end.y + vector.y * length);
	}
	
	public static bool CircleLineIntersect__(Vector2 linePoint1, Vector2 linePoint2, Vector2 circleCenter, float circleRadius)
	{
		var dx = linePoint2.x - linePoint1.x;
		var dy = linePoint2.y - linePoint1.y;
		var a = dx * dx + dy * dy;
		var b = 2 * (dx * (linePoint1.x - circleCenter.x) + dy * (linePoint1.y - circleCenter.y));
		var c = circleCenter.x * circleCenter.x + circleCenter.y * circleCenter.y;
		c += linePoint1.x * linePoint1.x + linePoint1.y * linePoint1.y;
		c -= 2 * (circleCenter.x * linePoint1.x + circleCenter.y * linePoint1.y);
		c -= circleRadius * circleRadius;
		var bb4Ac = b * b - 4 * a * c;
		return !(bb4Ac < 0);
	}
	
	public static bool CircleLineIntersect(Vector2 linePoint1, Vector2 linePoint2, Vector2 circleCenter, float circleRadius)
	{
		var dx = linePoint2.x - linePoint1.x;
		var dy = linePoint2.y - linePoint1.y;
		var vector = Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2);
		var t = ((circleCenter.x - linePoint1.x) * dx + (circleCenter.y - linePoint1.y) * dy) / vector;
		if(t > 1) t = 1;
		else if(t < 0) t = 0;
		var nearestX = linePoint1.x + t * dx;
		var nearestY = linePoint1.y + t * dy;
		var dist = Mathf.Sqrt(Mathf.Pow(nearestX - circleCenter.x, 2) + Mathf.Pow(nearestY - circleCenter.y, 2));
		return !(dist > circleRadius);
	}
	
}
