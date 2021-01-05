using System;

//struct for copy instead of reference
public struct MessageHeader
{
	public const int HEADER_SIZE = 3; //userID, MessageType

	//TODO: make this it's own script
	public enum MessageType : byte
	{
		none,
		configMessage,
		poseMessage,
		audioMessage
	}

	public MessageType messageType; //1 byte
	public byte ClientID;           //1 byte
	public byte dataByte;           //1 byte

	public MessageHeader(MessageType _messageType, int _ClientID, byte _dataByte = 0)
	{
		messageType = _messageType;
		ClientID = (byte)_ClientID;
		dataByte = _dataByte;
	}

	public MessageHeader(byte[] data)
	{
		if (data.Length != HEADER_SIZE)
		{
			throw new ArgumentException($"ClientMessageHeader must be of size {HEADER_SIZE} using first {HEADER_SIZE} bytes which may be a bad idea");
		}

		messageType = (MessageType)data[0];
		ClientID = data[1];
		dataByte = data[2];
	}



	public byte[] GetBytes()
	{
		byte[] output = new byte[HEADER_SIZE];

		output[0] = (byte)messageType;
		output[1] = (byte)ClientID;
		output[2] = (byte)dataByte;

		return output;
	}

	public override string ToString()
	{
		return $"messageType:{messageType}, ClientID:{ClientID}, DataByte{dataByte}";
	}

}