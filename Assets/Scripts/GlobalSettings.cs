using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class GlobalSettings{

	public const int Fps = 60;
	public const float MaxPosition = 100.0f;
	public const float PositionPrecision = 0.1f;
	public const float MinPosition = -100.0f;
	public const int MaxHealth = 100;
	public const int MaxMatchDuration = 3600;
	public const int MaxACK = int.MaxValue;

	public const int GamePort = 42069;

	public const float maxRotation = 360.0f;

	public const int ReliableTimeout = 30;

	public const float BUFFERWINDOW = 3;
	public const int PrintingSubFrameRate = 3;

	public const int MaxPlayers = 32;
}
