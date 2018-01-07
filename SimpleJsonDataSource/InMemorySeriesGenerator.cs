using SimpleJsonDataSource.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleJsonDataSource
{
    public sealed class InMemorySeriesGenerator : InMemoryDataSeriesRepository<double>, IDisposable
	{
		private readonly Random _random;
		private Timer _timer;
		private TimeSpan _period;
		private DateTime _nextDateTime;
		private double _lastValue;

		public InMemorySeriesGenerator(int seed, TimeSpan period)
		{
			_random = new Random(seed);
			_period = period;

			Init(DateTime.UtcNow, new TimeSpan(12, 0, 0));
			
			_timer = new Timer(Tick, null, 0, Timeout.Infinite);
		}

		private void Init(DateTime now, TimeSpan duration)
		{
			_nextDateTime = now - duration;

			while (_nextDateTime <= now)
			{
				AddValue();
			}
		}

		public void Dispose()
		{
			_timer.Dispose();
		}

		private void Tick(object state)
		{
			var now = DateTime.UtcNow;

			while (_nextDateTime < now)
			{
				AddValue();
			}

			_timer.Change(_period, Timeout.InfiniteTimeSpan);
		}

		private void AddValue()
		{
			AddValue(_nextDateTime);
			_nextDateTime += _period;
		}

		private void AddValue(DateTime dateTime)
			=> Add(dateTime, _lastValue = Math.Max(0, _lastValue * 0.25d + 100 * _random.NextDouble()));
	}
}
