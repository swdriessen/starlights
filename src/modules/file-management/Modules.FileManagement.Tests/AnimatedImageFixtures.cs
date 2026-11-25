using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using SkiaSharp;

namespace Starlights.Modules.FileManagement.Tests;

internal static class AnimatedImageFixtures
{
    public static byte[] Png()
    {
        using var output = new MemoryStream();
        output.Write([137, 80, 78, 71, 13, 10, 26, 10]);
        byte[] header = [0, 0, 0, 1, 0, 0, 0, 1, 8, 6, 0, 0, 0];
        WritePngChunk(output, "IHDR", header);
        WritePngChunk(output, "acTL", [0, 0, 0, 2, 0, 0, 0, 0]);
        byte[] control = new byte[26];
        BinaryPrimitives.WriteUInt32BigEndian(control.AsSpan(4), 1);
        BinaryPrimitives.WriteUInt32BigEndian(control.AsSpan(8), 1);
        control[21] = 1;
        control[23] = 10;
        WritePngChunk(output, "fcTL", control);
        WritePngChunk(output, "IDAT", CompressedPixel(255, 0, 0));
        BinaryPrimitives.WriteUInt32BigEndian(control, 1);
        WritePngChunk(output, "fcTL", control);
        WritePngChunk(output, "fdAT", [0, 0, 0, 2, .. CompressedPixel(0, 0, 255)]);
        WritePngChunk(output, "IEND", []);
        return output.ToArray();
    }

    public static byte[] Webp(bool includeSecondFrame = true)
    {
        using var bitmap = new SKBitmap(1, 1);
        bitmap.Erase(SKColors.CornflowerBlue);
        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(SKEncodedImageFormat.Webp, 90);
        var frame = encoded.ToArray()[12..];
        using var chunks = new MemoryStream();
        WriteWebpChunk(chunks, "VP8X", [2, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        WriteWebpChunk(chunks, "ANIM", new byte[6]);
        byte[] frameHeader = new byte[16];
        frameHeader[12] = 100;
        if (includeSecondFrame)
            WriteWebpChunk(chunks, "ANMF", [.. frameHeader, .. frame]);
        WriteWebpChunk(chunks, "ANMF", [.. frameHeader, .. frame]);
        using var output = new MemoryStream();
        output.Write("RIFF"u8);
        Span<byte> size = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(size, (uint)chunks.Length + 4);
        output.Write(size);
        output.Write("WEBP"u8);
        chunks.Position = 0;
        chunks.CopyTo(output);
        return output.ToArray();
    }

    private static byte[] CompressedPixel(byte red, byte green, byte blue)
    {
        using var output = new MemoryStream();
        using (var compression = new ZLibStream(output, CompressionLevel.SmallestSize, leaveOpen: true))
            compression.Write([0, red, green, blue, 255]);
        return output.ToArray();
    }

    public static void WritePngChunk(Stream output, string type, byte[] data)
    {
        Span<byte> number = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(number, (uint)data.Length);
        output.Write(number);
        var payload = Encoding.ASCII.GetBytes(type).Concat(data).ToArray();
        output.Write(payload);
        var crc = uint.MaxValue;
        foreach (var value in payload)
        {
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc >> 1) ^ ((crc & 1) != 0 ? 0xEDB88320u : 0u);
        }
        BinaryPrimitives.WriteUInt32BigEndian(number, ~crc);
        output.Write(number);
    }

    private static void WriteWebpChunk(Stream output, string type, byte[] data)
    {
        output.Write(Encoding.ASCII.GetBytes(type));
        Span<byte> size = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(size, (uint)data.Length);
        output.Write(size);
        output.Write(data);
        if (data.Length % 2 != 0)
            output.WriteByte(0);
    }
}
