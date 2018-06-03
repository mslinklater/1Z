using UnityEngine;
using System.Collections;
using System.IO;

// Nasty global stream writing class

public static class DebugLogger {

	private static StreamWriter m_debugOutput;

	public static void WriteLine (object line) {
		if (m_debugOutput == null) {
			m_debugOutput = File.CreateText("output.txt");
			m_debugOutput.AutoFlush = true;
		}
		m_debugOutput.WriteLine (line);
	}
}
