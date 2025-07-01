

namespace CustorPortalAPI.Models;

public partial class File
{
    public int FileKey { get; set; }

    public int ProjectKey { get; set; }

    public string? FileName { get; set; } 

    public string? FileType { get; set; } 

    public int Version { get; set; }

    public string? FilePath { get; set; } 

    public int Size { get; set; }

    public int UploaderKey { get; set; }

    public DateTime UploadedAt { get; set; }

    public bool? IsCurrent { get; set; }

    public ICollection<Comment>? Comments { get; set; } 

    public  Project? Project { get; set; } 

    public  User? Uploader { get; set; } 
}
