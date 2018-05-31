using elearning.model.DataModels;
using MVCCodeGenerator.Model;
using MVCCodeGenerator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MVCCodeGenerator
{
    public class EmbeddedCodeGenerator : ABasicGenerator
    {
        private InputEmbedded _input;
        private string _searchFieldsQuery;
        private string _editFields;
        private Dictionary<string, string> _mappingModels;

        public EmbeddedCodeGenerator(InputEmbedded input)
        {
            _input = input;
            _mappingModels = new Dictionary<string, string>();
        }

        public void Run()
        {
            LogAndDisplay("Code generation started at " + DateTime.Now);

            Assembly assembly = Assembly.Load(_input.AssemblyName);           

            foreach (Type type in assembly.GetTypes())
            {
                if (!type.Name.Equals(_input.TargetModelName, StringComparison.CurrentCultureIgnoreCase)
                    || !type.Namespace.Equals(_input.TargetModelNameSpace, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                
                Console.WriteLine(string.Format("Type Found it Class: {0} -- Namespace {1} -- Beginning process.........", type.Name, type.Namespace));
                
                //build viewmodels
                BuildViewModels(type);
                // build services
                BuildServices();
                //build controllers
                BuildControllers();
                //build views              
                BuildViews();
                //inject
                DoAllInjections();
                //do mappings
                string s = string.Join(";" + Environment.NewLine, _mappingModels.Select(x => 
                    string.Format("CreateMap<{0}, {1}>()", x.Key , x.Value)).ToArray()) + (_mappingModels.Count > 1? ";" + Environment.NewLine : "");
                DoAllMappings(s);

                LogAndDisplay("Code generation completed at " + DateTime.Now);
            } 
        }

        private void DoAllInjections()
        {
            LogAndDisplay("Starting all injections");

            var searchString = @"/***Dependency_Injection***/";
            var content = FileUtils.ReadStringFromFile(_input.ContainerInjectionPath);
                      
            //mainly services injection.
            var injectionString = string.Format("builder.RegisterType<{0}>().As<I{0}>();", _input.TargetModelName + "Service");

            if (content.Contains(injectionString))
            {
                LogAndDisplay("Type already registered " + injectionString);
                return;
            }

            var code = content.Replace(searchString, Environment.NewLine + injectionString + Environment.NewLine + searchString);           

            FileUtils.CreateFileOrOverwrite(_input.ContainerInjectionPath, code);

            LogAndDisplay("Injections completed " + injectionString);
        }

        private void DoAllMappings(string mappingString)
        {
            LogAndDisplay("Starting all mappings");

            var searchString = @"/***Mapping_Injection***/";
            var content = FileUtils.ReadStringFromFile(_input.MappingProfilePath);

            if (content.Contains(mappingString))
            {
                LogAndDisplay("Type already registered " + mappingString);
                return;
            }

            var code = content.Replace(searchString, Environment.NewLine +  mappingString + Environment.NewLine + searchString);

            FileUtils.CreateFileOrOverwrite(_input.MappingProfilePath, code);

            LogAndDisplay("Mapping completed " + mappingString);
        }

        private void BuildControllers()
        {
            var sbModelCode = new StringBuilder(Heading);
            var sbTempAttribs = new StringBuilder();
            var controllerName = _input.TargetModelName + "Controller.cs";
            var templateStr = FileUtils.ReadStringFromFile(_input.ControllerPath + "\\TemplateController.txt");
            var code = templateStr.Replace("[***]", _input.TargetModelName);
            var controllerFullPath = _input.ControllerPath + "\\" + controllerName;
            sbModelCode.AppendLine(code);

            var bModelExist = File.Exists(controllerFullPath);
            FileUtils.CreateFileOrOverwrite(controllerFullPath, sbModelCode.ToString());

            if (!bModelExist)
            {
                AddFileToProject(_input.ControllerProjectPath + "\\" + _input.ControllerProjectName, controllerFullPath.Replace(_input.ControllerProjectPath + "\\", ""));
            }
        }

        private void BuildViews()
        {
            var sbTemp = new StringBuilder();
            var editModelName = _input.TargetModelName + "EditVm";
            var viewModelName = editModelName + ".cs";

            var createTemplateStr = FileUtils.ReadStringFromFile(_input.ViewsPath + "\\Template\\Create.txt");
            var editTemplateStr = FileUtils.ReadStringFromFile(_input.ViewsPath + "\\Template\\Details.txt");
            var listTemplateStr = FileUtils.ReadStringFromFile(_input.ViewsPath + "\\Template\\Index.txt");

            var targetCreateViewPath = _input.ViewsPath + "\\" + _input.TargetModelName + "\\Create.cshtml";
            var targetEditViewPath = _input.ViewsPath + "\\" + _input.TargetModelName + "\\Details.cshtml";
            var targetListViewPath = _input.ViewsPath + "\\" + _input.TargetModelName + "\\Index.cshtml";

            var createTemplateCode = createTemplateStr.Replace("[***]", editModelName).Replace("[***Model***]", _input.TargetModelName);
            var editTemplateCode = editTemplateStr.Replace("[***]", editModelName).Replace("[***Model***]", _input.TargetModelName);
            var listTemplateCode = listTemplateStr.Replace("[***]", _input.TargetModelName);


            //var bModelExist = File.Exists(viewModelFullPath);
           
            var createResult = FileUtils.CreateFileOrOverwrite(targetCreateViewPath, createTemplateCode);
            var editResult = FileUtils.CreateFileOrOverwrite(targetEditViewPath, editTemplateCode);
            var listResult = FileUtils.CreateFileOrOverwrite(targetListViewPath, listTemplateCode);

            AddFileToProject(_input.ViewsProjectPath, targetCreateViewPath.Replace(_input.ViewsPath + "\\", "\\Views"));
            AddFileToProject(_input.ViewsProjectPath, targetEditViewPath.Replace(_input.ViewsPath + "\\", "\\Views"));
            AddFileToProject(_input.ViewsProjectPath, targetListViewPath.Replace(_input.ViewsPath + "\\", "\\Views"));

        }

        private void BuildViewModels(Type t)
        {
            var sbModelCode = new StringBuilder(Heading);
            var sbTemp = new StringBuilder();
            var sbTempAttribs = new StringBuilder();
            var sbSearchField = new StringBuilder();

            var viewModelName = _input.TargetModelName + "EditVm.cs";
            var templateFileFullPath = _input.ViewModelPath + "\\TemplateEditVm.txt";
            var templateStr = FileUtils.ReadStringFromFile(templateFileFullPath);
            var viewModelFullPath = _input.ViewModelPath + "\\" + viewModelName;

            //object obj = Activator.CreateInstance(type);
            var properties = t.GetProperties();

            //this is for later mappings
            _mappingModels.Add(_input.TargetModelName, _input.TargetModelName + "EditVm");
            _mappingModels.Add(_input.TargetModelName + "EditVm", _input.TargetModelName);

            //get searchable fields query
            _searchFieldsQuery = GetSearchableFieldQuery(properties);
            _editFields = GetAllEditFields(properties, _input.TargetModelName, "model");

            //build EditViewModel properties
            foreach (var prop in properties)
            {
                var key = PropertyIsAcceptable(prop);

                if(string.IsNullOrEmpty(key))  continue;

                var propReturn = string.Empty;
                var propTypeName = prop.PropertyType.Name;
                var attribs = GetPropertyAttributes(prop);                

                sbTemp.Append(attribs);
                sbTemp.AppendLine(" public " + key + " " + prop.Name + "{get;set;}");
                LogAndDisplay("prop => public " + key + " " + prop.Name + "{get;set;}");
            }

            //*** inject using/import statements from source data file
            var code = templateStr.Replace("[***]", _input.TargetModelName).Replace("[***code***]", sbTemp.ToString());
            sbModelCode.AppendLine(code);
            var bModelExist = File.Exists(viewModelFullPath);
            //create file
            FileUtils.CreateFileOrOverwrite(viewModelFullPath, sbModelCode.ToString());
            //add file to project           
            if (!bModelExist)
            {
                var projectPath = _input.ViewModelProjectPath + "\\" + _input.ViewModelProjectName;
                AddFileToProject(projectPath, viewModelFullPath.Replace(_input.ViewModelProjectPath + "\\", ""));
            }
        }              

        private void BuildServices()
        {
            Console.WriteLine("Starting to build services....");

            var interfacePath = _input.ServicesPath + "\\Interfaces";
            var implementationPath = _input.ServicesPath + "\\Implementation";

            var interfaceStr = FileUtils.ReadStringFromFile(interfacePath + "\\ITemplateService.txt");
            var implementationStr = FileUtils.ReadStringFromFile(implementationPath + "\\TemplateService.txt");

            var newInterfaceFullPath = interfacePath + "\\I" + _input.TargetModelName + "Service.cs";
            var newImplementaitonFullPath = implementationPath + "\\" + _input.TargetModelName + "Service.cs";
            
            var sbInterface = new StringBuilder(Heading + interfaceStr);
            var sbImplementation = new StringBuilder(Heading + implementationStr);
            var bInterfaceExist = File.Exists(newInterfaceFullPath);
            var bImplementationExist = File.Exists(newImplementaitonFullPath);

            sbInterface.Replace("[***]", _input.TargetModelName);
            sbImplementation.Replace("[***]", _input.TargetModelName)
                .Replace("[***SearchQuery***]", _searchFieldsQuery)
                .Replace("[***AllEdit***]", _editFields);

            //back-up destination files if they exists
            //log all file activities
            //inject services
            //do mapping profiles for models in model building

            Console.WriteLine("Creating target files");

            FileUtils.CreateFileOrOverwrite(newInterfaceFullPath, sbInterface.ToString());
            FileUtils.CreateFileOrOverwrite(newImplementaitonFullPath, sbImplementation.ToString());

            Console.WriteLine("Build services succeeded with paths:");
            Console.WriteLine(newInterfaceFullPath);
            Console.WriteLine(newImplementaitonFullPath);

            //check/make sure that items have been added to project (check for double entry in project)
            if (!bInterfaceExist || !bImplementationExist)
            {
                var projectPath = _input.ServicesPath + "\\" + _input.ServicesProjectName;
                var p = new Microsoft.Build.Evaluation.Project(projectPath);
                AddFileToProject(p, newInterfaceFullPath.Replace(_input.ServicesProjectName + "\\", ""));
                AddFileToProject(p, newImplementaitonFullPath.Replace(_input.ServicesProjectName + "\\", ""));
            }

        }

        private void AddFileToProject(string projectPath, string filePath)
        {
            try
            {
                LogAndDisplay("Adding new files to project: " + projectPath + Environment.NewLine + filePath);

                var p = new Microsoft.Build.Evaluation.Project(projectPath);
                p.AddItem("Compile", filePath);
                p.Save();
            }
            catch(Exception ex)
            {
                LogAndDisplay("Failed to add file to project:" + Environment.NewLine + projectPath + Environment.NewLine + filePath);
                LogError(ex.Message);
            }
        }

        private void AddFileToProject(Microsoft.Build.Evaluation.Project project, string filePath)
        {
            try
            {
                LogAndDisplay("Adding new files to project: " + project.DirectoryPath + Environment.NewLine + filePath);

                project.AddItem("Compile", filePath);
                project.Save();
            }
            catch(Exception ex)
            {
                LogAndDisplay("Failed to add file to project:" + Environment.NewLine + project.DirectoryPath + Environment.NewLine + filePath);
                LogError(ex.Message);
            }
}
    }
}
