namespace HexaGen
{
    using System;
    using System.Text.Json;

    public class BaseConfig
    {
        public string? Url { get; set; }

        public MergeOptions MergeOptions { get; set; } = MergeOptions.All;
    }

    public class ConfigComposer : LoggerBase
    {
        private const string FileProtocol = "file://";
        private const string HttpProtocol = "http://";
        private const string HttpsProtocol = "https://";

        public void Compose(CsCodeGeneratorConfig config)
        {
            if (config.BaseConfig.Url == null)
            {
                return;
            }

            var baseConfig = LoadBaseConfig(config.BaseConfig);

            Compose(baseConfig); // Recursively load base configs

            config.Merge(baseConfig, config.BaseConfig.MergeOptions);
        }

        private CsCodeGeneratorConfig LoadBaseConfig(BaseConfig baseConfig)
        {
            CsCodeGeneratorConfig? baseGeneratorConfig = null;

            ReadOnlySpan<char> url = baseConfig.Url;

            if (url.StartsWith(FileProtocol))
            {
                var path = url[FileProtocol.Length..].ToString();
                if (!File.Exists(path))
                {
                    LogCritical($"File not found: {path}");
                    throw new FileNotFoundException($"File not found: {path}");
                }

                baseGeneratorConfig = CsCodeGeneratorConfig.Load(path);
            }
            if (url.StartsWith(HttpProtocol))
            {
                LogWarn("Config Composer: HTTP is not secure, consider using HTTPS");
                baseGeneratorConfig = DownloadConfig(url);
            }
            if (url.StartsWith(HttpsProtocol))
            {
                baseGeneratorConfig = DownloadConfig(url);
            }

            if (baseGeneratorConfig == null)
            {
                LogCritical($"Invalid URL: {url}");
                throw new Exception($"Invalid URL: {url}");
            }

            return baseGeneratorConfig;
        }

        private CsCodeGeneratorConfig? DownloadConfig(ReadOnlySpan<char> url)
        {
            CsCodeGeneratorConfig? baseGeneratorConfig;
            var uri = new Uri(url.ToString());
            var client = new HttpClient();
            var response = client.GetAsync(uri).Result;
            if (!response.IsSuccessStatusCode)
            {
                LogCritical($"Failed to download config from {url}");
                throw new Exception($"Failed to download config from {url}");
            }

            var json = response.Content.ReadAsStringAsync().Result;
            baseGeneratorConfig = JsonSerializer.Deserialize<CsCodeGeneratorConfig>(json);
            return baseGeneratorConfig;
        }
    }
}