using SimpleJsonDataSource.Models;
using System;
using System.Collections.Generic;

namespace SimpleJsonDataSource
{
	public static class DataSeriesFilter
	{
		public static IEnumerable<DataPoint<double>> FilterDataPoints(this IEnumerable<DataPoint<double>> dataPoints, DateTime startDateTime, DateTime endDateTime, TimeSpan samplingInterval, TimeSpan smoothingWindow)
		{
			if (smoothingWindow.Ticks <= 0) throw new ArgumentOutOfRangeException(nameof(smoothingWindow));
			if (samplingInterval.Ticks <= 0) throw new ArgumentOutOfRangeException(nameof(samplingInterval));

			var duration = (endDateTime - startDateTime);

			if (duration.Ticks < 0) throw new ArgumentException();

			var points = new(double Value, double Count)[checked((int)Math.Max(1, unchecked(duration.Ticks / samplingInterval.Ticks)))];

			long halfWindowTicks = Math.Max(1, smoothingWindow.Ticks >> 1);

			int firstPointIndex = 0;
			DateTime firstPointDateTime = startDateTime;

			foreach (var dataPoint in dataPoints)
			{
				int pointIndex;
				DateTime pointDateTime;

				for (pointIndex = firstPointIndex, pointDateTime = firstPointDateTime; pointIndex < points.Length && pointDateTime < endDateTime; pointIndex++, pointDateTime += samplingInterval)
				{
					long distance = (dataPoint.DateTime - pointDateTime).Ticks;

					if (distance < -halfWindowTicks)
					{
						break;
					}
					else if (distance > halfWindowTicks)
					{
						firstPointIndex = pointIndex + 1;
						firstPointDateTime = pointDateTime + samplingInterval;
						continue;
					}

					double d = (double)distance / halfWindowTicks;

					Aggregate(ref points[pointIndex], LinearInterpolate(dataPoint.Value, d), d);
				}

				if (firstPointIndex >= points.Length) break;
			}

			return GetDataPointEnumerable(points, startDateTime, samplingInterval);
		}

		private static IEnumerable<DataPoint<double>> GetDataPointEnumerable((double Value, double Count)[] points, DateTime startDateTime, TimeSpan samplingInterval)
		{
			int pointIndex;
			DateTime pointDateTime;

			for (pointIndex = 0, pointDateTime = startDateTime; pointIndex < points.Length; pointIndex++, pointDateTime += samplingInterval)
			{
				yield return CreateDataPoint(ref points[pointIndex], pointDateTime);
			}
		}

		private static DataPoint<double> CreateDataPoint(ref (double Value, double Count) point, DateTime dateTime)
			=> new DataPoint<double>(dateTime, point.Count > 0 ? point.Value / point.Count : 0);

		private static double LinearInterpolate(double value, double distance) => value * (1d - Math.Abs(distance));
		
		private static void Aggregate(ref (double Value, double Count) point, double value, double distance)
		{
			point.Value += value;
			point.Count += 1d - Math.Abs(distance);
		}
	}
}
