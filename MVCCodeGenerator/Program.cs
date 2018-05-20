
using MVCCodeGenerator.Model;
using elearning.model.DataModels;
using System;

namespace MVCCodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //var input = new InputStruct
            //{
            //    TargetModelFilePath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.model\DataModels",
            //    TargetModelFileName = "CourseChapter.cs",
            //    TargetModelReferencePath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.model\bin\Debug",
            //    TargetModelName = "CourseChapter",
            //    DataDbContextPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.data\DataDbContext.cs",
            //    EnumPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.model\Enums",
            //    ServicesPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.services",
            //    ViewModelPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.model\ViewModels",
            //    WebProjectPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.admin"
            //};

            //var generator = new CodeGenerator(input);

            //generator.Run();

            var input = new InputEmbedded
            {
                AssemblyName = "elearning.model",
                TargetModelName = "CourseChapter",
                TargetModelNameSpace = "elearning.model.DataModels",
                ServicesPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.services"
            };

            (new EmbeddedCodeGenerator(input)).Run();

            Console.ReadLine();

        }
    }
}

