using DismAgent.Models;
using DismAgent.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var cfgSvc = new ConfigService();
var config = cfgSvc.Load();

var arg = (args.Length > 0 ? args[0] : "").ToLower();

if (arg == "--install") { Install(); return 0; }
if (arg == "setup") { SetupWizard(cfgSvc); return 0; }
if (arg == "help") { Help(); return 0; }
if (arg == "--repl") { /* already in spawned window, continue to REPL */ }
else if (arg.Length == 0)
{
    // Spawn a new window for the interactive REPL
    var exe = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName;
    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/k \"\"{exe}\" --repl\"")
    {
        UseShellExecute = true,
        WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
    });
    return 0;
}

// ── REPL mode ──
Banner();
var cmdSvc = new CommandService();

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("dismagent> ");
    Console.ResetColor();
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) continue;
    if (input is "exit" or "quit") break;

    if (input == "help") { Help(); continue; }
    if (input == "dismagent setup") { SetupWizard(cfgSvc); config = cfgSvc.Load(); continue; }
    if (input == "dismagent config") { ShowConfig(config); continue; }
    if (input.StartsWith("dismagent ")) { Console.WriteLine($"Unknown: {input}"); continue; }

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n◆ AI Analysis ─────");
    Console.ResetColor();
    try
    {
        var ai = new AiService(config);
        Console.WriteLine(await ai.Explain(input));
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[!] {ex.Message}");
        Console.ResetColor();
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("\nExecute? (y/n): ");
    Console.ResetColor();
    if (Console.ReadLine()?.Trim().ToLower() == "y")
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("─── Output ───");
        Console.ResetColor();
        Console.WriteLine(cmdSvc.Execute(input));
    }
    Console.WriteLine();
}
return 0;

// ── Helper methods ──

static void Banner()
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(@"
  ██████  ██▓  ██████  ███▄ ▄███▓
▒██    ▒ ▓██▒▒██    ▒ ▓██▒▀█▀ ██▒
░ ▓██▄   ▒██▒░ ▓██▄   ▓██    ▓██░
  ▒   ██▒░██░  ▒   ██▒▒██    ▒██ 
▒██████▒▒░██░▒██████▒▒▒██▒   ░██▒
▒ ▒▓▒ ▒ ░░▓  ▒ ▒▓▒ ▒ ░░ ▒░   ░  ░
░ ░▒  ░ ░ ▒ ░░ ░▒  ░ ░░  ░      ░
░  ░  ░   ▒ ░░  ░  ░  ░      ░   
      ░   ░        ░         ░   
");
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("  Dism Agent v1.0 — PowerShell AI Assistant");
    Console.WriteLine("  Type a command, or 'help'\n");
    Console.ResetColor();
}

static void Help()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(@"
  Commands:
    <command>            Analyze & optionally execute a PowerShell command
    dismagent setup      Configure AI provider & search
    dismagent config     Show current config
    help                 This help
    exit / quit          Exit

  CLI flags:
    dismagent --install  Install Dism Agent into system
    dismagent setup      Setup wizard
");
    Console.ResetColor();
}

static void ShowConfig(AppConfig c)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n  Config:");
    Console.ResetColor();
    Console.WriteLine($"  Provider : {c.AiProvider}");
    Console.WriteLine($"  Ollama   : {c.OllamaEndpoint} ({c.OllamaModel})");
    foreach (var k in new[] { "Kimi", "GLM", "DeepSeek", "Doubao", "Mimo" })
        Console.WriteLine($"  {k,-9}: {(GetApiKey(c, k.ToLower()) != "" ? "****" : "not set")}");
    Console.WriteLine($"  Search   : {c.SearchProvider}");
    Console.WriteLine($"  File     : {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DismAgent", "config.json")}\n");
}

static string GetApiKey(AppConfig c, string p) => p switch
{
    "kimi" => c.KimiApiKey, "glm" => c.GlmApiKey, "deepseek" => c.DeepSeekApiKey,
    "doubao" => c.DoubaoApiKey, "mimo" => c.MimoApiKey, _ => ""
};

static void SetupWizard(ConfigService cs)
{
    var cfg = cs.Load();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("\n╔══════════════════════════════════════╗");
    Console.WriteLine("║       Dism Agent Setup Wizard        ║");
    Console.WriteLine("╚══════════════════════════════════════╝\n");
    Console.ResetColor();

    // AI Provider
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("Select AI Provider:");
    Console.ResetColor();
    var providers = new[] { "ollama (local)", "kimi (moonshot)", "glm (zhipu)", "deepseek", "doubao (bytedance)", "mimo" };
    for (int i = 0; i < providers.Length; i++)
        Console.WriteLine($"  {i + 1}. {providers[i]}");
    Console.Write("Choice (1-6) [1]: ");
    var choice = Console.ReadLine()?.Trim();
    var idx = int.TryParse(choice, out var n) && n >= 1 && n <= 6 ? n - 1 : 0;
    var key = providers[idx].Split(' ')[0];
    cfg.AiProvider = key;

    if (key == "ollama")
    {
        Console.Write("Endpoint [http://localhost:11434]: ");
        var e = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(e)) cfg.OllamaEndpoint = e;
        Console.Write("Model [llama3]: ");
        var m = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(m)) cfg.OllamaModel = m;
        // Auto-pull
        Console.Write($"Pull Ollama model '{cfg.OllamaModel}'? (Y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "n")
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\nDownloading {cfg.OllamaModel}...\n");
            Console.ResetColor();
            try
            {
                using var proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "ollama";
                proc.StartInfo.Arguments = $"pull {cfg.OllamaModel}";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.OutputDataReceived += (_, e) => { if (e.Data != null) ShowPullProgress(e.Data); };
                proc.ErrorDataReceived += (_, e) => { if (e.Data != null) ShowPullProgress(e.Data); };
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✓ Download complete");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] ollama not found: {ex.Message}");
                Console.ResetColor();
            }
        }
        Tutorial("Ollama", "1. Install from ollama.ai\n2. Run: ollama serve");
    }
    else
    {
        Console.Write("API Key: ");
        var ak = Console.ReadLine()?.Trim() ?? "";
        SetApiKey(cfg, key, ak);
        Tutorial(char.ToUpper(key[0]) + key[1..], key switch
        {
            "kimi" => "1. Go to platform.moonshot.cn\n2. Create API key",
            "glm" => "1. Go to open.bigmodel.cn\n2. Generate API key",
            "deepseek" => "1. Go to platform.deepseek.com\n2. Create API key",
            "doubao" => "1. Go to console.volcengine.com\n2. Enable ARK & create key",
            "mimo" => "1. Go to mimo.ai\n2. Generate API key",
            _ => "See provider docs for API key"
        });
    }

    // Search
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\nSearch Provider:");
    Console.ResetColor();
    Console.WriteLine("  1. Google Custom Search");
    Console.WriteLine("  2. DuckDuckGo (no key)");
    Console.WriteLine("  3. None");
    Console.Write("Choice (1-3) [3]: ");
    var sc = Console.ReadLine()?.Trim();
    cfg.SearchProvider = sc switch { "1" => "google", "2" => "duckduckgo", _ => "none" };

    if (cfg.SearchProvider == "google")
    {
        Console.Write("Google API Key: ");
        cfg.GoogleApiKey = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Search Engine ID (cx): ");
        cfg.GoogleCx = Console.ReadLine()?.Trim() ?? "";
        Tutorial("Google Search", "1. Go to console.cloud.google.com\n2. Enable Custom Search API\n3. Create API key\n4. Go to cse.google.com → get cx");
    }

    cs.Save(cfg);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\n✓ Configuration saved!\n");
    Console.ResetColor();
}

static void SetApiKey(AppConfig c, string p, string k)
{
    switch (p) { case "kimi": c.KimiApiKey = k; break; case "glm": c.GlmApiKey = k; break;
        case "deepseek": c.DeepSeekApiKey = k; break; case "doubao": c.DoubaoApiKey = k; break;
        case "mimo": c.MimoApiKey = k; break; }
}

static void Tutorial(string name, string steps)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"\n── {name} Setup Guide ──");
    foreach (var line in steps.Split('\n'))
    {
        foreach (var c in line) { Console.Write(c); Thread.Sleep(8); }
        Console.WriteLine();
    }
    Console.ResetColor();
    Thread.Sleep(300);
}

static void Install()
{
    var src = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName;
    var targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DismAgent");
    Directory.CreateDirectory(targetDir);
    var targetExe = Path.Combine(targetDir, "dismagent.exe");

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("\nInstalling Dism Agent...");
    Console.ResetColor();

    // Create default config
    var cfgSvcLocal = new ConfigService();
    var existing = cfgSvcLocal.Load();
    cfgSvcLocal.Save(existing);

    // Copy self
    try { File.Copy(src, targetExe, true); }
    catch (Exception ex) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"[!] Copy failed: {ex.Message}"); Console.ResetColor(); return; }

    // Add to PATH
    try
    {
        var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? "";
        if (!path.Contains(targetDir))
        {
            var newPath = path + ";" + targetDir;
            Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Added to PATH (user)");
            Console.ResetColor();
        }
    }
    catch (Exception ex) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"[!] PATH update failed: {ex.Message}"); Console.ResetColor(); }

    // Copy setup too
    var setupSrc = Path.Combine(Path.GetDirectoryName(src)!, "setup.exe");
    if (!File.Exists(setupSrc))
        setupSrc = Path.Combine(Path.GetDirectoryName(src)!, "dismagent-setup.exe");
    if (File.Exists(setupSrc))
    {
        var setupTarget = Path.Combine(targetDir, "setup.exe");
        File.Copy(setupSrc, setupTarget, true);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ setup.exe copied");
        Console.ResetColor();
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\n✓ Dism Agent installed!");
    Console.WriteLine($"  Location: {targetDir}");
    Console.WriteLine("  Type 'dismagent' in a new terminal to start.");
    Console.ResetColor();
}

static void ShowPullProgress(string line)
{
    // Ollama output: "pulling xxx... 45% ▕████▌  xxx MB  5s"
    var match = System.Text.RegularExpressions.Regex.Match(line, @"(\d+)%.*?(\d+\.?\d*)\s*(GB|MB|KB)/s.*?(\d+)s");
    if (match.Success)
    {
        var pct = match.Groups[1].Value;
        var speed = $"{match.Groups[2].Value} {match.Groups[3].Value}/s";
        var eta = match.Groups[4].Value;
        var barWidth = 30;
        var filled = int.Parse(pct) * barWidth / 100;
        var bar = new string('█', filled) + new string(' ', barWidth - filled);
        Console.Write($"\r  Pulling... {pct}% ▕{bar}▏ {speed}  ETA: {eta}s  ");
    }
    else if (line.Contains("pulling manifest") || line.Contains("verifying") || line.Contains("writing manifest"))
    {
        Console.WriteLine($"\r{line}");
    }
    else if (line.Contains("success") || line.Contains("already"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\r✓ {line}");
        Console.ResetColor();
    }
    else if (!string.IsNullOrWhiteSpace(line))
    {
        Console.WriteLine($"\r{line}");
    }
}
