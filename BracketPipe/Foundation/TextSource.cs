namespace BracketPipe
{
  using BracketPipe.Extensions;
  using System;
  using System.IO;
  using System.Text;
  using System.Threading;
#if !NET35
  using System.Threading.Tasks;
#endif

  /// <summary>
  /// A string/stream abstraction to handle encoding.
  /// </summary>
  /// <remarks>
  /// Both a <see cref="string"/> and <see cref="Stream"/> are 
  /// implicitly convertible to <see cref="TextSource"/> and 
  /// can be used in place of a <see cref="TextSource"/>
  /// </remarks>
  public sealed class TextSource : TextReader
  {
    #region Fields

    const Int32 DefaultBufferSize = 4096;

    readonly int _bufferSize;
    readonly Stream _baseStream;
    readonly MemoryStream _raw;
    readonly Byte[] _buffer;
    readonly Char[] _chars;

    StringBuilder _content;
    EncodingConfidence _confidence;
    Boolean _finished;
    Encoding _encoding;
    Decoder _decoder;
    Int32 _index;

    #endregion

    #region ctor

    TextSource(Encoding encoding)
    {
      _index = 0;
      _encoding = encoding ?? TextEncoding.Utf8;
      _decoder = _encoding.GetDecoder();
    }

    /// <summary>
    /// Creates a new text source from a <see cref="StringBuilder"/>. No underlying stream will
    /// be used.
    /// </summary>
    /// <param name="source">The data source.</param>
    public TextSource(StringBuilder source)
        : this(TextEncoding.Utf8)
    {
      _finished = true;
      _content = source;
      _confidence = EncodingConfidence.Irrelevant;
    }

    /// <summary>
    /// Creates a new text source from a string. No underlying stream will
    /// be used.
    /// </summary>
    /// <param name="source">The data source.</param>
    public TextSource(String source)
        : this(TextEncoding.Utf8)
    {
      _finished = true;
      _content = Pool.NewStringBuilder();
      _content.Append(source);
      _confidence = EncodingConfidence.Irrelevant;
    }

    /// <summary>
    /// Creates a new text source from a string. The underlying stream is
    /// used as an unknown data source.
    /// </summary>
    /// <param name="baseStream">
    /// The underlying stream as data source.
    /// </param>
    /// <param name="encoding">
    /// The initial encoding. Otherwise UTF-8.
    /// </param>
    public TextSource(Stream baseStream, Encoding encoding = null)
        : this(encoding)
    {
      if (baseStream.CanSeek)
        _bufferSize = (int)(baseStream.Length / 2);
      else
        _bufferSize = DefaultBufferSize;

      _buffer = new Byte[_bufferSize];
      _chars = new Char[_bufferSize + 1];
      _raw = new MemoryStream();
      _baseStream = baseStream;
      _content = Pool.NewStringBuilder();
      _confidence = EncodingConfidence.Tentative;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the full text buffer.
    /// </summary>
    public String Text
    {
      get { return _content.ToString(); }
    }

    /// <summary>
    /// Gets the character at the given position in the text buffer.
    /// </summary>
    /// <param name="index">The index of the character.</param>
    /// <returns>The character.</returns>
    public Char this[Int32 index]
    {
      get { return _content[index]; }
    }

    /// <summary>
    /// Gets or sets the encoding to use.
    /// </summary>
    public Encoding CurrentEncoding
    {
      get { return _encoding; }
      set
      {
        if (_confidence != EncodingConfidence.Tentative)
        {
          return;
        }

        if (_encoding.IsUnicode())
        {
          _confidence = EncodingConfidence.Certain;
          return;
        }

        if (value.IsUnicode())
        {
          value = TextEncoding.Utf8;
        }

        if (value == _encoding)
        {
          _confidence = EncodingConfidence.Certain;
          return;
        }

        _encoding = value;
        _decoder = value.GetDecoder();

        var raw = _raw.ToArray();
        var raw_chars = new Char[_encoding.GetMaxCharCount(raw.Length)];
        var charLength = _decoder.GetChars(raw, 0, raw.Length, raw_chars, 0);
        var content = new String(raw_chars, 0, charLength);
        var index = Math.Min(_index, content.Length);

        if (content.Substring(0, index).Is(_content.ToString(0, index)))
        {
          //If everything seems to fit up to this point, do an
          //instant switch
          _confidence = EncodingConfidence.Certain;
          _content.Remove(index, _content.Length - index);
          _content.Append(content.Substring(index));
        }
        else
        {
          //Otherwise consider restart from beginning ...
          _index = 0;
          _content.Clear().Append(content);
          throw new NotSupportedException();
        }
      }
    }

    /// <summary>
    /// Gets or sets the current index of the insertation and read point.
    /// </summary>
    public Int32 Index
    {
      get { return _index; }
      set { _index = value; }
    }

    /// <summary>
    /// Gets the length of the text buffer.
    /// </summary>
    public Int32 Length
    {
      get { return _content.Length; }
    }

    #endregion

    #region Disposable

    /// <summary>
    /// Disposes the text source by freeing the underlying stream, if any.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      var isDisposed = _content == null;

      if (!isDisposed)
      {
        if (_raw != null)
          _raw.Dispose();
        _content.Clear().ToPool();
        _content = null;
      }
    }

    #endregion

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents the full text buffer.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents the full text buffer.
    /// </returns>
    public override string ToString()
    {
      return _content.ToString();
    }
    
    /// <summary>
    /// Converts the value of a substring of this buffer to a <see cref="System.String" />.
    /// </summary>
    /// <param name="start">The starting position of the substring in this instance.</param>
    /// <param name="length">The length of the substring.</param>
    /// <returns>
    /// A <see cref="System.String" /> whose value is the same as the specified substring of this buffer.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> or <paramref name="length"/>
    /// is less than zero.
    /// -or- The sum of <paramref name="start"/> and <paramref name="length"/>
    /// is greater than the length of the current buffer.</exception>
    public string ToString(int start, int length)
    {
      return _content.ToString(start, length);
    }

    public char[] ToCharArray(int start, int length)
    {
      var dest = new char[length];
      _content.CopyTo(start, dest, 0, length);
      return dest;
    }

    #region Text Methods

    /// <summary>Reads the next character without changing the state of the reader or the character source. Returns the next available character without actually reading it from the reader.</summary>
    /// <returns>An integer representing the next character to be read, or -1 if no more characters are available or the reader does not support seeking.</returns>
    /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextReader" /> is closed. </exception>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    public override int Peek()
    {
      var result = Read();
      _index--;
      return result;
    }

    /// <summary>Reads the next character from the text reader and advances the character position by one character.</summary>
    /// <returns>The next character from the text reader, or -1 if no more characters are available. The default implementation returns -1.</returns>
    /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextReader" /> is closed. </exception>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    public override int Read()
    {
      var result = ReadCharacter();
      if (result == Symbols.EndOfFile)
        return -1;
      return result;
    }

    /// <summary>
    /// Reads the next character from the buffer or underlying stream, if
    /// any.
    /// </summary>
    /// <returns>The next character.</returns>
    public Char ReadCharacter()
    {
      if (_index < _content.Length)
      {
        return _content[_index++];
      }

      ExpandBuffer(_bufferSize);
      var index = _index++;
      return index < _content.Length ? _content[index] : Symbols.EndOfFile;
    }

    /// <summary>Reads a specified maximum number of characters from the current reader and writes the data to a buffer, beginning at the specified index.</summary>
    /// <returns>The number of characters that have been read. The number will be less than or equal to <paramref name="count" />, depending on whether the data is available within the reader. This method returns 0 (zero) if it is called when no more characters are left to read.</returns>
    /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index" /> and (<paramref name="index" /> + <paramref name="count" /> - 1) replaced by the characters read from the current source. </param>
    /// <param name="index">The position in <paramref name="buffer" /> at which to begin writing. </param>
    /// <param name="count">The maximum number of characters to read. If the end of the reader is reached before the specified number of characters is read into the buffer, the method returns. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="buffer" /> is null. </exception>
    /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />. </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="index" /> or <paramref name="count" /> is negative. </exception>
    /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextReader" /> is closed. </exception>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    public override int Read(char[] buffer, int index, int count)
    {
      var start = _index;
      var end = start + count;

      if (end <= _content.Length)
      {
        _index += count;
        _content.CopyTo(_index, buffer, index, count);
        return count;
      }

      ExpandBuffer(Math.Max(_bufferSize, count));
      _index += count;
      count = Math.Min(count, _content.Length - start);
      _content.CopyTo(_index, buffer, index, count);
      return count;
    }

    /// <summary>Reads a specified maximum number of characters from the current text reader and writes the data to a buffer, beginning at the specified index.</summary>
    /// <returns>The number of characters that have been read. The number will be less than or equal to <paramref name="count" />, depending on whether all input characters have been read.</returns>
    /// <param name="buffer">When this method returns, this parameter contains the specified character array with the values between <paramref name="index" /> and (<paramref name="index" /> + <paramref name="count" /> -1) replaced by the characters read from the current source. </param>
    /// <param name="index">The position in <paramref name="buffer" /> at which to begin writing.</param>
    /// <param name="count">The maximum number of characters to read. </param>
    /// <exception cref="T:System.ArgumentNullException">
    ///   <paramref name="buffer" /> is null. </exception>
    /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />. </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref name="index" /> or <paramref name="count" /> is negative. </exception>
    /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextReader" /> is closed. </exception>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    public override int ReadBlock(char[] buffer, int index, int count)
    {
      return Read(buffer, index, count);
    }

    /// <summary>
    /// Reads the upcoming numbers of characters from the buffer or
    /// underlying stream, if any.
    /// </summary>
    /// <param name="characters">The number of characters to read.</param>
    /// <returns>The string with the next characters.</returns>
    public String ReadCharacters(Int32 characters)
    {
      var start = _index;
      var end = start + characters;

      if (end <= _content.Length)
      {
        _index += characters;
        return _content.ToString(start, characters);
      }

      ExpandBuffer(Math.Max(_bufferSize, characters));
      _index += characters;
      characters = Math.Min(characters, _content.Length - start);
      return _content.ToString(start, characters);
    }

#if INCLUDE_ASYNC
    /// <summary>
    /// Reads the next character from the buffer or underlying stream
    /// asynchronously, if any.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task resulting in the next character.</returns>
    public async Task<Char> ReadCharacterAsync(CancellationToken cancellationToken)
    {
      if (_index >= _content.Length)
      {
        await ExpandBufferAsync(_bufferSize, cancellationToken).ConfigureAwait(false);
        var index = _index++;
        return index < _content.Length ? _content[index] : Char.MaxValue;
      }

      return _content[_index++];
    }

    /// <summary>
    /// Reads the upcoming numbers of characters from the buffer or
    /// underlying stream asynchronously.
    /// </summary>
    /// <param name="characters">The number of characters to read.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The string with the next characters.</returns>
    public async Task<String> ReadCharactersAsync(Int32 characters, CancellationToken cancellationToken)
    {
      var start = _index;
      var end = start + characters;

      if (end <= _content.Length)
      {
        _index += characters;
        return _content.ToString(start, characters);
      }

      await ExpandBufferAsync(Math.Max(_bufferSize, characters), cancellationToken).ConfigureAwait(false);
      _index += characters;
      characters = Math.Min(characters, _content.Length - start);
      return _content.ToString(start, characters);
    }

    /// <summary>
    /// Prefetches the number of bytes by expanding the internal buffer.
    /// </summary>
    /// <param name="length">The number of bytes to prefetch.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The awaitable task.</returns>
    public Task PrefetchAsync(Int32 length, CancellationToken cancellationToken)
    {
      return ExpandBufferAsync(length, cancellationToken);
    }

    /// <summary>
    /// Prefetches the whole stream by expanding the internal buffer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The awaitable task.</returns>
    public async Task PrefetchAllAsync(CancellationToken cancellationToken)
    {
      if (_content.Length == 0)
      {
        await DetectByteOrderMarkAsync(cancellationToken).ConfigureAwait(false);
      }

      while (!_finished)
      {
        await ReadIntoBufferAsync(cancellationToken).ConfigureAwait(false);
      }
    }
#endif

    /// <summary>
    /// Inserts the given content at the current insertation mark. Moves the
    /// insertation mark.
    /// </summary>
    /// <param name="content">The content to insert.</param>
    public void InsertText(String content)
    {
      if (_index >= 0 && _index < _content.Length)
      {
        _content.Insert(_index, content);
      }
      else
      {
        _content.Append(content);
      }

      _index += content.Length;
    }

    #endregion

    #region Helpers

#if INCLUDE_ASYNC
    async Task DetectByteOrderMarkAsync(CancellationToken cancellationToken)
    {
      var count = await _baseStream.ReadAsync(_buffer, 0, _bufferSize).ConfigureAwait(false);
      var offset = 0;

      if (count > 2 && _buffer[0] == 0xef && _buffer[1] == 0xbb && _buffer[2] == 0xbf)
      {
        _encoding = TextEncoding.Utf8;
        offset = 3;
      }
      else if (count > 3 && _buffer[0] == 0xff && _buffer[1] == 0xfe && _buffer[2] == 0x0 && _buffer[3] == 0x0)
      {
        _encoding = TextEncoding.Utf32Le;
        offset = 4;
      }
      else if (count > 3 && _buffer[0] == 0x0 && _buffer[1] == 0x0 && _buffer[2] == 0xfe && _buffer[3] == 0xff)
      {
        _encoding = TextEncoding.Utf32Be;
        offset = 4;
      }
      else if (count > 1 && _buffer[0] == 0xfe && _buffer[1] == 0xff)
      {
        _encoding = TextEncoding.Utf16Be;
        offset = 2;
      }
      else if (count > 1 && _buffer[0] == 0xff && _buffer[1] == 0xfe)
      {
        _encoding = TextEncoding.Utf16Le;
        offset = 2;
      }
      else if (count > 3 && _buffer[0] == 0x84 && _buffer[1] == 0x31 && _buffer[2] == 0x95 && _buffer[3] == 0x33)
      {
        _encoding = TextEncoding.Gb18030;
        offset = 4;
      }

      if (offset > 0)
      {
        count -= offset;
        Array.Copy(_buffer, offset, _buffer, 0, count);
        _decoder = _encoding.GetDecoder();
        _confidence = EncodingConfidence.Certain;
      }

      AppendContentFromBuffer(count);
    }

    async Task ExpandBufferAsync(Int64 size, CancellationToken cancellationToken)
    {
      if (!_finished && _content.Length == 0)
      {
        await DetectByteOrderMarkAsync(cancellationToken).ConfigureAwait(false);
      }

      while (size + _index > _content.Length && !_finished)
      {
        await ReadIntoBufferAsync(cancellationToken).ConfigureAwait(false);
      }
    }

    async Task ReadIntoBufferAsync(CancellationToken cancellationToken)
    {
      var returned = await _baseStream.ReadAsync(_buffer, 0, _bufferSize, cancellationToken).ConfigureAwait(false);
      AppendContentFromBuffer(returned);
    }
#endif

    void DetectByteOrderMark()
    {
      var count = _baseStream.Read(_buffer, 0, _bufferSize);
      var offset = 0;

      if (count > 2 && _buffer[0] == 0xef && _buffer[1] == 0xbb && _buffer[2] == 0xbf)
      {
        _encoding = TextEncoding.Utf8;
        offset = 3;
      }
      else if (count > 3 && _buffer[0] == 0xff && _buffer[1] == 0xfe && _buffer[2] == 0x0 && _buffer[3] == 0x0)
      {
        _encoding = TextEncoding.Utf32Le;
        offset = 4;
      }
      else if (count > 3 && _buffer[0] == 0x0 && _buffer[1] == 0x0 && _buffer[2] == 0xfe && _buffer[3] == 0xff)
      {
        _encoding = TextEncoding.Utf32Be;
        offset = 4;
      }
      else if (count > 1 && _buffer[0] == 0xfe && _buffer[1] == 0xff)
      {
        _encoding = TextEncoding.Utf16Be;
        offset = 2;
      }
      else if (count > 1 && _buffer[0] == 0xff && _buffer[1] == 0xfe)
      {
        _encoding = TextEncoding.Utf16Le;
        offset = 2;
      }
      else if (count > 3 && _buffer[0] == 0x84 && _buffer[1] == 0x31 && _buffer[2] == 0x95 && _buffer[3] == 0x33)
      {
        _encoding = TextEncoding.Gb18030;
        offset = 4;
      }

      if (offset > 0)
      {
        count -= offset;
        Array.Copy(_buffer, offset, _buffer, 0, count);
        _decoder = _encoding.GetDecoder();
        _confidence = EncodingConfidence.Certain;
      }

      AppendContentFromBuffer(count);
    }

    void ExpandBuffer(Int64 size)
    {
      if (!_finished && _content.Length == 0)
      {
        DetectByteOrderMark();
      }

      while (size + _index > _content.Length && !_finished)
      {
        ReadIntoBuffer();
      }
    }

    void ReadIntoBuffer()
    {
      var returned = _baseStream.Read(_buffer, 0, _bufferSize);
      AppendContentFromBuffer(returned);
    }

    void AppendContentFromBuffer(Int32 size)
    {
      _finished = size == 0;
      var charLength = _decoder.GetChars(_buffer, 0, size, _chars, 0);

      if (_confidence != EncodingConfidence.Certain)
      {
        _raw.Write(_buffer, 0, size);
      }

      _content.Append(_chars, 0, charLength);
    }

    #endregion

    #region Confidence

    enum EncodingConfidence : byte
    {
      Tentative,
      Certain,
      Irrelevant
    }

    #endregion
    
    public static implicit operator TextSource(string value)
    {
      return new TextSource(value);
    }
    public static implicit operator TextSource(Stream value)
    {
      return new TextSource(value);
    }
  }
}
