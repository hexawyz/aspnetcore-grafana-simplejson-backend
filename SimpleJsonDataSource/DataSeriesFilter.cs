using SimpleJsonDataSource.Models;
using System;
using System.Collections.Generic;

namespace SimpleJsonDataSource
{
	public static class DataSeriesFilter
	{
		public static IEnumerable<DataPoint<double>> FilterDataPoints(this IEnumerable<DataPoint<double>> dataPoints, DateTime startDateTime, DateTime endDateTime, TimeSpan samplingInterval, TimeSpan smoothingWindow)
		{
			var windows = Init(startDateTime, samplingInterval, smoothingWindow);
			int windowIndex = 0;

			DateTime nextWindowStartDateTime = windows[windows.Length - 1].StartDateTime + samplingInterval;
			DateTime currentPointDateTime = startDateTime;

			sbyte r;

			foreach (var dataPoint in dataPoints)
			{
				while ((r = Aggregate(ref windows[windowIndex], dataPoint, endDateTime)) >= 0)
				{
					if (r > 0)
					{
						yield return CreateDataPoint(currentPointDateTime, ref windows[windowIndex]);

						ResetWindow(ref windows[windowIndex], ref nextWindowStartDateTime, samplingInterval);

						if ((currentPointDateTime += samplingInterval) > endDateTime)
						{
							yield break;
						}
					}

					if (++windowIndex == windows.Length)
					{
						windowIndex = 0;
					}
				}
			}
			
			while (currentPointDateTime <= endDateTime)
			{
				yield return CreateDataPoint(currentPointDateTime, ref windows[windowIndex]);

				ResetWindow(ref windows[windowIndex], ref nextWindowStartDateTime, samplingInterval);

				currentPointDateTime += samplingInterval;

				if (++windowIndex == windows.Length)
				{
					windowIndex = 0;
				}
			}
		}

		private static void ResetWindow(ref (double Sum, int Count, DateTime StartDateTime, DateTime EndDateTime) window, ref DateTime nextWindowStartDateTime, TimeSpan samplingInterval)
		{
			window.Sum = 0;
			window.Count = 0;
			window.StartDateTime = nextWindowStartDateTime;
			window.EndDateTime = nextWindowStartDateTime += samplingInterval;
		}

		private static DataPoint<double> CreateDataPoint(DateTime dateTime, ref (double Sum, int Count, DateTime StartDateTime, DateTime EndDateTime) window)
			=> new DataPoint<double>(dateTime, window.Count > 0 ? window.Sum / window.Count : 0);

		private static sbyte Aggregate(ref (double Sum, int Count, DateTime StartDateTime, DateTime EndDateTime) window, DataPoint<double> dataPoint, DateTime endDateTime)
		{
			if (dataPoint.DateTime >= window.StartDateTime)
			{
				if (dataPoint.DateTime <= window.EndDateTime)
				{
					window.Sum += dataPoint.Value;
					window.Count++;
					return 0;
				}
				else
				{
					return 1;
				}
			}

			return -1;
		}

		private static (double Sum, int Count, DateTime StartDateTime, DateTime EndDateTime)[] Init(DateTime startDateTime, TimeSpan samplingInterval, TimeSpan smoothingWindow)
		{
			var windowCount = Math.Max(smoothingWindow.Ticks / samplingInterval.Ticks, 1);

			var windows = new(double Sum, int Count, DateTime StartDateTime, DateTime EndDateTime)[windowCount];
			DateTime nextWindowStartDateTime = startDateTime.AddTicks(-(smoothingWindow.Ticks >> 1));

			for (int i = 0; i < windows.Length; i++)
			{
				ref var window = ref windows[i];

				window.StartDateTime = nextWindowStartDateTime;
				window.EndDateTime = nextWindowStartDateTime += samplingInterval;
			}

			return windows;
		}
	}
}
