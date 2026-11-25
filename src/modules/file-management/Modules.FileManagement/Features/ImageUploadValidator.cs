using System.Buffers.Binary;
using SkiaSharp;

namespace Starlights.Modules.FileManagement.Features;

internal sealed class ImageUploadValidator
{
    public const int MaximumBytes = 5 * 1024 * 1024;
    public const int MaximumDimension = 4096;

    public static (byte[] Bytes, string ContentType) Validate(string encoded)
    {
        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(encoded);
        }
        catch (FormatException)
        {
            throw new FileUploadException("The file content must be valid base64.");
        }
        if (bytes.Length == 0)
            throw new FileUploadException("Choose a non-empty image.");
        if (bytes.Length > MaximumBytes)
            throw new FileUploadException("Images must not exceed 5 MiB.", 413);

        using var data = SKData.CreateCopy(bytes);
        using var codec = SKCodec.Create(data);
        if (codec is null)
            throw new FileUploadException("The file is not a valid PNG, JPEG or WebP image.");
        var contentType = codec.EncodedFormat switch
        {
            SKEncodedImageFormat.Png => "image/png",
            SKEncodedImageFormat.Jpeg => "image/jpeg",
            SKEncodedImageFormat.Webp => "image/webp",
            _ => throw new FileUploadException("Only PNG, JPEG and WebP images are supported.")
        };
        if (codec.Info.Width <= 0 || codec.Info.Height <= 0
            || codec.Info.Width > MaximumDimension || codec.Info.Height > MaximumDimension)
            throw new FileUploadException("Images must not exceed 4096 pixels per side.");
        if (codec.FrameCount > 1
            || (codec.EncodedFormat == SKEncodedImageFormat.Png && HasAnimationControl(bytes))
            || (codec.EncodedFormat == SKEncodedImageFormat.Webp && bytes.Length >= 30
                && bytes.AsSpan(12, 4).SequenceEqual("VP8X"u8) && (bytes[20] & 2) != 0))
            throw new FileUploadException("Animated images are not supported.");

        using var bitmap = new SKBitmap(new SKImageInfo(codec.Info.Width, codec.Info.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        if (codec.GetPixels(bitmap.Info, bitmap.GetPixels()) != SKCodecResult.Success)
            throw new FileUploadException("The image is incomplete or malformed.");
        return (bytes, contentType);
    }

    private static bool HasAnimationControl(byte[] bytes)
    {
        // APNG can otherwise be decoded as an ordinary static PNG by some codecs.
        var offset = 8;
        while (offset <= bytes.Length - 12)
        {
            var length = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(offset, 4));
            if (length > bytes.Length - offset - 12)
                throw new FileUploadException("The PNG image contains an invalid chunk.");
            if (bytes.AsSpan(offset + 4, 4).SequenceEqual("acTL"u8))
                return true;
            if (bytes.AsSpan(offset + 4, 4).SequenceEqual("IEND"u8))
            {
                if (length != 0 || offset + 12 != bytes.Length)
                    throw new FileUploadException("The PNG image has an invalid end marker.");
                return false;
            }
            offset += (int)length + 12;
        }
        throw new FileUploadException("The PNG image is incomplete.");
    }
}
