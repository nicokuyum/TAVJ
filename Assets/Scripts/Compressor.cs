using System;
using System.IO;

public class Compressor : Encoder
{
    private long bits;
    private int currentBitCount;

    private MemoryStream buffer;

    public Compressor()
    {
        bits = 0;
        currentBitCount = 0;
        buffer = new MemoryStream();
    }

    public byte[] GetBuffer()
    {
        Flush();
        return buffer.GetBuffer();
    }

    public void WriteNumber(long value, int size)
    {
        while (size > 0)
        {
            PutBit(((value >> (size-1)) & 1) != 0);
            size--;
        }
    }

    public void WriteFloat(float value, float max, float min, float precision)
    {
        double values = (max-min) / precision;
        int maxValues = Convert.ToInt32(Math.Ceiling(values));
        int requiredBits = BitLength(maxValues);
        int actualBits = Convert.ToInt32((value - min) / precision);
        WriteNumber(actualBits, requiredBits);
    }
    
    public void PutBit(bool value) {
        Console.WriteLine("Putting bit: " + (value ? 1 : 0));
        long longValue = value ? 1L : 0L;
        bits |= longValue << currentBitCount;
        currentBitCount++;
        WriteIfNecessary();
    }
    
    private void WriteIfNecessary() {
        if (currentBitCount >= 32) {
            //if (buffer.Position + 4 > buffer.Capacity) {
            //    throw new InvalidOperationException("write buffer overflow");
            //}
            int word = (int) bits;
            byte a = (byte) (word);
            byte b = (byte) (word >> 8);
            byte c = (byte) (word >> 16);
            byte d = (byte) (word >> 24);
            buffer.WriteByte(a);
            buffer.WriteByte(b);
            buffer.WriteByte(c);
            buffer.WriteByte(d);
            bits >>= 32;
            currentBitCount -= 32;
        }
    }

    private void Flush()
    {
        if (currentBitCount > 0)
        {
            WriteNumber(0, 32 - currentBitCount);
            WriteIfNecessary();
        }
    }
}
