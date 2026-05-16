using Godot;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

public static class Log {
	private static readonly string LogPath = ProjectSettings.GlobalizePath("user://game_logs.txt");
	private static readonly object LockObj = new object();
	private static readonly StreamWriter fileWriter;

	// ANSI/BBCode Colors for Editor Console
	private const string ColorReset = "[/color]";
	private const string ColorWarn = "[color=yellow]";
	private const string ColorErr = "[color=red]";
	private const string ColorInfo = "[color=cyan]";

	static Log() {
		try {
            // Append: false clears the log every time the game starts
            fileWriter = new StreamWriter(LogPath, append: false) {
                AutoFlush = true
            };

            Log.Info($"[LOG] File initialized at: {LogPath}");
		}
		catch (Exception e) {
			GD.PrintErr($"[LOG] Failed to initialize file logger: {e.Message}");
		}
	}

	[Conditional("DEBUG")]
	public static void Info(object msg, [CallerFilePath] string path = "", [CallerMemberName] string member = "") {
		Write("INFO", msg.ToString(), path, member, ColorInfo);
	}

	[Conditional("DEBUG")]
	public static void Warn(object msg, [CallerFilePath] string path = "", [CallerMemberName] string member = "") {
		GD.PushWarning($"{FormatHeader(path, member)} {msg}");
		Write("WARN", msg.ToString(), path, member, ColorWarn);
	}

	public static void Error(object msg, [CallerFilePath] string path = "", [CallerMemberName] string member = "") {
		GD.PushError($"{FormatHeader(path, member)} {msg}");
		Write("ERROR", msg.ToString(), path, member, ColorErr);
	}

	private static void Write(string level, string msg, string path, string member, string color) {
		string fileName = Path.GetFileNameWithoutExtension(path);
		string timestamp = DateTime.Now.ToString("HH:mm:ss");
		string header = $"[{timestamp}] [{level}] [{fileName} > {member}]";
		string fullMessage = $"{header}: {msg}";

		// 1. Output to Godot Editor Console with Colors
		GD.PrintRich($"{color}{fullMessage}{ColorReset}");

		// 2. Output to File (Thread Safe)
		lock (LockObj) {
			fileWriter?.WriteLine(fullMessage);
		}
	}

	private static string FormatHeader(string path, string member)
		=> $"[{Path.GetFileNameWithoutExtension(path)} > {member}]:";
}