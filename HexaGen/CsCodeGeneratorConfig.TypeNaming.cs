using HexaGen.CppAst.Model.Declarations;
using HexaGen.CppAst.Model.Types;
using HexaGen.Metadata;

namespace HexaGen
{
    public delegate void CustomEnumItemMapperDelegate(CppEnum cppEnum, CppEnumItem cppEnumItem, CsEnumMetadata csEnum, CsEnumItemMetadata csEnumItem);

    public partial class CsCodeGeneratorConfig
    {
        [JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        public CustomEnumItemMapperDelegate? CustomEnumItemMapper { get; set; }

        [JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        internal Dictionary<string, CsEnumMetadata> DefinedCppEnums { get; set; } = []; // set to empty just to make sure, because if enum generation is disabled, this will not be set

        public static int IndexOfAnonymousField(CppClass parent, CppClass union)
        {
            for (int i = 0; i < parent.Fields.Count; i++)
            {
                var field = parent.Fields[i];
                if (field.Type.GetCanonicalRoot(true) == union)
                    return i;
            }
            return -1;
        }

        public string GetCsSubTypeName(CppClass parentClass, string parentCsName, CppClass subClass, int idxSubClass)
        {
            string csSubName;
            if (subClass.IsAnonymous)
            {
                idxSubClass = IndexOfAnonymousField(parentClass, subClass);
                if (idxSubClass != -1)
                {
                    var field = parentClass.Fields[idxSubClass];
                    if (field.Type == subClass)
                    {
                        string csFieldName = GetFieldName(field.Name);

                        if (string.IsNullOrEmpty(csFieldName))
                        {
                            string label = parentClass.Classes.Count == 1 ? "" : idxSubClass.ToString();
                            csSubName = parentCsName + "Anonymous" + label;
                            return csSubName;
                        }

                        csSubName = csFieldName + "Anonymous";
                    }
                    else if (field.Type is CppArrayType)
                    {
                        string csFieldName = GetFieldName(field.Name);
                        if (csFieldName.EndsWith('s'))
                        {
                            csFieldName = csFieldName[..^1];
                        }
                        csSubName = csFieldName;
                    }
                    else
                    {
                        string label = parentClass.Classes.Count == 1 ? "" : idxSubClass.ToString();
                        csSubName = parentCsName + "Anonymous" + label;
                    }
                }
                else
                {
                    string label = parentClass.Classes.Count == 1 ? "" : idxSubClass.ToString();
                    csSubName = parentCsName + "Anonymous" + label;
                }
            }
            else
            {
                csSubName = GetCsCleanName(subClass.Name);
            }

            foreach (var field in parentClass.Fields)
            {
                if (field.Name == csSubName)
                {
                    csSubName = parentCsName + csSubName;
                    break;
                }
            }

            return csSubName;
        }
    }
}