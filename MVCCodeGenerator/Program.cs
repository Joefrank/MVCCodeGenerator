
using MVCCodeGenerator.Model;
using System;

namespace MVCCodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new InputEmbedded
            {
                AssemblyName = "elearning.model",
                TargetModelName = "CourseChapter",
                TargetModelNameSpace = "elearning.model.DataModels",
                ServicesPath = @"C:\projects\Websites\Personal\itestudy.com\elearning\elearning.services",
                ViewModelPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.model\ViewModels",
                ViewModelProjectPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.model",
                ViewModelProjectName = "elearning.model.csproj",
                ServicesProjectName = "elearning.services.csproj",
                ControllerPath = @"C:\projects\Websites\Personal\itestudy.com\elearning\elearning.admin\Controllers",
                ControllerProjectPath = @"C:\projects\Websites\Personal\itestudy.com\elearning\elearning.admin",
                ControllerProjectName = "elearning.admin.csproj",
                ViewsPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.admin\Views",
                PartialFormViewsPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.admin\Views\Shared\Forms",
                ViewsProjectPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.admin\elearning.admin.csproj",
                MappingProfilePath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.admin\Models\MapperProfile.cs",
                ContainerInjectionPath = @"C:\Projects\Websites\Personal\itestudy.com\elearning\elearning.utils\ElearningApplication.cs"
            };

            (new EmbeddedCodeGenerator(input)).Run();

            Console.ReadLine();

        }
    }
}

