using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Common
{
public class FunctionTimer : IDisposable
{
	private string m_memberName;
	private TimeSpan m_maxTime;
	private Stopwatch m_stopwatch;
	private bool m_isDisposed;

	public FunctionTimer(TimeSpan maxTime, [CallerMemberName] string memberName = "")
	{
		m_maxTime = maxTime;
		m_stopwatch = Stopwatch.StartNew();
		m_memberName = memberName;
	}

	public void Dispose()
	{
		if (m_isDisposed)
		{
			return;
		}
		m_isDisposed = true;
		if (m_stopwatch.Elapsed > m_maxTime)
		{
			Console.WriteLine($"Warning: Long running function: {m_memberName}. {m_stopwatch.Elapsed} - expected max {m_maxTime}");
			Debugger.Break();
		}
	}
}
}
