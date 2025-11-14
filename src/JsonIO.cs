using System.IO;
using System.IO.Compression;
using System.Text;

namespace TourneyMod;

internal static class JsonIO
{
    internal static void WriteFile(string path, string data)
    {
        using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        using FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        {
            CopyStream(memoryStream, fileStream);
        }
    }

    internal static string ReadFile(FileInfo file)
    {
        using FileStream fileStream = file.OpenRead();
        using MemoryStream memoryStream = new MemoryStream();
        {
            CopyStream(fileStream, memoryStream);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }

    private static void CopyStream(Stream input, Stream output)
    {
        byte[] buffer = new byte[8 * 1024];
        int len;
        while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, len);
        }
    }
}