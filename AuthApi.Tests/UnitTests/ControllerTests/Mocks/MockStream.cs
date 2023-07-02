using System.Text;

namespace AuthApi.Tests.UnitTests.ControllerTests.Mocks;

public class MockStream : Stream
{
    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => true;
    public override long Length => throw new NotImplementedException();
    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Body { get; private set; } = null!;
    public override void Flush()
    {
        throw new NotImplementedException();
    }
    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }
    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
        Body = Encoding.UTF8.GetString(buffer.Skip(offset).Take(count).ToArray());
    }
}