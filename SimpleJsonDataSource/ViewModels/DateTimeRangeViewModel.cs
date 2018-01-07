using System;

namespace SimpleJsonDataSource.ViewModels
{
	public struct DateTimeRangeViewModel
    {
		public DateTimeRangeViewModel(DateTime from, DateTime to)
		{
			From = from;
			To = to;
		}

		public DateTime From { get; }
		public DateTime To { get; }
    }
}
