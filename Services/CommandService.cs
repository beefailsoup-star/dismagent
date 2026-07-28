using System.Diagnostics;

namespace DismAgent.Services;

public class CommandService
{
    public string Execute(string command)
    {
        try
        {
            using var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -Command \"{command.Replace("\"", "\\\"")}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();
            var o = p.StandardOutput.ReadToEnd();
            var e = p.StandardError.ReadToEnd();
            p.WaitForExit();
            return (o + (string.IsNullOrEmpty(e) ? "" : $"\n[ERR]\n{e}")).Trim();
        }
        catch (Exception ex) { return $"[ERROR] {ex.Message}"; }
    }
}
