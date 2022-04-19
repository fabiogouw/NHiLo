﻿using System;
using System.IO;
using System.Text;

namespace NHiLo.Guid
{
    /// <summary>
    /// C# implementation of ASCII85 encoding. 
    /// Based on C code from http://www.stillhq.com/cgi-bin/cvsweb/ascii85/
    /// </summary>
    /// <remarks>
    /// Jeff Atwood
    /// http://www.codinghorror.com/blog/archives/000410.html
    /// </remarks>
    internal class Ascii85
    {
        /// <summary>
        /// Prefix mark that identifies an encoded ASCII85 string, traditionally '<~'
        /// </summary>
        public const string PrefixMark = "<~";
        /// <summary>
        /// Suffix mark that identifies an encoded ASCII85 string, traditionally '~>'
        /// </summary>
        public const string SuffixMark = "~>";
        /// <summary>
        /// Maximum line length for encoded ASCII85 string; 
        /// set to zero for one unbroken line.
        /// </summary>
        public const int LineLength = 75;
        /// <summary>
        /// Add the Prefix and Suffix marks when encoding, and enforce their presence for decoding
        /// </summary>
        public bool EnforceMarks = true;

        private const int _asciiOffset = 33;
        private readonly byte[] _encodedBlock = new byte[5];
        private readonly byte[] _decodedBlock = new byte[4];
        private uint _tuple = 0;
        private int _linePos = 0;

        private readonly uint[] pow85 = { 85 * 85 * 85 * 85, 85 * 85 * 85, 85 * 85, 85, 1 };

        /// <summary>
        /// Decodes an ASCII85 encoded string into the original binary data
        /// </summary>
        /// <param name="s">ASCII85 encoded string</param>
        /// <returns>byte array of decoded binary data</returns>
        public byte[] Decode(string s)
        {
            if (EnforceMarks && !s.StartsWith(PrefixMark) || !s.EndsWith(SuffixMark))
            {
                throw new NHiLoException(ErrorCodes.DecodingASCII85StringError)
                    .WithInfo("reason", $"ASCII85 encoded data should begin with '{ PrefixMark }' and end with '{ SuffixMark }'");
            }

            s = RemoveSufixAndPrefix(s);

            using (MemoryStream ms = new MemoryStream())
            {
                int count = 0;
                bool processChar = false;

                foreach (char c in s)
                {
                    switch (c)
                    {
                        case 'z':
                            if (count != 0)
                            {
                                throw new NHiLoException(ErrorCodes.DecodingASCII85StringError)
                                    .WithInfo("reason", "The character 'z' is invalid inside an ASCII85 block.");
                            }
                            _decodedBlock[0] = 0;
                            _decodedBlock[1] = 0;
                            _decodedBlock[2] = 0;
                            _decodedBlock[3] = 0;
                            ms.Write(_decodedBlock, 0, _decodedBlock.Length);
                            processChar = false;
                            break;
                        case '\n':
                        case '\r':
                        case '\t':
                        case '\0':
                        case '\f':
                        case '\b':
                            processChar = false;
                            break;
                        default:
                            if (c < '!' || c > 'u')
                            {
                                throw new NHiLoException(ErrorCodes.DecodingASCII85StringError)
                                    .WithInfo("reason", $"Bad character '{ c }' found. ASCII85 only allows characters '!' to 'u'.");
                            }
                            processChar = true;
                            break;
                    }

                    if (processChar)
                    {
                        _tuple += ((uint)(c - _asciiOffset) * pow85[count]);
                        count++;
                        if (count == _encodedBlock.Length)
                        {
                            DecodeBlock();
                            ms.Write(_decodedBlock, 0, _decodedBlock.Length);
                            _tuple = 0;
                            count = 0;
                        }
                    }
                }

                // if we have some bytes left over at the end..
                if (count != 0)
                {
                    if (count == 1)
                    {
                        throw new NHiLoException(ErrorCodes.DecodingASCII85StringError)
                            .WithInfo("reason", "The last block of ASCII85 data cannot be a single byte.");
                    }
                    count--;
                    _tuple += pow85[count];
                    DecodeBlock(count);
                    for (int i = 0; i < count; i++)
                    {
                        ms.WriteByte(_decodedBlock[i]);
                    }
                }

                return ms.ToArray();
            }
        }

        private static string RemoveSufixAndPrefix(string s)
        {
            // strip prefix and suffix if present
            if (s.StartsWith(PrefixMark))
            {
                s = s.Substring(PrefixMark.Length);
            }
            if (s.EndsWith(SuffixMark))
            {
                s = s.Substring(0, s.Length - SuffixMark.Length);
            }

            return s;
        }

        /// <summary>
        /// Encodes binary data into a plaintext ASCII85 format string
        /// </summary>
        /// <param name="ba">binary data to encode</param>
        /// <returns>ASCII85 encoded string</returns>
        public string Encode(byte[] ba)
        {
            StringBuilder sb = new StringBuilder((ba.Length * (_encodedBlock.Length / _decodedBlock.Length)));
            _linePos = 0;

            int count = 0;
            _tuple = 0;
            foreach (byte b in ba)
            {
                if (count >= _decodedBlock.Length - 1)
                {
                    _tuple |= b;
                    if (_tuple == 0)
                    {
                        AppendChar(sb, 'z');
                    }
                    else
                    {
                        EncodeBlock(sb);
                    }
                    _tuple = 0;
                    count = 0;
                }
                else
                {
                    _tuple |= (uint)(b << (24 - (count * 8)));
                    count++;
                }
            }

            // if we have some bytes left over at the end..
            if (count > 0)
            {
                EncodeBlock(count + 1, sb);
            }
            return sb.ToString();
        }

        private void EncodeBlock(StringBuilder sb)
        {
            EncodeBlock(_encodedBlock.Length, sb);
        }

        private void EncodeBlock(int count, StringBuilder sb)
        {
            for (int i = _encodedBlock.Length - 1; i >= 0; i--)
            {
                _encodedBlock[i] = (byte)((_tuple % 85) + _asciiOffset);
                _tuple /= 85;
            }

            for (int i = 0; i < count; i++)
            {
                char c = (char)_encodedBlock[i];
                AppendChar(sb, c);
            }

        }

        private void DecodeBlock()
        {
            DecodeBlock(_decodedBlock.Length);
        }

        private void DecodeBlock(int bytes)
        {
            for (int i = 0; i < bytes; i++)
            {
                _decodedBlock[i] = (byte)(_tuple >> 24 - (i * 8));
            }
        }

        private void AppendChar(StringBuilder sb, char c)
        {
            sb.Append(c);
            _linePos++;
            if (LineLength > 0 && (_linePos >= LineLength))
            {
                _linePos = 0;
                sb.Append('\n');
            }
        }

    }
}
