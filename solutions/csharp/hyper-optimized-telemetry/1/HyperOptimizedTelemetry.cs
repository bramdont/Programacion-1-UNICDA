public static class TelemetryBuffer
{
    public static byte[] ToBuffer(long reading)
    {
        int width;
        bool signed;

        if (reading >= 0 && reading <= ushort.MaxValue)
        {
            width = 2;
            signed = false;
        }
        else if (reading >= short.MinValue && reading <= -1)
        {
            width = 2;
            signed = true;
        }
        else if (reading >= (long)int.MaxValue + 1 && reading <= uint.MaxValue)
        {
            width = 4;
            signed = false;
        }
        else if ((reading > ushort.MaxValue && reading <= int.MaxValue) ||
                 (reading >= int.MinValue && reading < short.MinValue))
        {
            width = 4;
            signed = true;
        }
        else
        {
            width = 8;
            signed = true;
        }

        byte[] payload = width switch
        {
            2 => BitConverter.GetBytes((short)reading),
            4 => BitConverter.GetBytes((int)reading),
            _ => BitConverter.GetBytes(reading)
        };

        var buffer = new byte[9];
        buffer[0] = signed ? (byte)(256 - width) : (byte)width;
        Array.Copy(payload, 0, buffer, 1, width);
        return buffer;
    }

    public static long FromBuffer(byte[] buffer)
    {
        return buffer[0] switch
        {
            2 => BitConverter.ToUInt16(buffer, 1),
            4 => BitConverter.ToUInt32(buffer, 1),
            8 => (long)BitConverter.ToUInt64(buffer, 1),
            0xfe => BitConverter.ToInt16(buffer, 1),
            0xfc => BitConverter.ToInt32(buffer, 1),
            0xf8 => BitConverter.ToInt64(buffer, 1),
            _ => 0
        };
    }
}
