namespace HexaGen.Cpp2C
{
    using Newtonsoft.Json;


    public partial class Cpp2CGeneratorConfig
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

        public static Cpp2CGeneratorConfig Load(string file, IConfigComposer? composer = null)
        {
            Cpp2CGeneratorConfig result;
            if (File.Exists(file))
            {
                result = JsonConvert.DeserializeObject<Cpp2CGeneratorConfig>(File.ReadAllText(file)) ?? new();
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
