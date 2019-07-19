using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace ParseTimingLog
{
	class Program
	{
		static void Main(string[] args)
		{
			var timings = new List<Timing>();
			var separator = new char[] { ' ' };

			var dir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
			using (var reader = File.OpenText(Path.Combine (dir, "log.txt")))
			{
				while (!reader.EndOfStream)
				{
					const string prefix = "_TM_";
					const string suffix = "ms";
					string line = reader.ReadLine();

					if (!line.EndsWith(suffix, StringComparison.Ordinal))
						continue;

					int index = line.IndexOf(prefix);
					if (index == -1)
						continue;

					index += prefix.Length + 1;
					line = line.Substring(index, line.Length - index);
					var split = line.Split(separator, 2);
					var ms = split[1];
					if (!double.TryParse(ms.Substring(0, ms.Length - suffix.Length), out double duration))
						continue;

					timings.Add(new Timing
					{
						Name = split[0],
						Duration = duration,
					});
				}
			}

			var group = from t in timings
						group t by t.Name;
			var query = from t in @group
						let c = t.Count()
						let sum = t.Sum(t => t.Duration)
						orderby sum descending
						select new { Name = t.Key, Count = c, Sum = sum };
			foreach (var t in query)
			{
				Console.WriteLine($"{t.Name} {t.Count} {t.Sum}");
			}
#if DEBUG
			Console.ReadLine();
#endif
		}

		class Timing
		{
			public string Name { get; set; }
			public double Duration { get; set; }
		}
	}
}
