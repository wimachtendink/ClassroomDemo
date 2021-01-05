using System;
using System.Collections.Generic;
using System.Net.Sockets;

//40 * 24bytes about 1kb
public class PoseMessage_Server
{
	public MessageHeader header;
	//data byte will tell us how many poses are being sent

	//we will need to fill a large number of PoseDescriptions from server
	//might be best to have an arr[] instead, but we'll figure that out later
	public List<PoseDescription> poseDescriptions;

	public void ReadStream(NetworkStream networkStream, MessageHeader incomingHeader)
	{
		header = incomingHeader;

		poseDescriptions = new List<PoseDescription>(header.dataByte);

		for (int readIdx = 0; readIdx < header.dataByte; readIdx++)
		{
			PoseDescription newPoseDescription = new PoseDescription().FakeConstructor();

			_ = networkStream.Read(ByteTypeConverter.SbytesTobytes(newPoseDescription.data), 0, PoseDescription.DATA_SIZE);

			poseDescriptions.Add(newPoseDescription);
		}
	}

	public PoseMessage_Server(NetworkStream _networkStream, MessageHeader _header)
	{
		poseDescriptions = new List<PoseDescription>(_header.dataByte);
		ReadStream(_networkStream, _header);
	}
}