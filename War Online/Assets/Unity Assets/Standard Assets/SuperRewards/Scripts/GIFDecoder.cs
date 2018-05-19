using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace Playerize
{
	/// <summary>
	/// GIF image decoder.
	/// </summary>
	public class GIFDecoder
	{
		public enum DecodeStatus { OK, FormatError, OpenError }

		/// <summary>
		/// The status of the image decoding.
		/// </summary>
		public DecodeStatus status { get; private set; }

		/// <summary>
		/// The texture object available after decoding.
		/// </summary>
		public Texture2D texture { get; private set; }

		/// <summary>
		/// Whether an error was encountered during decoding.
		/// </summary>
		bool errorEncountered { get { return status != DecodeStatus.OK; } }

		/// <summary>
		/// Full image width.
		/// </summary>
		int width;

		/// <summary>
		/// Full image height.
		/// </summary>
		int height;

		/// <summary>
		/// Whether the global color table is used.
		/// </summary>
		bool gctFlag;

		/// <summary>
		/// Whether the local color table is used.
		/// </summary>
		bool lctFlag;

		/// <summary>
		/// Size of the global color table.
		/// </summary>
		int gctSize;

		/// <summary>
		/// Size of the local color table.
		/// </summary>
		int lctSize;

		/// <summary>
		/// Global color table.
		/// </summary>
		int[] gct;

		/// <summary>
		/// Local color table.
		/// </summary>
		int[] lct;

		/// <summary>
		/// Active color table.
		/// </summary>
		int[] act;

		/// <summary>
		/// Whether the image is interlaced.
		/// </summary>
		bool interlace;

		int ix, iy, iw, ih; // Image rectangle
		byte[] block = new byte[256]; // Current data block
		int blockSize;

		/// <summary>
		/// Reads the raw bytes of a GIF image.
		/// </summary>
		/// <param name="data">The byte array representing the GIF.</param>
		public GIFDecoder (byte[] data)
		{
			if (data == null) {
				status = DecodeStatus.OpenError;
				return;
			}

			var stream = new MemoryStream(data);
			ReadHeader(stream);

			if (!errorEncountered) {
				ReadContents(stream);
			}

			stream.Close();
		}

		/// <summary>
		/// Reads GIF file header.
		/// This is a fixed-length header, either "GIF87a" or "GIF89a".
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		void ReadHeader (Stream stream)
		{
			var buffer = new byte[6];
			stream.Read(buffer, 0, buffer.Length);
			var header = Encoding.UTF8.GetString(buffer);

			if (!header.StartsWith("GIF")) {
				status = DecodeStatus.FormatError;
				return;
			}

			ReadLSD(stream);

			if (gctFlag && !errorEncountered) {
				gct = ReadColorTable(stream, gctSize);
			}
		}

		/// <summary>
		/// Reads the logical screen descriptor.
		/// This provides size information and other characteristics.
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		void ReadLSD (Stream stream)
		{
			width = ReadShort(stream);
			height = ReadShort(stream);

			var packed = Read(stream);
			// Bits:
			//     1:   Global Color Table (GCT) flag
			//     2-4: Color resolution (skipped)
			//     5:   GCT sort flag (skipped)
			//     6-8: GCT size
			gctFlag = (packed & 0x80) != 0;
			gctSize = 2 << (packed & 7);

			// Ignore:
			Read(stream); // Background image index
			Read(stream); // Pixel aspect ratio
		}

		/// <summary>
		/// Reads color table as 256 RGB integer values.
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		/// <param name="colorCount">Number of colors to read.</param>
		/// <returns>Integer array containing 256 colors (packed ARGB with full alpha).</returns>
		int[] ReadColorTable (Stream stream, int colorCount)
		{
			var table = new int[256]; // Max size to avoid bounds checks
			var buffer = new byte[colorCount * 3];
			var bytesWritten = 0;

			try {
				bytesWritten = stream.Read(buffer, 0, buffer.Length);
			} catch (IOException) {}

			if (bytesWritten < buffer.Length) {
				status = DecodeStatus.FormatError;
			} else {
				var count = 0;

				for (int i = 0; i < colorCount; i++) {
					uint r = (uint)buffer[count++] & 0xff;
					uint g = (uint)buffer[count++] & 0xff;
					uint b = (uint)buffer[count++] & 0xff;
					table[i] = (int)(0xff000000 | (r << 16) | (g << 8) | b);
				}
			}

			return table;
		}

		/// <summary>
		/// Main file parser. Reads GIF content blocks.
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		void ReadContents (Stream stream)
		{
			var done = false;

			while (!(done || errorEncountered)) {
				var code = Read(stream);

				switch (code) {
					case 0x2c: // Image separator
						ReadImage(stream);
						done = true;
						break;

					case 0x21: // Extension
						// If we wanted to support animations this would be necessary.
						code = Read(stream);
						Skip(stream);
						break;

					case 0x3B: // Terminator
						done = true;
						break;

					case 0x00: // Bad byte, but keep going and see what happens
						break;

					default:
						status = DecodeStatus.FormatError;
						break;
				}
			}
		}

		/// <summary>
		/// Reads the next frame image.
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		void ReadImage (Stream stream)
		{
			// (Sub)image position and size.
			ix = ReadShort(stream);
			iy = ReadShort(stream);
			iw = ReadShort(stream);
			ih = ReadShort(stream);

			var packed = Read(stream);
			// Bits:
			//     1:   Local Color Table (LCT) flag
			//     2:   Interlace flag
			//     3:   Sort flag (skipped)
			//     4-5: Reserved (skipped)
			//     6-8: LCT size
			lctFlag = (packed & 0x80) != 0;
			interlace = (packed & 0x40) != 0;
			lctSize = 2 << (packed & 7);

			if (lctFlag) {
				// Make local table active.
				lct = ReadColorTable(stream, lctSize);
				act = lct;
			} else {
				// Make global table active.
				act = gct;
			}

			if (act == null) {
				// No color table defined.
				status = DecodeStatus.FormatError;
			}

			if (errorEncountered) { return; }

			var imageData = DecodeImageData(stream);
			Skip(stream);

			if (errorEncountered) { return; }

			texture = new Texture2D(width, height);
			var pixels = GeneratePixelArray(imageData);
			ApplyPixelsToTexture(texture, pixels);
		}

		/// <summary>
		/// Decodes LZW image data into pixel array.
		/// Adapted from John Cristy's ImageMagick.
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		/// <returns>Image color values.</returns>
		byte[] DecodeImageData (Stream stream)
		{
			const int nullCode = -1;
			const int maxStackSize = 4096;
			var pixelCount = iw * ih;
			var imageData = new byte[pixelCount];

			// LZW decoder working arrays.
			var prefix = new short[maxStackSize];
			var suffix = new byte[maxStackSize];
			var pixelStack = new byte[maxStackSize + 1];

			// Initialize GIF data stream decoder.
			int dataSize = Read(stream);
			int clear = 1 << dataSize;
			int endOfInformation = clear + 1;
			int available = clear + 2;
			int oldCode = nullCode;
			int codeSize = dataSize + 1;
			int codeMask = (1 << codeSize) - 1;
			int bits, code, count, datum, first, i, top, bi, pi;
			bits = count = datum = first = top = bi = pi = 0;

			for (code = 0; code < clear; code++) {
				prefix[code] = 0;
				suffix[code] = (byte) code;
			}

			//  Decode GIF pixel stream.
			for (i = 0; i < pixelCount;) {
				if (top == 0) {
					if (bits < codeSize) {
						//  Load bytes until there are enough bits for a code.
						if (count == 0) {
							// Read a new data block.
							count = ReadBlock(stream);

							if (count <= 0) {
								break;
							}

							bi = 0;
						}

						datum += (((int)block[bi]) & 0xff) << bits;
						bits += 8;
						bi++;
						count--;
						continue;
					}

					// Get the next code:

					code = datum & codeMask;
					datum >>= codeSize;
					bits -= codeSize;

					// Interpret the code:

					if ((code > available) || (code == endOfInformation)) {
						break;
					}

					if (code == clear) {
						// Reset decoder.
						codeSize = dataSize + 1;
						codeMask = (1 << codeSize) - 1;
						available = clear + 2;
						oldCode = nullCode;
						continue;
					}

					if (oldCode == nullCode) {
						pixelStack[top++] = suffix[code];
						oldCode = code;
						first = code;
						continue;
					}

					var inCode = code;

					if (code == available) {
						pixelStack[top++] = (byte)first;
						code = oldCode;
					}

					while (code > clear) {
						pixelStack[top++] = suffix[code];
						code = prefix[code];
					}

					first = ((int)suffix[code]) & 0xff;

					//  Add a new string to the string table:

					if (available >= maxStackSize) {
						break;
					}

					pixelStack[top++] = (byte)first;
					prefix[available] = (short)oldCode;
					suffix[available] = (byte)first;
					available++;

					if (((available & codeMask) == 0) && (available < maxStackSize)) {
						codeSize++;
						codeMask += available;
					}

					oldCode = inCode;
				}

				// Pop a pixel off the pixel stack.
				top--;
				imageData[pi++] = pixelStack[top];
				i++;
			}

			// Clear missing pixels.
			for (i = pi; i < pixelCount; i++) {
				imageData[i] = 0;
			}

			return imageData;
		}

		/// <summary>
		/// Applies a pixel array to a texture.
		/// </summary>
		/// <param name="imageData">Image color values.</param>
		/// <returns>Pixel array.</returns>
		int[] GeneratePixelArray (byte[] imageData)
		{
			var pixels = new int[width * height * 3];
			var pass = 1;
			var inc = 8;
			var iline = 0;

			// Copy each source line to the appropriate place in the destination.
			for (int i = 0; i < ih; i++) {
				var line = i;

				if (interlace) {
					if (iline >= ih) {
						pass++;

						switch (pass) {
							case 2:
								iline = 4;
								break;
							case 3:
								iline = 2;
								inc = 4;
								break;
							case 4:
								iline = 1;
								inc = 2;
								break;
						}
					}

					line = iline;
					iline += inc;
				}

				line += iy;

				if (line < height) {
					int k = line * width;
					int dx = k + ix; // Start of line in destination
					int dlim = dx + iw; // End of destination line

					if ((k + width) < dlim) {
						dlim = k + width; // Past destination edge
					}

					int sx = i * iw; // Start of line in source

					while (dx < dlim) {
						// Map color and insert in destination.
						int index = ((int)imageData[sx++]) & 0xff;
						int c = act[index];

						if (c != 0) {
							pixels[dx] = c;
						}

						dx++;
					}
				}
			}

			return pixels;
		}

		/// <summary>
		/// Applies a pixel array to a texture.
		/// </summary>
		/// <param name-"texture">Texture to apply the values to.</param>
		/// <param name="pixels">Pixel color values.</param>
		void ApplyPixelsToTexture (Texture2D texture, int[] pixels)
		{
			var count = 0;

			for (int y = 0; y < texture.height; y++) {
				for (int x = 0; x < texture.width; x++) {
					var parts = BitConverter.GetBytes(pixels[count++]);
					float a = parts[3] / 255f;
					float r = parts[2] / 255f;
					float g = parts[1] / 255f;
					float b = parts[0] / 255f;
					var color = new Color(r, g, b, a);
					texture.SetPixel(x, height - y - 1, color);
				}
			}

			texture.Apply();
		}

		/// <summary>
		/// Reads a single byte from the input stream.
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		/// <returns>Resulting byte.</returns>
		int Read (Stream stream)
		{
			var curByte = 0;

			try {
				curByte = stream.ReadByte();
			} catch (IOException) {
				status = DecodeStatus.FormatError;
			}

			return curByte;
		}

		/// <summary>
		/// Reads next variable length block from stream.
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		/// <returns>Number of bytes stored in "buffer".</returns>
		int ReadBlock (Stream stream)
		{
			blockSize = Read(stream);
			var n = 0;

			if (blockSize > 0) {
				try {
					while (n < blockSize) {
						var count = stream.Read(block, n, blockSize - n);

						if (count == -1) {
							break;
						}

						n += count;
					}
				} catch (IOException) {}

				if (n < blockSize) {
					status = DecodeStatus.FormatError;
				}
			}

			return n;
		}

		/// <summary>
		/// Reads the next 16-bit value in a stream, LSB first.
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		/// <returns>Short converted to an integer.</returns>
		int ReadShort (Stream stream)
		{
			return Read(stream) | (Read(stream) << 8);
		}

		/// <summary>
		/// Skips variable length blocks up to and including next zero length blocks.
		/// </summary>
		/// <param name="stream">Byte stream.</param>
		void Skip (Stream stream)
		{
			do {
				ReadBlock(stream);
			} while ((blockSize > 0) && !errorEncountered);
		}
	}
}
