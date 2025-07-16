﻿namespace HexaGen
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;

    public class BaseConfig
    {
        public string? Url { get; set; }

        public HashSet<string> IgnoredProperties { get; set; } = [];
    }

    public class ConfigComposer : LoggerBase, IConfigComposer
    {
        private const string FileProtocol = "file://";
        private const string HttpProtocol = "http://";
        private const string HttpsProtocol = "https://";

        private static readonly JsonMergeSettings mergeSettings = new()
        {
            MergeArrayHandling = MergeArrayHandling.Union,
            MergeNullValueHandling = MergeNullValueHandling.Merge,
            PropertyNameComparison = StringComparison.Ordinal
        };

        public void Compose(ref CsCodeGeneratorConfig config)
        {
            var stack = new Stack<CsCodeGeneratorConfig>();
            var current = config;
            while (true)
            {
                stack.Push(current);
                if (current.BaseConfig?.Url == null)
                    break;
                current = LoadBaseConfig(current.BaseConfig);
            }

            var merged = CsCodeGeneratorConfig.Default;
            while (stack.TryPop(out var top))
            {
                merged = Merge(top, merged);
            }
            CollectionNormalizer.Normalize(merged);
            config = merged;
        }

        private static CsCodeGeneratorConfig Merge(CsCodeGeneratorConfig config, CsCodeGeneratorConfig baseConfig)
        {
            var baseJ = JObject.FromObject(baseConfig, CsCodeGeneratorConfig.MergeSerializer);

            if (config.BaseConfig != null)
            {
                ApplyConstrains(baseJ, config.BaseConfig.IgnoredProperties);
            }

            var overrideJ = JObject.FromObject(config, CsCodeGeneratorConfig.MergeSerializer);

            baseJ.Merge(overrideJ, mergeSettings);

            config = baseJ.ToObject<CsCodeGeneratorConfig>(CsCodeGeneratorConfig.MergeSerializer)!;
            return config;
        }

        private static void ApplyConstrains(JObject baseJ, HashSet<string> ignoredPropPaths)
        {
            baseJ.Remove("BaseConfig");
            foreach (var ignoredPropPath in ignoredPropPaths)
            {
                ReadOnlySpan<char> span = ignoredPropPath.AsSpan().Trim();
                JToken current = baseJ;
                while (!span.IsEmpty)
                {
                    int idx = span.IndexOf('.');
                    if (idx == -1) idx = span.Length;
                    var part = span[..idx].Trim();
                    JToken? token = current[part.ToString()];
                    if (token == null) break;
                    current = token;
                    if (idx == span.Length)
                    {
                        switch (token.Type)
                        {
                            case JTokenType.Object:
                                token.Remove();
                                break;

                            case JTokenType.Array:
                                ((JArray)token).RemoveAll();
                                break;

                            case JTokenType.Property:
                                ((JProperty)token).Remove();
                                break;
                        }
                        break;
                    }
                    span = span[(idx + 1)..];
                }
            }
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

                baseGeneratorConfig = JsonConvert.DeserializeObject<CsCodeGeneratorConfig>(File.ReadAllText(path));
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
            baseGeneratorConfig = JsonConvert.DeserializeObject<CsCodeGeneratorConfig>(json);
            return baseGeneratorConfig;
        }
    }
}