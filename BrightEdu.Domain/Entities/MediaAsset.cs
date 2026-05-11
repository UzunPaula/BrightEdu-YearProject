namespace BrightEdu.Domain.Entities;

public class MediaAsset
{
    private MediaAsset()
    {
    }

    public MediaAsset(Guid id, string fileName, string storedFileName, string relativePath, string mimeType, long sizeInBytes)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName invalid.", nameof(fileName));
        if (string.IsNullOrWhiteSpace(storedFileName)) throw new ArgumentException("StoredFileName invalid.", nameof(storedFileName));
        if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("RelativePath invalid.", nameof(relativePath));
        if (string.IsNullOrWhiteSpace(mimeType)) throw new ArgumentException("MimeType invalid.", nameof(mimeType));
        if (sizeInBytes < 0) throw new ArgumentOutOfRangeException(nameof(sizeInBytes));

        Id = id;
        FileName = fileName.Trim();
        StoredFileName = storedFileName.Trim();
        RelativePath = relativePath.Trim();
        MimeType = mimeType.Trim();
        SizeInBytes = sizeInBytes;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string FileName { get; private set; } = null!;
    public string StoredFileName { get; private set; } = null!;
    public string RelativePath { get; private set; } = null!;
    public string MimeType { get; private set; } = null!;
    public long SizeInBytes { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
