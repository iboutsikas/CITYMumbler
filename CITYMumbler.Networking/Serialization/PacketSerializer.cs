﻿using System;
using System.Text;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Utilities;

namespace CITYMumbler.Networking.Serialization
{
	public class PacketSerializer : IPacketSerializer
	{
		public static readonly byte[] APP_IDENTIFIER = Encoding.ASCII.GetBytes("CM");
		public static readonly byte VERSION_MAJOR = 1;
		public static readonly byte VERSION_MINOR = 0;

		public IPacket FromBytes(byte[] bytes)
		{
			PacketReader reader = new PacketReader(bytes);

			byte identifier1 = reader.ReadByte();
			byte identifier2 = reader.ReadByte();

			if (identifier1 != APP_IDENTIFIER[0] || identifier2 != APP_IDENTIFIER[1])
				throw new ArgumentException("The APP_IDENTIFIER on this packet does not match the application");

			byte major = reader.ReadByte();
			byte minor = reader.ReadByte();

			if (major != VERSION_MAJOR || minor != VERSION_MINOR)
				throw new ArgumentException("This packet comes from a different version of the Serializer");

			PacketType type = (PacketType)reader.ReadByte();
			IPacket packet = reader.ReadPacket(type);

			return packet;
		}

		public byte[] ToBytes(IPacket packet)
		{
			PacketWritter writter = new PacketWritter();

			// Write Header
			//
			// Write the App identifier and Version
			writter.Write(APP_IDENTIFIER);
			writter.Write(VERSION_MAJOR);
			writter.Write(VERSION_MINOR);

			// Write the packetType
			writter.Write((byte) packet.PacketType);
			// 
			// End Header

			// Write Payload
			switch (packet.PacketType)
			{
				case PacketType.SendKeystroke:
					writter.Write((SendKeystrokePacket) packet);
					break;
				case PacketType.GroupMessage:
					writter.Write((GroupMessagePacket)packet);
					break;
				case PacketType.PrivateMessage:
					writter.Write((PrivateMessagePacket)packet);
					break;
				case PacketType.Connection:
					writter.Write((ConnectionPacket)packet);
					break;
				case PacketType.Disconnection:
					writter.Write((DisconnectionPacket) packet);
					break;
				case PacketType.Connected:
					writter.Write((ConnectedPacket)packet);
					break;
				case PacketType.CreateGroup:
					writter.Write((CreateGroupPacket)packet);
					break;
				case PacketType.DeleteGroup:
					writter.Write((DeleteGroupPacket)packet);
					break;
				case PacketType.ChangeGroupOwner:
					writter.Write((ChangeGroupOwnerPacket)packet);
					break;
				case PacketType.JoinGroup:
					writter.Write((JoinGroupPacket)packet);
					break;
				case PacketType.JoinedGroup:
					writter.Write((JoinedGroupPacket)packet);
					break;
				case PacketType.Kick:
					writter.Write((KickPacket)packet);
					break;
				case PacketType.LeaveGroup:
					writter.Write((LeaveGroupPacket)packet);
					break;
				case PacketType.LeftGroup:
					writter.Write((LeftGroupPacket)packet);
					break;
				default:
					throw new ArgumentException("Packet is of an unsupported packetType");
			}

			// Return the end packet
			return writter.GetBytes();
		}
	}
}
