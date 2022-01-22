using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using Flow.Launcher.Plugin;



public class Main : IPlugin
{
	Process process;
	public void Init(PluginInitContext context)
	{
		context.CurrentPluginMetadata.ActionKeywords.Add("qap");
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
		string procArguments = " -m 1500 ";
		if (query.ActionKeyword == "qap")
		{
			procArguments = " -p 10 -m 1500 ";
		}
		string processOutput = getProcOutput(query.Search, procArguments);
		bool resultIsMultiLine = processOutput.IndexOf('\n') == -1;
		List<Result> result = new List<Result> { };
		result.Add(new Result
		{
			IcoPath = "qalculate.png",
			Score = 1,
			Action = e =>
			{
				string resInput = query.Search;
				string resArgs = procArguments + " -t ";
				var output = getProcOutput(resInput, resArgs);
				Clipboard.SetText(output);
				return true;
			}
		});
		if (resultIsMultiLine)
			result[0].Title = processOutput;
		else
			result[0].SubTitle = processOutput;
		return result;
	}

	string getProcOutput(string input, string arguments)
	{
		process.StartInfo.Arguments = arguments;
		process.StartInfo.Arguments += '\"' + input + '\"';
		process.Start();
		var output = process.StandardOutput.ReadToEnd();
		try { if (!process.HasExited) process.WaitForExit(); } catch (System.InvalidOperationException) { }
		if (output.Length > 0)
			output = output.Substring(0, output.Length - 2);
		return output;
	}

	~Main()
	{
		process.Kill();
		process.Dispose();
	}
}