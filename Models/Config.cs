namespace DismAgent.Models;

public class AppConfig
{
    public string AiProvider { get; set; } = "ollama";
    public string OllamaEndpoint { get; set; } = "http://localhost:11434";
    public string OllamaModel { get; set; } = "llama3";
    public string KimiApiKey { get; set; } = "";
    public string GlmApiKey { get; set; } = "";
    public string DeepSeekApiKey { get; set; } = "";
    public string DoubaoApiKey { get; set; } = "";
    public string MimoApiKey { get; set; } = "";
    public string SearchProvider { get; set; } = "none";
    public string GoogleApiKey { get; set; } = "";
    public string GoogleCx { get; set; } = "";
}
