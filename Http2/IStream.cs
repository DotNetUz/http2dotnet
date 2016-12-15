using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Hpack;

namespace Http2
{
    /// <summary>
    /// Enumerates possible states of a stream
    /// </summary>
    public enum StreamState
    {
        Idle,
        ReservedLocal,
        ReservedRemote,
        Open,
        HalfClosedLocal,
        HalfClosedRemote,
        Closed,
        Reset, // TODO: Should this be an extra state?
    }

    /// <summary>
    /// A HTTP/2 stream
    /// </summary>
    public interface IStream : IStreamReader, IStreamWriterCloser
    {
        /// <summary>The ID of the stream</summary>
        uint Id { get; }
        /// <summary>Returns the current state of the stream</summary>
        StreamState State { get; }

        /// <summary>
        /// Cancels the stream.
        /// This will cause sending a RESET frame to the remote peer
        /// if the stream was not yet fully processed.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Writes a header block for the stream.
        /// </summary>
        /// <param name="headers">
        /// The list of header fields to write.
        /// The headers must contain the required pseudo headers for the
        /// type of the stream. The pseudo headers must be at the start
        /// of the list.
        /// </param>
        /// <param name="endOfStream">
        /// Whether the stream should be closed with the headers.
        /// If this is set to true no data frames may be written.
        /// </param>
        ValueTask<object> WriteHeaders(IEnumerable<HeaderField> headers, bool endOfStream);

        /// <summary>
        /// Writes a block of trailing headers for the stream.
        /// The writing side of the stream will automatically be closed
        /// through this operation.
        /// </summary>
        /// <param name="headers">
        /// The list of trailing headers to write.
        /// No pseudo headers must be contained in this list.
        /// </param>
        ValueTask<object> WriteTrailers(IEnumerable<HeaderField> headers);

        /// <summary>
        /// Writes data to the stream and optionally allows to signal the end
        /// of the stream.
        /// </summary>
        /// <param name="buffer">The block of data to write</param>
        /// <param name="endOfStream">
        /// Whether this is the last data block and the stream should be closed
        /// after this operation
        /// </param>
        ValueTask<object> WriteAsync(ArraySegment<byte> buffer, bool endOfStream = false);
    }
}