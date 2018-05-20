using elearning.model.DataModels;
using MVCCodeGenerator.Model;
using MVCCodeGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MVCCodeGenerator
{
    public class EmbeddedCodeGenerator
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

                object obj = Activator.CreateInstance(type);
                var properties = type.GetProperties();

                Console.WriteLine(string.Format("Type Found it Class: {0} -- Namespace {1} -- Beginning process.........", type.Name, type.Namespace));

                BuildServices();

                //foreach (var prop in properties)
                //{
                //    Console.WriteLine("pro " + prop.Name);
                //}                

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

            var sbInterface = new StringBuilder(interfaceStr);
            var sbImplementation = new StringBuilder(implementationStr);

            sbInterface.Replace("[***]", _input.TargetModelName);
            sbImplementation.Replace("[***]", _input.TargetModelName);

            FileUtils.CreateFileOrOverwrite(newInterfaceFullPath, sbInterface.ToString());
            FileUtils.CreateFileOrOverwrite(newImplementaitonFullPath, sbImplementation.ToString());

            Console.WriteLine("Build services succeeded with paths:");
            Console.WriteLine(newInterfaceFullPath);
            Console.WriteLine(newImplementaitonFullPath);
        }
    }
}
