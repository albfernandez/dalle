
using System;
using System.IO;
using System.IO.Compression;

using Dalle.Archivers;
using Dalle.Archivers.cpio;
using Dalle.Streams;
using Dalle.Utilidades;

namespace Dalle.Archivers.rpm
{

	public class RPMHeader {
		
		
		
		int magic;
		int version;
		int numIndex;
		int numData;
		public int Version {
			get {
				return version;
			}
			set {
				version = value;
			}
		}
		
		
		public int NumIndex {
			get {
				return numIndex;
			}
			set {
				numIndex = value;
			}
		}
		
		
		public int NumData {
			get {
				return numData;
			}
			set {
				numData = value;
			}
		}
		
		
		public int Magic {
			get {
				return magic;
			}
			set {
				magic = value;
			}
		}
		
		
		public RPMHeader ()
		{

		}
		public RPMHeader (byte[] data)
		{
			// TODO Leer y comprobar magic
			// TODO leer version
			if (data[0] != 0x8E || data[1] != 0xAD || data[2] != 0xE8){
				throw new IOException("Invalid header magic");
			}
			this.numIndex = UtArrays.LeerInt32BE (data, 8);
			this.numData = UtArrays.LeerInt32BE (data, 12);
		}
		
		/*magic[3];     (3  byte)  (8e ad e8)
   unsigned char version;      (1  byte)
   char reserved[4];           (4  byte)
   int num_index;              (4  byte)
   int num_data;               (4  byte)
   */
	}

	public class RPMArchiveInputStream : ArchiveInputStream
	{
		public static readonly int RPM_LEAD_LENGTH = 96;
		public static readonly uint RPM_LEAD_MAGIC = 0xEDABEEDB;
		
		private byte[] tmpbuf = new byte[4096];
		
		private ArchiveInputStream cpioStream = null;
		
		public override ArchiveEntry GetNextEntry ()
		{
			//return GetNextRPMArchiveEntry ();
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
			
			
			//PAYLOADCOMPRESSOR=gzip
			//PAYLOADFORMAT=cpio
			//PAYLOADFLAGS
			
			GZipStream gzipStream = new GZipStream (new SizeLimiterStream(this.inputStream, -1), CompressionMode.Decompress);
			cpioStream = new CpioArchiveInputStream (gzipStream);
			/*
			Console.WriteLine ("Position = " + this.Position + " 0x" + this.Position.ToString ("X"));
			for (int i = 0; i < 100; i++) 
			{
				Console.WriteLine (this.inputStream.ReadByte ().ToString ("X"));
			}*/
		}
		private void ReadRPMHeader ()
		{
			RPMHeader header = ReadHeader ();
			this.Skip (header.NumIndex * 16);
			this.Skip (header.NumData);
		}
		private void ReadRPMSignature ()
		{
			RPMHeader header = ReadHeader ();
			this.Skip (header.NumIndex * 16);
			this.Skip (header.NumData);
			this.Skip (8 - (header.NumData % 8));
		
		}
		public RPMHeader ReadHeader ()
		{
			byte[] h = new byte[16];
			int bRead = this.inputStream.Read (h, 0, h.Length);
			this.Count (bRead);
			if (bRead != h.Length) 
			{
				throw new IOException ("Premature end of file");
			}
			return new RPMHeader (h);
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
			// 0xEDABEEDB
			//if (header[0] != 0xED || header[])
			
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
