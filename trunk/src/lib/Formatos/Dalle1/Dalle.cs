/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Dalle1.Dalle
	
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
using System.IO.Compression;
using System.Collections.Generic;

using ICSharpCode.SharpZipLib.Tar;


using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Formatos.Generico;
using Dalle.Streams;



namespace Dalle.Formatos.Dalle1
{

	
	public class Dalle : Parte
	{
		public Dalle ()
		{
			nombre = "dalle";
			descripcion = "Dalle";
			web = "http://dalle.sourceforge.net";
			compatible = true;
			parteFicheros = false;
		}
		
		public void Unir (FileInfo fichero, DirectoryInfo dirDest)
		{

			DalleStream dstream = new DalleStream (fichero);
			
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			OnProgress (0, dstream.Length);
			Stream gzipStream = new GZipStream (dstream, CompressionMode.Decompress);
			
			TarInputStream tarStream = new TarInputStream (gzipStream);
			
			TarEntry tarEntry = null;

			OnProgress (0, 1);

			while ((tarEntry = tarStream.GetNextEntry ()) != null) {
				// Tamaño de la cabecera de la entrada.
				// Nota: TarInputStream ignora sileciosamente algunas entradas,
				// por lo que el progreso no será totalmente preciso.
				if (tarEntry.IsDirectory) {
					continue;
				}
				Stream entrada = new SizeLimiterStream (tarStream, tarEntry.Size);
				Stream salida = UtilidadesFicheros.CreateWriter (dirDest.FullName + Path.DirectorySeparatorChar + tarEntry.Name);
				
				int leidos = 0;
				
				while ((leidos = entrada.Read (buffer, 0, buffer.Length)) > 0) {
					salida.Write (buffer, 0, leidos);
					OnProgress (dstream.Position, dstream.Length+1); // +1 para evitar llegar al 100% antes de tiempo
				}
				salida.Close ();

			}
			tarStream.Close ();
			OnProgress (1, 1);

		}
		protected override void _Unir (string fichero, string dirDest)
		{
			DirectoryInfo salida = new DirectoryInfo(dirDest);
			FileInfo fi = new FileInfo(fichero);
			Unir(fi, salida);
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			FileInfo fi = null;
			DirectoryInfo din = null;
			DirectoryInfo dout = new DirectoryInfo (dir);


			if (File.Exists (fichero)) {
				fi = new FileInfo (fichero);
			} else if (Directory.Exists (fichero)) {
				din = new DirectoryInfo (fichero);
			} else {
				throw new Exception ("" + fichero + " not found");
			}
			List<FileInfo> files = load (fichero);
			
			string baseName = "";
			if (fi != null) {
				baseName = fi.Name;
			} else if (din != null) {
				baseName = din.Name;
			}
			
			
			if ((sal1 == null) || (sal1 == string.Empty)) {
				//
				if (din != null) {
					sal1 = din.Name;
				}
				if (fi != null) {
					sal1 = fi.Name;
				}
			}
			
			long totalSize = calculateTotalSize (files);
			long fragments = totalSize / (kb * 1024);
			string s = "" + fragments;
			JoinInfo info = new JoinInfo ();
			info.OriginalFile = baseName;
			info.InitialFragment = 0;
			info.Digits = Math.Max (s.Length, 3);
			info.BaseName = sal1 + ".tar.gz.";
			info.Directory = dout;
			info.Length = totalSize;
			
			Stream stream = new SplitStream (info, kb * 1024, info.Directory.FullName + Path.DirectorySeparatorChar + info.BaseName + "sha512sum.dalle", "SHA512");
			stream = new GZipStream (stream, CompressionMode.Compress);
			TarOutputStream taros = new TarOutputStream (stream);
			foreach (FileInfo f in files) {
				
				TarEntry te = TarEntry.CreateEntryFromFile (f.FullName);
				te.UserId = 0;
				te.GroupId = 0;
				te.UserName = String.Empty;
				te.GroupName = String.Empty;
				taros.PutNextEntry (te);
				FileStream fs = f.OpenRead ();
				long leidosTotales = 0;
				byte[] buffer = new byte[Consts.BUFFER_LENGTH];
				int leidos = 0;
				while ((leidos = fs.Read (buffer, 0, buffer.Length)) > 0) {
					taros.Write (buffer, 0, leidos);
					leidosTotales += leidos;
					OnProgress (leidosTotales, totalSize);
				}
				taros.CloseEntry ();
				fs.Close ();
			}			
			taros.Close ();
			OnProgress (totalSize, totalSize);
		}

		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)) {
				return false;
			}
			return fichero.ToLower ().EndsWith (".sha512sum.dalle");
		}

		
		
		private long calculateTotalSize (List<FileInfo> files)
		{
			long counter = 0;
			foreach (FileInfo fi in files) {
				counter += fi.Length;
			}
			return counter;
		}
		private List<FileInfo> load (string fichero)
		{
			List<FileInfo> result = new List<FileInfo> ();
			if (File.Exists (fichero)) {
				FileInfo fi = new FileInfo (fichero);
				Console.WriteLine ("Es un fichero");
				result.Add (fi);
			} else if (Directory.Exists (fichero)) {
				DirectoryInfo din = new DirectoryInfo (fichero);
				Console.WriteLine ("es un directorio");
				load (result, din);
			}
			return result;
		}
		private void load (List<FileInfo> files, DirectoryInfo info)
		{
			DirectoryInfo[] x = info.GetDirectories ();
			foreach (DirectoryInfo di in x) {
				load (files, di);
			}
			files.AddRange (info.GetFiles ());
		}
		
	}
}

