/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Dalle1.DalleStream
	
    Copyright (C) 2010  Alberto Fernández <infjaf@gmail.com>

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
using System.Collections.Generic;
using System.Security.Cryptography;

using Dalle.Streams;

namespace Dalle.Formatos.Dalle1
{
	public class DalleStream : Stream
	{
		private List<DalleFile> listaFicheros;
		private HashStream currentStream = null;
		private int currentIndex = -1;
		private DalleFile currentFile = null;
		private long length = 0;
		private long position = 0;
		public DalleStream (FileInfo fichero)
		{
			listaFicheros = getListaFicheros (fichero);
			this.length = calculateLength (listaFicheros);
		}
		private long calculateLength (List<DalleFile> lista)
		{
			long tmp = 0;
			foreach (DalleFile d in lista) {
				tmp += d.File.Length;
			}
			return tmp;
			
		}
		private List<DalleFile> getListaFicheros (FileInfo fichero)
		{
			DirectoryInfo dirFiles = fichero.Directory;
			List<DalleFile> ret = new List<DalleFile> ();
			TextReader reader = File.OpenText (fichero.FullName);
			string linea;
			linea = reader.ReadLine ();
			while (linea != null) {
				int idx = linea.IndexOf (';');
				if (idx >= 0)
					linea = linea.Substring (0, linea.IndexOf (';'));
				linea = linea.Trim ();
				if (linea != string.Empty) {
					try {
						string fname = linea.Substring (linea.IndexOf (" ")).Trim ();
						string hash = linea.Substring (0, linea.IndexOf (" ")).Trim ();
						fname = fname.Replace ('/', Path.DirectorySeparatorChar);
						
						DalleFile el = new DalleFile (new FileInfo(dirFiles.FullName + Path.DirectorySeparatorChar + fname), hash);
						ret.Add (el);
					} catch (System.Exception) {
					}
				}
				linea = reader.ReadLine ();
			}
			reader.Close ();
			return ret;
		}
		public override int Read (byte[] buffer, int offset, int count)
		{
			if (currentStream == null) {
				if (currentIndex == Int32.MaxValue){
					return 0;
				}
				if (currentIndex < listaFicheros.Count) {
					currentIndex++;
				}
				if (currentIndex >= listaFicheros.Count) {
					return 0;
				}	
				currentFile = listaFicheros[currentIndex];				
				currentStream = new HashStream (
					File.OpenRead (currentFile.File.FullName),
					HashAlgorithm.Create ("SHA512"),
					currentFile.Hash
				);
			}
			int tmp = 0;
			tmp = currentStream.Read (buffer, offset, count);
			position += tmp;
			if (tmp < count) {
				try {
					currentStream.Close ();
				}
				catch (IOException e) {
					throw new IOException (currentFile.File + " " + e.Message);
				}
				currentStream = null;
				tmp += this.Read(buffer, offset+tmp, count - tmp);
			}
			return tmp;
		}
		public override int ReadByte ()
		{
			return base.ReadByte ();
		}
		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException ();
		}
		public override void WriteByte (byte value)
		{
			base.WriteByte (value);
		}
		public override void Flush ()
		{
			throw new NotImplementedException ();
		}
		public override void Close ()
		{
			currentIndex = Int32.MaxValue;
			if (currentStream != null) {
				try {					
					currentStream.Close ();
					currentStream = null;
				} catch (IOException e) {
					throw new IOException (currentFile.File + " " + e.Message);
				}
			}
		}
		public override long Position {
			get {
				return position;
			}
			set {
				throw new NotImplementedException ();
			}
		}
		public override long Length {
			get {
				return this.length;
			}
		}
		public override bool CanWrite {
			get {
				return false;
			}
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
			throw new NotImplementedException ();
		}
		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}
	}
}

