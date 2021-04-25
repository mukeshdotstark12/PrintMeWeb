namespace PrintForMe.Models
{
    public class FileModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsSelected { get; set; }
        public byte[] fileBytes { get; set; }
    }
}