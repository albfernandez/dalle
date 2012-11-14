/*

    Dalle - A split/join file utility library
	
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

namespace Dalle.Formatos.SplitterMerger
{


	public class SplitterMergerStream : Stream
	{

		
		private SplitterMergerInfo info = null;
		private Stream currentStream = null;
		private int currentFragment = 0;
		private bool finished = false;
		private long bytesReaded = 0;

		public SplitterMergerStream (SplitterMergerInfo info)
		{
			this.info = info;
			currentFragment = 1;
			currentStream = File.OpenRead (info.GetFramentFilename (currentFragment));
			currentStream.Seek (info.GetDataOffset (currentFragment), SeekOrigin.Begin);				
		}
		public override int Read (byte[] buffer, int offset, int count)
		{
			if (finished || currentStream == null)
			{
				return 0;
			}
			int leidos = currentStream.Read(buffer, offset, count);
			bytesReaded += leidos;

			

			if (leidos == count) 
			{
				return leidos;	
			}
			currentStream.Close();
			currentStream = null;
			if (currentFragment == info.Fragments)
			{
				finished = true;
				return leidos;
			}
			currentFragment++;
			currentStream = File.OpenRead(info.GetFramentFilename(currentFragment));
			currentStream.Seek(info.GetDataOffset(currentFragment), SeekOrigin.Begin);
			return leidos + this.Read(buffer, offset+leidos, count-leidos);
			
		}


		public override bool CanRead {
			get {
				return true;
			}
		}
		public override bool CanSeek {
			get {
				return false;
			}
		}
		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new System.NotImplementedException ();
		}

		public override void SetLength (long value)
		{
			throw new System.NotImplementedException ();
		}
		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new System.NotImplementedException ();
		}


		public override bool CanWrite {
			get {
				return false;
			}
		}
		public override long Length {
			get {
				return info.Length;
			}
		}
		public override long Position {
			get {
				return bytesReaded;
			}
			set {
				throw new System.NotImplementedException ();
			}
		}
		public override void Flush ()
		{
			throw new System.NotImplementedException ();
		}




	}
}
