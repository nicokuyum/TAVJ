using System;
using System.IO;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Decompressor : Encoder
{

	private byte[] data;
	private int position;

	public Decompressor(byte[] data)
	{
		this.data = data;
		this.position = 0;
	}

	public bool GetBoolean()
	{
		return (GetByteFromPos() >> ((position++ % 8)) & 1) == 1;
	}

	public String GetString()
	{
		int size = GetNumber(_maxStringLength);
		if (size < 0 || size > _maxStringLength)
		{
			throw new Exception("Invalid string length " + size);
		}
		char[] c = new char[size];
		for (int i = 0; i < size; i++)
		{
			c[i] = GetChar();
		}
		return new String(c);
	}

	public char GetChar()
	{
		return (char)GetNumber(_maxChar);
	}

	public int GetNumber(long maxValue)
	{
		var bits = GetBitsRequired(maxValue);
		var num = 0;
		while (bits > 0)
		{
			num <<= 1;
			num |= (GetBoolean() ? 1 : 0);
			bits--;
		}
		return num;
	}

	public float GetFloat(float max, float min, float precision)
	{
		var maxValue = Convert.ToInt64(Math.Ceiling((max-min) / precision));
		var num = GetNumber(maxValue);
		return min + num * precision;
	}

	private byte GetByteFromPos()
	{
		return data[(position) / 8];
	}
}
