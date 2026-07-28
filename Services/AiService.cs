using System.Text;
using System.Text.Json;
using DismAgent.Models;

namespace DismAgent.Services;

public class AiService
{
    readonly AppConfig _cfg;
    readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(30) };

    static readonly Dictionary<string, (string Endpoint, string Model)> Providers = new()
    {
        ["ollama"]   = ("", ""),
        ["kimi"]     = ("https://api.moonshot.cn/v1", "moonshot-v1-8k"),
        ["glm"]      = ("https://open.bigmodel.cn/api/paas/v4", "glm-4"),
        ["deepseek"] = ("https://api.deepseek.com/v1", "deepseek-chat"),
        ["doubao"]   = ("https://ark.cn-beijing.volces.com/api/v3", "ep-xxxx"),
        ["mimo"]     = ("https://api.mimo.com/v1", "mimo-chat"),
    };

    public AiService(AppConfig c) => _cfg = c;

    public async Task<string> Explain(string cmd)
    {
        var prompt = $"You are a PowerShell expert. Explain this command in Traditional Chinese concisely:\n\nCommand: {cmd}\n\nExplanation:";
        return _cfg.AiProvider == "ollama" ? await CallOllama(prompt) : await CallCloud(prompt);
    }

    async Task<string> CallOllama(string p)
    {
        var ep = _cfg.OllamaEndpoint.TrimEnd('/');
        // Build JSON manually — no reflection
        var body = $"{{\"model\":\"{JsonEscape(_cfg.OllamaModel)}\",\"prompt\":\"{JsonEscape(p)}\",\"stream\":false}}";
        var r = await _http.PostAsync($"{ep}/api/generate",
            new StringContent(body, Encoding.UTF8, "application/json"));
        r.EnsureSuccessStatusCode();
        var text = await r.Content.ReadAsStringAsync();
        // JsonDocument.Parse is read-only, no reflection needed
        return JsonDocument.Parse(text).RootElement.GetProperty("response").GetString() ?? "";
    }

    async Task<string> CallCloud(string p)
    {
        if (!Providers.TryGetValue(_cfg.AiProvider, out var prov)) return $"[ERROR] Unknown provider: {_cfg.AiProvider}";
        var key = _cfg.AiProvider switch
        {
            "kimi" => _cfg.KimiApiKey, "glm" => _cfg.GlmApiKey, "deepseek" => _cfg.DeepSeekApiKey,
            "doubao" => _cfg.DoubaoApiKey, "mimo" => _cfg.MimoApiKey, _ => ""
        };
        if (string.IsNullOrEmpty(key)) return "[ERROR] API key not set. Run: dismagent setup";

        // Build JSON manually — no reflection
        var body = $"{{\"model\":\"{JsonEscape(prov.Model)}\",\"messages\":[{{\"role\":\"user\",\"content\":\"{JsonEscape(p)}\"}}]}}";
        var req = new HttpRequestMessage(HttpMethod.Post, $"{prov.Endpoint}/chat/completions")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        req.Headers.Add("Authorization", $"Bearer {key}");
        var r = await _http.SendAsync(req);
        r.EnsureSuccessStatusCode();
        var text = await r.Content.ReadAsStringAsync();
        // JsonDocument.Parse is read-only, no reflection needed
        return JsonDocument.Parse(text).RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
    }

    static string JsonEscape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
}
