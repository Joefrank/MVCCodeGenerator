using elearning.model.DataModels;
using MVCCodeGenerator.Model;
using MVCCodeGenerator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MVCCodeGenerator
{
    public class EmbeddedCodeGenerator : ABasicGenerator
    {
        private InputEmbedded _input;        
               
        public EmbeddedCodeGenerator(InputEmbedded input)
        {
            _input = input;
        }

        public void Run()
        {
            Assembly assembly = Assembly.Load(_input.AssemblyName);           

            foreach (Type type in assembly.GetTypes())
            {
                if (!type.Name.Equals(_input.TargetModelName, StringComparison.CurrentCultureIgnoreCase)
                    || !type.Namespace.Equals(_input.TargetModelNameSpace, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                
                Console.WriteLine(string.Format("Type Found it Class: {0} -- Namespace {1} -- Beginning process.........", type.Name, type.Namespace));

                BuildViewModels(type);
                // build services
                //BuildServices();

                //build viewmodels

                //build controllers

                //build views              
                       
                // inject

            //do mappings

            } 
        }

        private void BuildViewModels(Type t)
        {
            var sbModelCode = new StringBuilder(Heading);
            var sbTemp = new StringBuilder();

            var viewModelName = _input.TargetModelName + "EditVm.cs";
            var templateFileFullPath = _input.ViewModelPath + "\\TemplateEditVm.txt";
            var templateStr = FileUtils.ReadStringFromFile(templateFileFullPath);
            var viewModelFullPath = _input.ViewModelPath + "\\" + viewModelName;

            //object obj = Activator.CreateInstance(type);
            var properties = t.GetProperties();

            //build properties

            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(string))
                {
                    string value = prop.Name;
                }

                sbTemp.AppendLine(" public " + prop.PropertyType.Name + " " + prop.Name + "{get;set;}");
                LogAndDisplay("prop => public " + prop.PropertyType.Name + " " + prop.Name + "{get;set;}");
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
            sbImplementation.Replace("[***]", _input.TargetModelName);

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

            //check/make sure that items have been added to project
            if (!bInterfaceExist || !bImplementationExist)
            {
                var projectPath = _input.ServicesPath + "\\" + _input.ServicesProjectName;

                AddFileToProject(projectPath, newInterfaceFullPath.Replace(_input.ServicesProjectName + "\\", ""));
                AddFileToProject(projectPath, newImplementaitonFullPath.Replace(_input.ServicesProjectName + "\\", ""));
            }

        }

        private void AddFileToProject(string projectPath, string filePath)
        {
           LogAndDisplay("Adding new files to project: " + projectPath + Environment.NewLine + filePath);

            var p = new Microsoft.Build.Evaluation.Project(projectPath);
            p.AddItem("Compile", filePath);
            p.Save();
        }
    }
}
