using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
	public List<Vector2> CreatePathPointsListByBezierMethod(List<Transform> listKeyPoint)
	{
		if (listKeyPoint.Count == 2)
		{
			print($"listKeyPoint.Count == 2");
			return new List<Vector2> { listKeyPoint[0].position, listKeyPoint[1].position };
		}

		List<Vector2> pathList = new List<Vector2>();
		const int numberOfPoints = 60;

		pathList.Add(listKeyPoint[0].position);

		for (var j = 0; j < listKeyPoint.Count - 2; j++)
		{
			if (listKeyPoint[j] == null || listKeyPoint[j + 1] == null || listKeyPoint[j + 2] == null)
				return null;

			var p0 = 0.5f * (listKeyPoint[j].position + listKeyPoint[j + 1].position);
			var p1 = listKeyPoint[j + 1].position;
			var p2 = 0.5f * (listKeyPoint[j + 1].position + listKeyPoint[j + 2].position);

			var pointStep = 1.0f / numberOfPoints;

			if (j == listKeyPoint.Count - 3)
				pointStep = 1.0f / (numberOfPoints - 1.0f);

			for (var i = 0; i < numberOfPoints; i++)
			{
				var t = i * pointStep;
				var position = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
				pathList.Add(position);
			}
		}

		pathList.Add(listKeyPoint[listKeyPoint.Count - 1].position);
		return pathList;
	}
}
