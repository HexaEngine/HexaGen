namespace HexaGen.Cpp2C
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public partial class Cpp2CCodeGeneratorConfig
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

        public static Cpp2CCodeGeneratorConfig Load(string file, IConfigComposer? composer = null)
        {
            Cpp2CCodeGeneratorConfig result;
            if (File.Exists(file))
            {
                result = JsonConvert.DeserializeObject<Cpp2CCodeGeneratorConfig>(File.ReadAllText(file)) ?? new();
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
