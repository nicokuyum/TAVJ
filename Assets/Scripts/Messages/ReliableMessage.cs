public abstract class ReliableMessage : GameMessage {

	private static int idCounter = 0;
    
	public int _MessageId;
	
	protected ReliableMessage()
	{
		this._MessageId = idCounter++;
	}

	public override bool isReliable()
	{
		return true;
	}

	public override int GetHashCode()
	{
		return _MessageId;
	}
}
