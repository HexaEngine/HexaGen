using CppAst;

namespace HexaGen
{
    public partial class CsCodeGeneratorSettings
    {
        public static int IndexOfUnionField(CppClass parent, CppClass union)
        {
            for (int i = 0; i < parent.Fields.Count; i++)
            {
                var field = parent.Fields[i];
                if (field.Type == union)
                    return i;
            }
            return -1;
        }

        public string GetCsSubTypeName(CppClass parentClass, string parentCsName, CppClass subClass, int idxSubClass)
        {
            string csSubName;
            if (string.IsNullOrEmpty(subClass.Name))
            {
                idxSubClass = IndexOfUnionField(parentClass, subClass);
                if (idxSubClass != -1)
                {
                    var field = parentClass.Fields[idxSubClass];
                    if (field.Type == subClass)
                    {
                        string csFieldName = GetFieldName(field.Name);

                        if (string.IsNullOrEmpty(csFieldName))
                        {
                            string label = parentClass.Classes.Count == 1 ? "" : idxSubClass.ToString();
                            csSubName = parentCsName + "Union" + label;
                            return csSubName;
                        }

                        csSubName = csFieldName + "Union";
                    }
                    else
                    {
                        string label = parentClass.Classes.Count == 1 ? "" : idxSubClass.ToString();
                        csSubName = parentCsName + "Union" + label;
                    }
                }
                else
                {
                    string label = parentClass.Classes.Count == 1 ? "" : idxSubClass.ToString();
                    csSubName = parentCsName + "Union" + label;
                }
            }
            else
            {
                csSubName = GetCsCleanName(subClass.Name);
            }

            if (parentClass.Fields.Any(x => x.Name == csSubName))
            {
                csSubName = parentCsName + csSubName;
            }

            return csSubName;
        }
    }
}