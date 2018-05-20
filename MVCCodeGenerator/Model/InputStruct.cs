
namespace MVCCodeGenerator.Model
{
    public class InputStruct
    {
        public string TargetModelFilePath { get; set; }
        public string TargetModelFileName { get; set; }
        public string TargetModelReferencePath { get; set; }
        public string TargetModelName { get; set; }
        public string ServicesPath { get; set; }
        public string DataDbContextPath { get; set; }
        public string ViewModelPath { get; set; }
        public string EnumPath { get; set; }
        public string WebProjectPath { get; set; }
    }
}
