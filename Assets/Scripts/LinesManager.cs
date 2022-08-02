using UnityEngine;

public class LinesManager : MonoBehaviour
{
	public void DrawCircle(Transform transformObject, float radius, float lineWidth, Color color)
	{

		GameObject lineObject = new GameObject();
		lineObject.name = $"orbitLine";
		lineObject.transform.parent = transformObject;

		
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
			points[i] = new Vector3(transformObject.position.x + Mathf.Sin(rad) * radius,
				transformObject.position.y + Mathf.Cos(rad) * radius);
		}

		line.SetPositions(points);
	}
}
