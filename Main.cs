using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using Flow.Launcher.Plugin;



public class Main : IPlugin
{
	Process process;
	public void Init(PluginInitContext context)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo
		{
			FileName = "C:/Program Files/Qalculate/qalc.exe",
			Arguments = "-e 0",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardInput = true,
			CreateNoWindow = true,

		};
		process = new Process { };
		process.StartInfo = processStartInfo;
		process.Start();
	}

	public List<Result> Query(Query query)
	{
		string processOutput = getProcOutput(query.Search);
		bool resultIsMultiLine = processOutput.IndexOf('\n') == -1;
		List<Result> result = new List<Result> { };

		result.Add(new Result
		{

			IcoPath = "qalculate.png",
			Score = 1,
			Action = e =>
			{
				var input = query.Search;
				var output = getProcOutput(input, true);
				System.Windows.Clipboard.SetText(output);
				return true;
			}
		});
		if (resultIsMultiLine)
			result[0].Title = processOutput;
		else
			result[0].SubTitle = processOutput;

		return result;
	}

	string getProcOutput(string input, bool terse = false)
	{
		process.StartInfo.Arguments = " -m 1500 ";
		if (terse)
			process.StartInfo.Arguments += "-t ";
		process.StartInfo.Arguments += '\"' + input + '\"';
		process.Start();
		var output = process.StandardOutput.ReadToEnd();
		if (output.Length > 0)
			output = output.Substring(0, output.Length - 2);
		process.WaitForExit();
		return output;
	}

	~Main()
	{
		process.Kill();
		process.Dispose();
	}
}