using System;

public class PoseMessage_Client
{

//-//Static
	static int _size = -255;
	public static int Size
	{
		get
		{
			if (_size < 0)
			{
				//checking every time... kinda dumb maybe learn about source generators :D
				_size = MessageHeader.HEADER_SIZE + PoseDescription.DATA_SIZE;
			}
			return _size;
		}
	}

//-//Member
	public MessageHeader header;
	public PoseDescription poseDescription;

	public byte[] GetBytes()
	{
		var output = new byte[Size];

		header.GetBytes().CopyTo(output, 0);

		//Buffer.BlockCopy(poseDescription.GetBytes() )
		poseDescription.GetBytes().CopyTo(output, MessageHeader.HEADER_SIZE);

		return output;
	}

	public PoseMessage_Client()
	{
		header = new MessageHeader(MessageHeader.MessageType.poseMessage, 255);
		poseDescription = new PoseDescription();
	}

}