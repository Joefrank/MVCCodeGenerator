
namespace MVCCodeGenerator.Model
{
    public class InputEmbedded
    {
        public string AssemblyName { get; set; }
        public string TargetModelName { get; set; }
        public string TargetModelNameSpace { get; set; }
        public string ServicesPath { get; set; }
        public string ServicesProjectName { get; set; }
        public string ViewModelPath { get; set; }
        public string ViewModelProjectPath { get; set; }
        public string ViewModelProjectName { get; set; }
        public string ControllerPath { get; set; }
        public string ControllerProjectPath { get; set; }
        public string ViewsPath { get; set; }
        public string PartialFormViewsPath { get; set; }
        public string ViewsProjectPath { get; set; }
        public string MappingProfilePath { get; set; }
        public string ContainerInjectionPath { get; set; }
    }
}
