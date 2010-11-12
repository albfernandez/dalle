/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Generico.SplitStream
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf@gmail.com>

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

using Dalle.Streams;
using System.Security.Cryptography;

namespace Dalle.Formatos.Generico
{
	public class SplitStream :Stream
	{
		private int currentFragment= 0;
		private InfoGenerico info = null;
		private Stream currentStream = null;
		private long fragmentSize = 0;
		private string md5File = null;
		private string hashAlg ="SHA512";
		private StreamWriter md5Stream = null;
		private FileInfo currentFileInfo;
		
		private Stream CreateStream ()
		{
			currentFileInfo = new FileInfo (info.GetFragmentFullPath (++currentFragment));
			FileStream fis = currentFileInfo.OpenWrite ();
			if (md5File != null && hashAlg != null) {
				if (md5Stream == null) 
				{
					 md5Stream = new StreamWriter(new FileInfo (md5File).OpenWrite ());
				}
				HashAlgorithm ha = HashAlgorithm.Create (hashAlg);	
				HashStream st = new HashStream (fis, ha);
				return st;
			}
			return fis;			
		}
		
		public SplitStream (InfoGenerico info, long fragmentSize, string md5File, string hashAlg) : this(info, fragmentSize)
		{
			this.md5File = md5File;
			this.hashAlg = hashAlg;
		}
		public SplitStream (InfoGenerico info, long fragmentSize)
		{
			this.info = info;
			this.fragmentSize = fragmentSize;
		}
		public override long Length {
			get {
				throw new NotImplementedException ();
			}
		}
		public override bool CanRead {
			get {
				return false;
			}
		}
		public override bool CanSeek {
			get {
				return false;
			}
		}
		public override bool CanTimeout {
			get {
				return false;
			}
		}
		public override bool CanWrite {
			get {
				return true;
			}
		}
		public override void Close ()
		{
			if (currentStream != null) {
				currentStream.Close ();
			}
			if (md5Stream != null) {
				HashStream hs = (HashStream)currentStream;
				string h = hs.Hash;
				md5Stream.WriteLine (h + "  " + currentFileInfo.Name);
				md5Stream.Close();
				md5Stream = null;
			}
		}
		public override void Flush ()
		{
			if (currentStream != null) {
				currentStream.Flush ();
			}
		}


		public override long Position {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}
		public override int Read (byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException ();
		}
		public override int ReadByte ()
		{
			throw new NotImplementedException ();
		}
		public override void Write (byte[] buffer, int offset, int count)
		{
			if (count == 0)
				return;
			if (currentStream == null) {
				currentStream = CreateStream ();
			}
			if (count < (fragmentSize - currentStream.Length)) {
				currentStream.Write (buffer, offset, count);
			}
			else {
				int c1 = (int)(fragmentSize - currentStream.Length);
				currentStream.Write (buffer, offset, c1);
				currentStream.Close ();
				if (md5Stream != null) 
				{
					HashStream h = (HashStream)currentStream;
					String hs = h.Hash;
					md5Stream.WriteLine (hs + "  " + currentFileInfo.Name);
				}
				currentStream = null;
				if (count > c1) {
					this.Write (buffer, offset + c1, count - c1);
				}
			}
			//throw new NotImplementedException ();
		}
		public override void WriteByte (byte value)
		{
			this.Write (new byte[] { value }, 0, 1);
		}
		
		
		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}
		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}
		public override int ReadTimeout {
			get {
				return base.ReadTimeout;
			}
			set {
				base.ReadTimeout = value;
			}
		}
		public override int WriteTimeout {
			get {
				return base.WriteTimeout;
			}
			set {
				base.WriteTimeout = value;
			}
		}
	}
}

