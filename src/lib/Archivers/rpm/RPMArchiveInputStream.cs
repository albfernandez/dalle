/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2004-2010 Alberto Fernández  <infjaf@gmail.com>
    Original java code: commons-compress, from apache (http://commons.apache.org/compress/)
    C# translation by - Alberto Fernández  <infjaf@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using ICSharpCode.SharpZipLib.BZip2;

using Dalle.Archivers;
using Dalle.Archivers.cpio;
using Dalle.Streams;
using Dalle.Utilidades;
using Dalle.Compression.LZMA;


namespace Dalle.Archivers.rpm
{


	public class RPMArchiveInputStream : ArchiveInputStream
	{
		
		
		
		public static readonly int RPM_LEAD_LENGTH = 96;
		public static readonly uint RPM_LEAD_MAGIC = 0xEDABEEDB;
		
		private byte[] tmpbuf = new byte[4096];
		
		private ArchiveInputStream cpioStream = null;
		private int payloadSize = -1;
		private string payloadCompressor = "gzip";
		private string payloadFormat = "cpio";
		
		private long totalLength = -1;
		
		public override long Length {
			get {
				return this.totalLength;
			}
		}
		
		public override ArchiveEntry GetNextEntry ()
		{
			if (cpioStream != null) {
				return cpioStream.GetNextEntry ();
			}
			return null;
		}
		
		public RPMArchiveEntry GetNextRPMArchiveEntry ()
		{
			throw new System.NotImplementedException ();
		}
		
		

		public RPMArchiveInputStream (Stream inputStream) : base(inputStream)
		{
			this.Init ();
		}
		private void Init ()
		{
			ReadRPMLead ();
			ReadRPMSignature ();
			ReadRPMHeader ();
			if (this.payloadSize > 0) 
			{
				this.totalLength = this.Position + this.payloadSize;
			}
			
			long limitSize = this.inputStream.Length - this.inputStream.Position;
			Stream s = new SizeLimiterStream(this.inputStream,limitSize);
			switch (payloadCompressor)
			{
				case "gzip":
					s = new GZipStream(s, CompressionMode.Decompress);
					break;
				case "bzip2":
					s = new BZip2InputStream (s);
					break;
				case "lzma":
					s = new LZMAInputStream(s);					
					break;
				default:
					throw new IOException("Unsupported payload compression:" + payloadCompressor);
			}
			switch (payloadFormat){
				case "cpio":
					cpioStream = new CpioArchiveInputStream(s);
					break;
				default :
					throw new IOException("Unsupported payload format:" + payloadFormat);
			}
		}
		private void ReadRPMHeader ()
		{
			RpmHeader header = ReadHeader ();
			List<RpmHeaderIndex> indices = this.ReadHeaderIndex (header.NumIndex);
			this.ReadHeaderData (header, indices);
		}
		private void ReadRPMSignature ()
		{
			RpmHeader header = ReadHeader ();
			List<RpmSignatureIndex> indices = this.ReadSignatureIndex (header.NumIndex);
			this.ReadSignatureData (header, indices);
			this.Skip (8 - (header.NumData % 8));
		}
		
		
		private void ReadHeaderData (RpmHeader header, List<RpmHeaderIndex> indices)
		{
			byte[] data = new byte[header.NumData];
			int bRead = this.inputStream.Read (data, 0, data.Length);
			this.Count (bRead);
			if (bRead != data.Length) 
			{
				throw new IOException ("Invalid header");
			}
			foreach (RpmHeaderIndex r in indices) 
			{
				
							
				switch (r.Tag) 
				{
				case RpmHeaderTag.RPMTAG_PAYLOADFORMAT:
					this.payloadFormat = UtArrays.LeerTexto (data, r.Offset);
					break;
				case RpmHeaderTag.RPMTAG_PAYLOADCOMPRESSOR:
					this.payloadCompressor = UtArrays.LeerTexto (data, r.Offset);
					break;
				default:
					break;					
				}
				//PrintDebugInfo(Enum.GetName (r.Tag.GetType (), r.Tag), r.Type, data, r.Offset, r.Count);
			}
			
		}
		private void ReadSignatureData (RpmHeader header, List<RpmSignatureIndex> indices)
		{
			byte[] data = new byte[header.NumData];
			int bRead = this.inputStream.Read (data, 0, data.Length);
			this.Count (bRead);
			if (bRead != data.Length) 
			{
				throw new IOException ("Invalid header");
			}
			foreach (RpmSignatureIndex r in indices) 
			{
				switch (r.Tag) 
				{
				case RpmSignatureTag.RPMSIGTAG_PAYLOADSIZE:
					this.payloadSize = UtArrays.LeerInt32BE (data, r.Offset);
					break;
				default:
					break;
				}
				//PrintDebugInfo(Enum.GetName (r.Tag.GetType (), r.Tag), r.Type, data, r.Offset, r.Count);
			}
		}
		/*
		private void PrintDebugInfo (string tag, RpmType tipo, byte[] data, int offset, int count)
		{
				Console.Write (tag + ":");
				switch (tipo)
				{
				case (RpmType.RPM_STRING_TYPE):
					Console.WriteLine (UtArrays.LeerTexto (data, offset));
					break;
				case (RpmType.RPM_CHAR_TYPE):
					Console.WriteLine (UtArrays.LeerTexto (data, offset, count));
					break;
				case RpmType.RPM_INT16_TYPE:
					Console.WriteLine (UtArrays.LeerInt16BE (data, offset));
					break;
				case RpmType.RPM_INT32_TYPE:
					Console.WriteLine (UtArrays.LeerInt32BE (data, offset));
					break;
				case RpmType.RPM_INT64_TYPE:
					Console.WriteLine (UtArrays.LeerInt64BE (data, offset));
					break;
				case RpmType.RPM_INT8_TYPE:
					Console.WriteLine (data[offset]);
					break;
				case RpmType.RPM_BIN_TYPE:
					Console.WriteLine ("*bin*");
					break;
				case RpmType.RPM_I18NSTRING_TYPE:
					Console.WriteLine ("*i18n*");
					break;
				case RpmType.RPM_NULL_TYPE:
					Console.WriteLine ("*null*");
					break;
				case RpmType.RPM_STRING_ARRAY_TYPE:
					Console.WriteLine ("*strarray*");
					break;
				default:
					break;
						
				}
		}
		*/
		private List<RpmSignatureIndex> ReadSignatureIndex (int numIndex)
		{
			List<RpmSignatureIndex> lista = new List<RpmSignatureIndex> ();
			byte[] index = new byte[16];
			int bRead = 0;
			for (int i = 0; i < numIndex; i++) {
				bRead = this.inputStream.Read (index, 0, index.Length);
				this.Count (bRead);
				if (bRead != index.Length) 
				{
					throw new IOException ("Invalid header");
				}
				RpmSignatureIndex idx = new RpmSignatureIndex ();
				idx.Tag = (RpmSignatureTag)UtArrays.LeerInt32BE (index, 0);
				idx.Type = (RpmType)UtArrays.LeerInt32BE (index, 4);
				idx.Offset = UtArrays.LeerInt32BE (index, 8);
				idx.Count = UtArrays.LeerInt32BE (index, 12);
				
				

				lista.Add (idx);
			}
			
			return lista;
		}
		
		private List<RpmHeaderIndex> ReadHeaderIndex (int numIndex)
		{
			List<RpmHeaderIndex> lista = new List<RpmHeaderIndex> ();
			byte[] index = new byte[16];
			int bRead = 0;
			for (int i = 0; i < numIndex; i++) {
				bRead = this.inputStream.Read (index, 0, index.Length);
				this.Count (bRead);
				if (bRead != index.Length) 
				{
					throw new IOException ("Invalid header");
				}
				RpmHeaderIndex idx = new RpmHeaderIndex ();
				idx.Tag = (RpmHeaderTag)UtArrays.LeerInt32BE (index, 0);
				idx.Type = (RpmType)UtArrays.LeerInt32BE (index, 4);
				idx.Offset = UtArrays.LeerInt32BE (index, 8);
				idx.Count = UtArrays.LeerInt32BE (index, 12);
				
				

				lista.Add (idx);
			}
			
			return lista;			
		}
		
		public RpmHeader ReadHeader ()
		{
			byte[] h = new byte[16];
			int bRead = this.inputStream.Read (h, 0, h.Length);
			this.Count (bRead);
			if (bRead != h.Length) 
			{
				throw new IOException ("Premature end of file");
			}
			return new RpmHeader (h);
		}
		private void Skip (int count)
		{
			int tot = 0;
			int bRead = 0;
			while ((bRead = this.inputStream.Read (tmpbuf, 0, Math.Min (tmpbuf.Length, count - tot))) > 0)
			{
				this.Count (bRead);
				tot += bRead;
			}
		}
		private void ReadRPMLead ()
		{
			byte[] header = new byte[RPM_LEAD_LENGTH];
			int bRead = this.inputStream.Read (header, 0, header.Length);
			this.Count (bRead);
			if (bRead != header.Length) 
			{
				throw new IOException ("Premature end of file");
			}
			
			uint magic = UtArrays.LeerUInt32BE (header, 0);
			if (magic != RPM_LEAD_MAGIC)
			{
				throw new IOException ("Invalid magic in header : " + magic.ToString("X"));
			}
		}
		public override int Read (byte[] data, int offset, int count)
		{
			int bRead = cpioStream.Read (data, offset, count);
			this.Count (bRead);
			return bRead;
		}
		public override void Close()
		{
			if (cpioStream != null) {
				cpioStream.Close();
			}
			this.inputStream.Close();
		}
	}
}
