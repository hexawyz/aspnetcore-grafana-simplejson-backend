using System.ComponentModel.DataAnnotations;

namespace SimpleJsonDataSource.ViewModels
{
	public class QueryViewModel
    {
		public QueryViewModel(DateTimeRangeViewModel range, int intervalMs, TargetViewModel[] targets, OutputFormat format, int maxDataPoints)
		{
			Range = range;
			IntervalMs = intervalMs;
			Targets = targets;
			Format = format;
			MaxDataPoints = maxDataPoints;
		}

		[Required]
		public DateTimeRangeViewModel Range { get; }
		[Required]
		public int IntervalMs { get; }
		[Required]
		public TargetViewModel[] Targets { get; }
		[Required]
		public OutputFormat Format { get; }
		[Required]
		public int MaxDataPoints { get; }
    }
}
