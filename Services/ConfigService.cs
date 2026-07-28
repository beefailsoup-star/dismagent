using System.Text.RegularExpressions;
using DismAgent.Models;

namespace DismAgent.Services;

public class ConfigService
{
    private readonly string _filePath;

    public ConfigService()
    {
        var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DismAgent");
        Directory.CreateDirectory(dir);
        _filePath = System.IO.Path.Combine(dir, "config.json");
    }

    public AppConfig Load()
    {
        if (!File.Exists(_filePath)) return new AppConfig();
        try
        {
            var text = File.ReadAllText(_filePath);
            var c = new AppConfig();
            if (TryGet(text, "AiProvider", out var v)) c.AiProvider = v;
            if (TryGet(text, "OllamaEndpoint", out v)) c.OllamaEndpoint = v;
            if (TryGet(text, "OllamaModel", out v)) c.OllamaModel = v;
            if (TryGet(text, "KimiApiKey", out v)) c.KimiApiKey = v;
            if (TryGet(text, "GlmApiKey", out v)) c.GlmApiKey = v;
            if (TryGet(text, "DeepSeekApiKey", out v)) c.DeepSeekApiKey = v;
            if (TryGet(text, "DoubaoApiKey", out v)) c.DoubaoApiKey = v;
            if (TryGet(text, "MimoApiKey", out v)) c.MimoApiKey = v;
            if (TryGet(text, "SearchProvider", out v)) c.SearchProvider = v;
            if (TryGet(text, "GoogleApiKey", out v)) c.GoogleApiKey = v;
            if (TryGet(text, "GoogleCx", out v)) c.GoogleCx = v;
            return c;
        }
        catch { return new AppConfig(); }
    }

    public void Save(AppConfig c)
    {
        string Q(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        var json = $@"{{
  {Q("AiProvider")}: {Q(c.AiProvider)},
  {Q("OllamaEndpoint")}: {Q(c.OllamaEndpoint)},
  {Q("OllamaModel")}: {Q(c.OllamaModel)},
  {Q("KimiApiKey")}: {Q(c.KimiApiKey)},
  {Q("GlmApiKey")}: {Q(c.GlmApiKey)},
  {Q("DeepSeekApiKey")}: {Q(c.DeepSeekApiKey)},
  {Q("DoubaoApiKey")}: {Q(c.DoubaoApiKey)},
  {Q("MimoApiKey")}: {Q(c.MimoApiKey)},
  {Q("SearchProvider")}: {Q(c.SearchProvider)},
  {Q("GoogleApiKey")}: {Q(c.GoogleApiKey)},
  {Q("GoogleCx")}: {Q(c.GoogleCx)}
}}";
        File.WriteAllText(_filePath, json);
    }

    static bool TryGet(string json, string key, out string val)
    {
        var m = Regex.Match(json, $@"""{key}"":\s*""((?:[^""\\]|\\.)*)""");
        val = m.Success ? m.Groups[1].Value.Replace("\\\"", "\"").Replace("\\\\", "\\") : "";
        return m.Success;
    }

    public string FilePath => _filePath;
}
