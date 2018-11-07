using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class GlobalSettings{

	public const int Fps = 60;
	public const float MaxPosition = 20.0f;
	public const float PositionPrecision = 0.01f;
	public const float MinPosition = -20.0f;
	public const int MaxHealth = 100;
	public const int MaxMatchDuration = 3600;
	public const int MaxACK = int.MaxValue;

	public const int GamePort = 42069;

	public const float maxRotation = 360.0f;

	public const float ReliableTimeout = 0.3f;

	public const float BufferWindow = 3;
	public const int PrintingSubFrameRate = 3;

	public const int MaxPlayers = 32;

	public const int ServerSendRate = 10;

	public const float TimePrecision = 0.001f;
	public const float MinTime = 0.0f;
	public const float MaxTime = 3600.0f;

	public const float speed = 1.0f;
}
