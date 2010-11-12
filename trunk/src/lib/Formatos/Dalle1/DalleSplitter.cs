/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Dalle1.DalleSplitter
	
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
using System.Collections.Generic;

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Formatos.Generico;

using System.IO.Compression;
using ICSharpCode.SharpZipLib.Tar;

namespace Dalle.Formatos.Dalle1
{
	public class DalleSplitter
	{
		private DirectoryInfo output;
		private String baseName;
		private long kb;
		List<FileInfo> files = new List<FileInfo>();
		public DalleSplitter (DirectoryInfo input, DirectoryInfo output, String baseName, long kb)
		{
			this.output = output;
			this.baseName = baseName;
			this.kb = kb;
			
			Add (input);
		}
		private void Add (DirectoryInfo info)
		{
			DirectoryInfo[] x = info.GetDirectories ();
			foreach (DirectoryInfo di in x) {
				Add (di);
			}
			files.AddRange (info.GetFiles());

		}
		public DalleSplitter (FileInfo input, DirectoryInfo output, String baseName, long kb)
		{
			this.output = output;
			this.baseName = baseName;
			this.kb = kb;
			files.Add (input);
		}
		public void Do ()
		{
			long totalSize = calculateTotalSize ();
			long fragments = totalSize / (kb*1024);
			string s = ""+fragments;
			InfoGenerico info = new InfoGenerico();
			info.OriginalFile = baseName;
			info.InitialFragment = 0;
			info.Digits = Math.Max(s.Length, 3);
			info.BaseName = baseName + ".tar.gz.";
			info.Directory = output;
			info.Length = totalSize;
			
			Stream stream = new SplitStream(info, kb*1024, output.FullName + Path.DirectorySeparatorChar + info.BaseName + "sha512sum.dalle", "SHA512");
			stream = new GZipStream(stream, CompressionMode.Compress);
			TarOutputStream taros = new TarOutputStream(stream);
			foreach(FileInfo f in files)
			{
				
				TarEntry te =  TarEntry.CreateEntryFromFile(f.FullName);
				te.UserId = 0;
				te.GroupId = 0;
				te.UserName=String.Empty;
				te.GroupName=String.Empty;
				taros.PutNextEntry(te);
				FileStream fs = f.OpenRead();
				byte[] buffer = new byte[Consts.BUFFER_LENGTH];
				int leidos = 0;
				while ((leidos = fs.Read(buffer,0,buffer.Length)) > 0)
				{
					taros.Write(buffer, 0, leidos);
					//OnProgress(leidosTotales , totalSize);
				}
				taros.CloseEntry();				
			}
			taros.Flush();
			taros.Close();
			
		}
		private long calculateTotalSize ()
		{
			long counter = 0;
			foreach (FileInfo fi in files) 
			{
				counter += fi.Length;
			}
			return counter;
		}
			
	}
}

