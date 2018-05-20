
using MVCCodeGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.IO;
using System.Reflection;
using System.Text;
using MVCCodeGenerator.Utils;

namespace MVCCodeGenerator
{
    /// <summary>
    /// Class will generat functionality based on model created on your target project
    /// </summary>
    public class CodeGenerator
    {
        private InputStruct _input;

        public CodeGenerator(InputStruct inputStruct)
        {
            _input = inputStruct;
        }

        public void Run()
        {
            //grap templates for interfaces, services, controller, views, models

            //grab data model from path
           
            CompileInput();

            Console.ReadLine();
        }

        private void CompileInput()
        {

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(FileUtils.ReadStringFromFile(_input.TargetModelFilePath + "\\" + _input.TargetModelFileName));

            string assemblyName = Path.GetRandomFileName();

            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
               // MetadataReference.CreateFromFile(typeof(System.ComponentModel.DataAnnotations.Schema.Table).Assembly.Location),
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());

                    Type type = assembly.GetType("RoslynCompileSample.Writer");
                    object obj = Activator.CreateInstance(type);

                    var properties = type.GetProperties();

                    foreach(var prop in properties)
                    {
                        Console.WriteLine("pro " + prop.Name);
                    }

                    //type.InvokeMember("Write",
                    //    BindingFlags.Default | BindingFlags.InvokeMethod,
                    //    null,
                    //    obj,
                    //    new object[] { "Hello World" });
                }
            }

            Console.ReadKey();
        
        }

        

    }
}
