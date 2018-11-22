public abstract class ReliableMessage : GameMessage {

	private static int idCounter = 560;
    
	public int _MessageId;
	public float _TimeStamp;
	
	protected ReliableMessage(float timeStamp, bool increase)
	{
		if (increase)
		{
			this._MessageId = idCounter++;
		}
		this._TimeStamp = timeStamp;
	}

	public override bool isReliable()
	{
		return true;
	}

	public override int GetHashCode()
	{
		return _MessageId;
	}

	public override bool Equals(object obj)
	{
		return _MessageId.Equals(((ReliableMessage)obj)._MessageId);
	}
}
