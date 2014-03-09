/*

    Dalle - A split/join file utility library
	
    Copyright (C) 2003-2014  Alberto Fernández <infjaf@gmail.com>

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

namespace Dalle.Streams
{
	public class JoinStream : Stream {
		private IJoinInfo info;
		private long alreadyRead = 0;
		private int currentFragment = 0;
		private Stream currentStream = null;
		public JoinStream (IJoinInfo info) {
			this.info = info;
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
				return alreadyRead;
			}
			set {
				throw new System.NotImplementedException ();
			}
		}
		public override void Flush ()
		{
			throw new System.NotImplementedException ();
		}
		public override int Read (byte[] buffer, int offset, int count)
		{
			if (currentFragment >= info.FragmentsNumber) {
				return 0;
			}
			int bytesRead = 0;			
			if (currentStream == null) {
				CreateStream(currentFragment++);
			}
			bytesRead = currentStream.Read(buffer, offset, count);
			if (bytesRead < count) {
				if (currentFragment >= info.FragmentsNumber) {
					return 0;
				}
				CreateStream(currentFragment++);
				return bytesRead += Read(buffer, offset+bytesRead, count - bytesRead);				
			}
			return bytesRead;

		}
		private void CreateStream (int fragment) {
			if (currentStream != null) {
				currentStream.Close();
			}
			currentStream = File.OpenRead (info.GetFragmentFullPath(fragment));
			currentStream.Seek(info.GetOffset(fragment), SeekOrigin.Begin);
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
		public override void Close() {
			if (currentStream != null) {
				currentStream.Close();
			}
		}
		
	
	
	}




}
