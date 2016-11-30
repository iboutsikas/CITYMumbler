﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;
using NUnit.Framework;

namespace CITYMumbler.UnitTests.Networking
{
    [TestFixture]
    class PacketSerializerTests
    {
        [Test]
        public void serialize_packet_to_bytes()
        {
            // Arrange
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] result = serializer.ToBytes(packet);

            // Assert
            BinaryReader reader = new BinaryReader(new MemoryStream(result));
            Assert.AreEqual(Encoding.ASCII.GetString(reader.ReadBytes(2)), Encoding.ASCII.GetString(PacketSerializer.APP_IDENTIFIER));
            Assert.AreEqual(reader.ReadByte(), PacketSerializer.VERSION_MAJOR);
            Assert.AreEqual(reader.ReadByte(), PacketSerializer.VERSION_MINOR);
            Assert.AreEqual(reader.ReadByte(), (byte)packet.PacketType);
        }

        [Test]
        public void deserialize_packet_from_bytes()
        {
            // Arrange
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] bytes = serializer.ToBytes(packet);
            IPacket result = serializer.FromBytes(bytes);

            // Assert
            Assert.AreEqual(result.PacketType, packet.PacketType);
            PrivateMessagePacket castResult = (PrivateMessagePacket)result;
            PrivateMessagePacket castPacket = (PrivateMessagePacket)packet;
            Assert.AreEqual(castResult.SenderId, castPacket.SenderId);
            Assert.AreEqual(castResult.ReceiverId, castPacket.ReceiverId);
            Assert.AreEqual(castResult.Message, castPacket.Message);
        }

        [Test]
        public void deserialize_throws_exception_if_app_identifier_wrong()
        {
            // Arrange 
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] bytes = serializer.ToBytes(packet);
            bytes[0] = Convert.ToByte('B');

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => serializer.FromBytes(bytes));
            Assert.That(ex.Message, Is.EqualTo("The APP_IDENTIFIER on this packet does not match the application"));
        }

        [Test]
        public void deserialize_throws_exception_if_version_is_wrong()
        {
            // Arrange 
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] bytes = serializer.ToBytes(packet);
            bytes[3] = 5;

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => serializer.FromBytes(bytes));
            Assert.That(ex.Message, Is.EqualTo("This packet comes from a different version of the Serializer"));
        }


        [Test]
        public void deserialize_throws_exception_if_packetType_does_not_exist()
        {
            // Arrange 
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] bytes = serializer.ToBytes(packet);
            bytes[4] = 104;

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => serializer.FromBytes(bytes));
            Assert.That(ex.Message, Is.EqualTo("The provided PacketTypeHeader is not valid."));
        }
    }
}
