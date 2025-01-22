using KristofferStrube.Blazor.FileSystem;
using System.ComponentModel.DataAnnotations;

namespace EngineeringCalculators.Web.Models
{
    public class FolderHandleModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public FileSystemDirectoryHandle? FolderHandle { get; set; }
    }
}
