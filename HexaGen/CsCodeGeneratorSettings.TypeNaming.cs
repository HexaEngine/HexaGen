using CppAst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HexaGen
{
    public partial class CsCodeGeneratorSettings
    {
        public string GetCsSubTypeName(CppClass parentClass, string parentCsName, CppClass subClass, int idxSubClass)
        {
            string csSubName;
            if (string.IsNullOrEmpty(subClass.Name))
            {
                if (parentClass.Fields.Count > idxSubClass)
                {
                    var field = parentClass.Fields[idxSubClass];
                    if (field.Type == subClass)
                    {
                        string csFieldName = NormalizeFieldName(field.Name);

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
            return csSubName;
        }
    }
}