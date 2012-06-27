using System;
namespace Palo.ChatLibrary.Converters
{
    public interface IByteToStringConverter
    {
        string Convert(byte[] value);
        string Convert(byte[] value, int length);
        byte[] ConvertBack(string value);
    }
}
