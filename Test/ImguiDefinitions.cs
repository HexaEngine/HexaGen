namespace Test
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal class ImguiDefinitions
    {
        public EnumDefinition[] Enums;
        public TypeDefinition[] Types;
        public FunctionDefinition[] Functions;
        public TypedefDefinition[] Typedefs;

        public static readonly List<string> WellKnownEnums = new()
        {
            "ImGuiMouseButton"
        };

        public static readonly Dictionary<string, string> AlternateEnumPrefixes = new()
        {
            { "ImGuiKey", "ImGuiMod" },
        };

        public static readonly Dictionary<string, string> AlternateEnumPrefixSubstitutions = new()
        {
            { "ImGuiMod_", "Mod" },
        };

        public ImguiDefinitions(string directory)
        {
            JObject typesJson;
            using (StreamReader fs = File.OpenText(Path.Combine(directory, "structs_and_enums.json")))
            using (JsonTextReader jr = new(fs))
            {
                typesJson = JObject.Load(jr);
            }

            JObject functionsJson;
            using (StreamReader fs = File.OpenText(Path.Combine(directory, "definitions.json")))
            using (JsonTextReader jr = new(fs))
            {
                functionsJson = JObject.Load(jr);
            }

            JObject typedefsJson;
            using (StreamReader fs = File.OpenText(Path.Combine(directory, "typedefs_dict.json")))
            using (JsonTextReader jr = new(fs))
            {
                typedefsJson = JObject.Load(jr);
            }

            var typeLocations = typesJson["locations"];

            Enums = typesJson["enums"].Select(jt =>
            {
                JProperty jp = (JProperty)jt;
                string name = jp.Name;
                string? comment = typesJson["enum_comments"][name]?["above"]?.ToString();
                EnumMember[] elements = jp.Values().Select(v =>
                {
                    return new EnumMember(v["name"].ToString(), v["calc_value"].ToString(), v["comment"]?.ToString());
                }).ToArray();
                return new EnumDefinition(name, elements, comment);
            }).Where(x => x != null).ToArray();

            Types = typesJson["structs"].Select(jt =>
            {
                JProperty jp = (JProperty)jt;
                string name = jp.Name;
                string? comment = typesJson["struct_comments"][name]?["above"]?.ToString();

                TypeReference[] fields = jp.Values().Select(v =>
                {
                    if (v["type"].ToString().Contains("static")) { return null; }

                    return new TypeReference(
                        v["name"].ToString(),
                        GetComment(v["comment"]),
                        v["type"].ToString(),
                            GetInt(v, "size"),
                        v["template_type"]?.ToString(),
                        Enums);
                }).Where(tr => tr != null).Cast<TypeReference>().ToArray();
                return new TypeDefinition(name, fields, comment);
            }).Where(x => x != null).ToArray();

            Functions = functionsJson.Children().Select(jt =>
            {
                JProperty jp = (JProperty)jt;
                string name = jp.Name;
                bool hasNonUdtVariants = jp.Values().Any(val => val["ov_cimguiname"]?.ToString().EndsWith("nonUDT") ?? false);
                OverloadDefinition[] overloads = jp.Values().Select(val =>
                {
                    string? ov_cimguiname = val["ov_cimguiname"]?.ToString();
                    string cimguiname = val["cimguiname"].ToString();
                    string? friendlyName = val["funcname"]?.ToString();
                    if (cimguiname.EndsWith("_destroy"))
                    {
                        friendlyName = "Destroy";
                    }

                    if (friendlyName == null) { return null; }

                    string? exportedName = ov_cimguiname;
                    exportedName ??= cimguiname;

                    if (hasNonUdtVariants && !exportedName.EndsWith("nonUDT2"))
                    {
                        return null;
                    }

                    string? selfTypeName = null;
                    int underscoreIndex = exportedName.IndexOf('_');
                    if (underscoreIndex > 0 && !exportedName.StartsWith("ig")) // Hack to exclude some weirdly-named non-instance functions.
                    {
                        selfTypeName = exportedName[..underscoreIndex];
                    }

                    List<TypeReference> parameters = new();

                    // find any variants that can be applied to the parameters of this method based on the method name

                    Dictionary<string, string> defaultValues = new();
                    foreach (JToken dv in val["defaults"])
                    {
                        JProperty dvProp = (JProperty)dv;
                        defaultValues.Add(dvProp.Name, dvProp.Value.ToString());
                    }
                    string returnType = val["ret"]?.ToString() ?? "void";
                    string? comment = val["comment"]?.ToString();

                    if (comment != null)
                    {
                        StringBuilder sb = new();
                        sb.AppendLine("/// <summary>");
                        var lines = comment.Replace("/", string.Empty).Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < lines.Length; i++)
                        {
                            sb.AppendLine($"/// {lines[i]}");
                        }
                        sb.AppendLine("/// </summary>");
                        comment = sb.ToString();
                    }

                    string structName = val["stname"].ToString();
                    bool isConstructor = val.Value<bool>("constructor");
                    bool isDestructor = val.Value<bool>("destructor");
                    if (isConstructor)
                    {
                        returnType = structName;
                    }

                    return new OverloadDefinition(
                        exportedName,
                        friendlyName,
                        parameters.ToArray(),
                        defaultValues,
                        returnType,
                        structName,
                        comment,
                        isConstructor,
                        isDestructor);
                }).Where(od => od != null).Cast<OverloadDefinition>().ToArray();
                if (overloads.Length == 0) return null;
                return new FunctionDefinition(name, overloads, Enums);
            }).Where(x => x != null).Cast<FunctionDefinition>().OrderBy(fd => fd.Name).ToArray();

            Typedefs = typedefsJson.Children().Select(jt =>
            {
                JProperty jp = (JProperty)jt;
                string name = jp.Name;
                string value = jp.Value.ToString();

                return new TypedefDefinition(name, value);
            }).ToArray();
        }

        private static int GetInt(JToken token, string key)
        {
            var v = token[key];
            if (v == null) return 0;
            return v.ToObject<int>();
        }

        private static string? GetComment(JToken? token)
        {
            if (token == null)
                return null;
            var above = token["above"]?.ToString();
            var sameline = token["sameline"]?.ToString();
            if (above == null && sameline == null)
                return null;
            if (above == null)
                return sameline;
            if (sameline == null)
                return null;
            return above + sameline;
        }
    }
}