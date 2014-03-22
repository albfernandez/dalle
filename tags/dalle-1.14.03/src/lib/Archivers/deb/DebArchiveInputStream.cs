/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2010 Alberto Fernández  <infjaf@gmail.com>

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
using System.IO;
using System.IO.Compression;

using Dalle.Archivers;
using Dalle.Archivers.ar;
using Dalle.Compression.LZMA;
using Dalle.Streams;

using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;


namespace Dalle.Archivers.deb
{


	public class DebArchiveInputStream : ArchiveInputStream
	{

		private ArArchiveInputStream arIn;
		private TarInputStream tarStream;
		private TarEntry entry;
		private bool finished = false;
		public DebArchiveInputStream (Stream inputStream) : base(inputStream)
		{
			this.arIn = new ArArchiveInputStream (inputStream);
			
			ArArchiveEntry binary = arIn.GetNextArEntry ();
			if (!binary.Name.Equals ("debian-binary"))
			{
				// TODO Customize
				throw new IOException ("Invalid name, expected debian-binary, readed:" + binary.Name);
			}
			ArArchiveEntry control = arIn.GetNextArEntry ();
			if (!control.Name.Equals ("control.tar.gz"))
			{
				throw new IOException ("Invalid name, expected control.tar.gz, readed:" + control.Name);
			}
			ArArchiveEntry data = arIn.GetNextArEntry ();
			Stream compressedStream = null;
			if (data.Name.Equals ("data.tar.gz"))
			{
				compressedStream = new GZipStream (arIn, CompressionMode.Decompress);
				
			}
			else if (data.Name.Equals ("data.tar.bz2")) 
			{
				compressedStream = new BZip2InputStream (arIn);
			}
			else if (data.Name.Equals ("data.tar.bz2"))
			{
				compressedStream = new LZMAInputStream (arIn);
				
				
			}
			else if (data.Name.Equals ("data.tar")) 
			{
				compressedStream = arIn;
			}
			else 
			{
				throw new IOException ("Unsupported compressed data:" + data.Name);
			}
			this.tarStream = new TarInputStream (compressedStream);		

		}
		
		public override ArchiveEntry GetNextEntry ()
		{
			return this.GetNextDebEntry ();
		}
		public DebArchiveEntry GetNextDebEntry ()
		{
			if (finished) {
				return null;
			}
			TarEntry tarEntry = tarStream.GetNextEntry ();
			this.entry = tarEntry;
			if (entry == null) 
			{
				this.finished = true;
				return null;
			}
			if (String.Empty.Equals(tarEntry.Name)){
				return GetNextDebEntry();
			}
			DebArchiveEntry debEntry = new DebArchiveEntry (tarEntry);			
			this.dataStream = new SizeLimiterStream (tarStream, tarEntry.Size);
			return debEntry;			
		}
	}
}
