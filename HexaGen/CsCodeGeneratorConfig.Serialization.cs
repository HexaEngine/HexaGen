namespace HexaGen
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class CsCodeGeneratorConfig
    {
        public static readonly JsonSerializerSettings SerializerSettings = new()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented,
        };

        public static readonly JsonSerializer Serializer = JsonSerializer.Create(SerializerSettings);

        public static readonly JsonSerializerSettings MergeSerializerSettings = new()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };

        public static readonly JsonSerializer MergeSerializer = JsonSerializer.Create(MergeSerializerSettings);

        public static CsCodeGeneratorConfig Load(string file, IConfigComposer? composer = null)
        {
            CsCodeGeneratorConfig result;
            if (File.Exists(file))
            {
                result = JsonConvert.DeserializeObject<CsCodeGeneratorConfig>(File.ReadAllText(file)) ?? new();
            }
            else
            {
                result = new();
            }

            result.Save(file);

            composer ??= new ConfigComposer();
            composer.Compose(ref result);

            return result;
        }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, SerializerSettings));
        }
    }
}