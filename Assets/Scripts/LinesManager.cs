using System.Collections.Generic;
using UnityEngine;

public class LinesManager : MonoBehaviour
{
	public void DrawCircle(Transform transformParent, Vector2 position, float radius, float lineWidth, Color color)
	{
		GameObject lineObject = new GameObject();
		lineObject.name = $"orbitLine";
		lineObject.transform.parent = transformParent;

		var line = lineObject.AddComponent<LineRenderer>();

		const int segments = 180;

		line.startWidth = lineWidth;
		line.endWidth = lineWidth;

		line.loop = true;
		
		line.material.color = color;

		line.useWorldSpace = false;
		line.positionCount = segments + 1;

		const int pointCount = segments + 1;
		var points = new Vector3[pointCount];

		for (var i = 0; i < pointCount; i++)
		{
			var rad = Mathf.Deg2Rad * (i * 360f / segments);
			points[i] = new Vector3(position.x + Mathf.Sin(rad) * radius,
				position.y + Mathf.Cos(rad) * radius);
		}

		line.SetPositions(points);
	}

	public void DrawLine(Transform transformObject, List<Vector2> pointList, float lineWidth, Color color)
	{
		GameObject lineObject = new GameObject();
		lineObject.name = $"Line";
		lineObject.transform.parent = transformObject;

		var line = lineObject.AddComponent<LineRenderer>();

		line.positionCount = pointList.Count;
		
		line.startWidth = lineWidth;
		line.endWidth = lineWidth;

		line.loop = false;

		var material = Resources.Load("Materials/Laser") as Material;
		line.material = material;
		
		line.material.color = color;

		line.useWorldSpace = false;

		var points = new Vector3[pointList.Count];

		for (var i = 0; i < pointList.Count; i++)
		{
			points[i] = new Vector3(pointList[i].x, pointList[i].y);
		}

		line.SetPositions(points);
	}
}
