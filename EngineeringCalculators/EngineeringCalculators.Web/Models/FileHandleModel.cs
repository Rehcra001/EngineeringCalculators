using KristofferStrube.Blazor.FileSystem;

namespace EngineeringCalculators.Web.Models
{
    public class FileHandleModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public FileSystemFileHandle? FileHandle { get; set; }
    }
}
