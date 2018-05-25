using MVCCodeGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MVCCodeGenerator
{
    public abstract class ABasicGenerator
    {
        private string _logFilePath;
        private string _header = @"
        /*****************************************************************
        * Code Generated at {0}
        * By Code MVCCodeGenerator
        *
        *
        ******************************************************************/
        ";

        public virtual List<KeyValuePair<string, Type>> AcceptableVmProperties
        {
            get
            {
                return new List<KeyValuePair<string, Type>>
                {
                    new KeyValuePair<string, Type>("string", typeof(string)),
                    new KeyValuePair<string, Type>("int", typeof(int)),
                    new KeyValuePair<string, Type>("DateTime", typeof(DateTime)),
                    new KeyValuePair<string, Type>("bool", typeof(bool)),
                    new KeyValuePair<string, Type>("decimal", typeof(decimal)),
                    new KeyValuePair<string, Type>("Guid", typeof(Guid)),
                    new KeyValuePair<string, Type>("Enum", typeof(Enum))
                };
            }
        }

        public virtual List<string> ExcludedAttributeVm
        {
            get
            {
                return new List<string>{
                    "Key",
                    "DatabaseGenerated",
                    "SearchableCG",
                    "EditableCG"
                };
            }
        }

        public string Heading
        {
            get
            {
               return string.Format(_header, DateTime.Now.ToString()) + Environment.NewLine;
            }
        }

        public ABasicGenerator()
        {
            _logFilePath = ConfigurationManager.AppSettings["logpath"] + "\\" + DateTime.Now.ToString("dd-MM-YYYY") + ".txt";
        }

        public virtual void LogItem(string message)
        {
            FileUtils.AppendToFile(_logFilePath, message);
        }

        public virtual void LogAndDisplay(string message)
        {
            FileUtils.AppendToFile(_logFilePath, message);
            Console.WriteLine(message);
        }

        public Dictionary<string, object> GetPropertyAttributeList(PropertyInfo property)
        {
            Dictionary<string, object> attribs = new Dictionary<string, object>();

            // look for attributes that takes one constructor argument
            foreach (CustomAttributeData attribData in property.GetCustomAttributesData())
            {
                if (attribData.ConstructorArguments.Count == 1)
                {
                    string typeName = attribData.Constructor.DeclaringType.Name;
                    if (typeName.EndsWith("Attribute")) typeName = typeName.Substring(0, typeName.Length - 9);
                    attribs[typeName] = attribData.ConstructorArguments[0].Value;
                }
            }
            return attribs;
        }

        public virtual string GetPropertyAttributes(PropertyInfo property)
        {
            var attribs = new StringBuilder();

            // look for attributes that takes one constructor argument
            foreach (CustomAttributeData attribData in property.GetCustomAttributesData())
            {
                var typeName = attribData.Constructor.DeclaringType.Name;
                if (typeName.EndsWith("Attribute"))
                    typeName = typeName.Substring(0, typeName.Length - 9);
                else
                    continue;

                //we will exclude some of the db attributes
                if (ExcludedAttributeVm.Contains(typeName))
                    continue;

                var errorMsg = string.Empty;

                //error message for required and length attributes
                if (typeName.ToLower().Equals("required"))
                {
                    errorMsg = @"ErrorMessage=""" + property.Name + @" is required!"" ";
                }

                if (attribData.ConstructorArguments.Count > 0)
                {   
                    var temp = string.Empty;
                    var bDoesErrorMessageExist = false;

                    foreach(var args in attribData.ConstructorArguments)
                    {
                        temp += !string.IsNullOrEmpty(temp) ? "," + args.Value : args.Value;
                    }

                    bDoesErrorMessageExist = temp.Contains("ErrorMessage");
                    //append the attrib with its params
                    attribs.AppendLine(string.Format("[{0}({1})]", typeName, temp + (bDoesErrorMessageExist ? "" : errorMsg)));                    
                }
                else
                {
                    attribs.AppendLine(string.Format("[{0}({1})]", typeName, errorMsg));
                }
            }
            return attribs.ToString();
        }

        public virtual string GetSearchableFieldQuery(PropertyInfo[] propertyInfos)
        {
            var sbTemp = new StringBuilder();

            foreach (var prop in propertyInfos)
            {
                var searchableAttrib = prop.GetCustomAttributesData().FirstOrDefault(x => 
                        x.Constructor.DeclaringType.Name.ToLower().StartsWith("searchablecg"));

                if (searchableAttrib == null)
                    continue;

                 if (sbTemp.Length > 4)
                    sbTemp.AppendLine(" || ");

                if(prop.PropertyType == typeof(string))
                    sbTemp.Append(string.Format(" ac.{0}.ToLower().Contains(key) ", prop.Name));
                else
                    sbTemp.Append(string.Format(" ac.{0} == key ", prop.Name));              
              
                
            }
            if (sbTemp.Length > 4)
                {
                    sbTemp.Insert(0, "." + Environment.NewLine + " Where(ac => ");
                    sbTemp.AppendLine(")");
                }
                else
                {
                    sbTemp.Append(";");
                }
            return sbTemp.ToString();
        }

        public virtual string GetAllEditFields(PropertyInfo[] propertyInfos, string className, string modelName)
        {
            var sbTemp = new StringBuilder();
            
            foreach (var prop in propertyInfos)
            {
                var searchableAttrib = prop.GetCustomAttributesData().FirstOrDefault(x =>
                        x.Constructor.DeclaringType.Name.ToLower().StartsWith("editablecg"));

                if (searchableAttrib == null)
                    continue;                
               
                sbTemp.AppendLine(string.Format(" {0}.{1} = {2}.{1};", className, prop.Name, modelName));                            
            }
          
            return sbTemp.ToString();
        }

        public virtual string PropertyIsAcceptable(PropertyInfo info)
        {
            var entry = AcceptableVmProperties.FirstOrDefault(x => x.Value == info.PropertyType);
            return entry.Key; // AcceptableVmProperties.Contains(info.PropertyType);
        }
        
    }
}
