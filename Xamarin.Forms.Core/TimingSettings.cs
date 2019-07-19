using System;
using System.Diagnostics;
using System.Text;

#if !DONT_TIME
[assembly: MethodTimer.Time]
#endif

#if NETSTANDARD2_0
public static class MethodTimeLogger
{
	public static void Log(System.Reflection.MethodBase methodBase, TimeSpan time, string message)
	{
		double ms = time.TotalMilliseconds;
		var parameters = methodBase.GetParameters();
		if (parameters.Length == 0)
		{
			Trace.WriteLine($"_TM_ {methodBase.DeclaringType.Name}.{methodBase.Name}() {ms}ms");
		}
		else
		{
			var b = new StringBuilder();
			foreach (var p in parameters)
			{
				if (b.Length > 0)
					b.Append(',');
				b.Append(p.ParameterType.Name);
			}
			Trace.WriteLine($"_TM_ {methodBase.DeclaringType.Name}.{methodBase.Name}({b}) {ms}ms");
		}
	}
}
#endif
