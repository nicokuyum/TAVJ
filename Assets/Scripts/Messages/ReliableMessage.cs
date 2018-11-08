public abstract class ReliableMessage : GameMessage {

	private static int idCounter = 0;
    
	public int _MessageId;
	public float _TimeStamp;
	
	protected ReliableMessage(float timeStamp)
	{
		this._MessageId = idCounter++;
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
