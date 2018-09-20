using System;
using System.IO;
using System.Text;
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
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < size; i++)
		{
			sb.Append(GetChar());
		}
		return sb.ToString();
	}

	public char GetChar()
	{
		return (char)(GetNumber(_maxChar)+_minChar);
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
		return data[position / 8];
	}
}
