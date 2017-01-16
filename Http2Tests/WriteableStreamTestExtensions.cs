using System;
using System.Threading.Tasks;

using Xunit;

using Http2;

namespace Http2Tests
{
    public static class WriteableStreamTestExtensions
    {
        public static async Task WriteFrameHeader(
            this IWriteAndCloseableByteStream stream,
            FrameHeader fh)
        {
            var headerBytes = new byte[FrameHeader.HeaderSize];
            fh.EncodeInto(new ArraySegment<byte>(headerBytes));
            await stream.WriteAsync(new ArraySegment<byte>(headerBytes));
        }

        public static async Task WriteDefaultSettings(
            this IWriteAndCloseableByteStream stream)
        {
            var settings = Settings.Default;
            var settingsData = new byte[settings.RequiredSize];
            var fh = new FrameHeader
            {
                Type = FrameType.Settings,
                Length = settingsData.Length,
                Flags = 0,
                StreamId = 0,
            };
            settings.EncodeInto(new ArraySegment<byte>(settingsData));
            await stream.WriteFrameHeader(fh);
            await stream.WriteAsync(new ArraySegment<byte>(settingsData));
        }

        public static async Task WriteSettingsAck(
            this IWriteAndCloseableByteStream stream)
        {
            var fh = new FrameHeader
            {
                Type = FrameType.Settings,
                Length = 0,
                Flags = (byte)SettingsFrameFlags.Ack,
                StreamId = 0,
            };
            await stream.WriteFrameHeader(fh);
        }
    }
}