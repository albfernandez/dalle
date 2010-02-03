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
using System.IO.Compression;

using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

using Dalle.Utilidades;
using Dalle.Compression.LZMA;
using Dalle.Streams;

namespace Dalle.Formatos.Tar
{


	public class Tar : Parte
	{

		public Tar ()
		{
			nombre = "tar";
			descripcion = "tar";
			web = "http://www.gnu.org/software/tar/";
			parteFicheros = false;
			compatible = false;
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			if (!File.Exists (fichero)) {
				return;
			}
			FileInfo fi = new FileInfo (fichero);
			long datosTotales = fi.Length;
			long uncompressedSize = 0;
			FileStream input = File.OpenRead (fichero);
			Stream input2 = input;
			
			if (fichero.ToLower ().EndsWith (".bz2") || fichero.ToLower ().EndsWith (".tbz2") || fichero.ToLower ().EndsWith (".tbz")) 
			{
				// No hay forma de saber el tamaño descomprimido de un bz2 de forma inmediata
				input2 = new BZip2InputStream (input);
			} 
			else if (fichero.ToLower ().EndsWith (".gz") || fichero.ToLower ().EndsWith (".tgz")) 
			{
				uncompressedSize = Dalle.Formatos.GZip.GZip.GetUncompressedSize (input);
				input2 = new GZipStream (input, CompressionMode.Decompress);
			}
			else if (fichero.ToLower ().EndsWith (".tar.lzma") || fichero.ToLower ().EndsWith ("tlz"))
			{
				input2 = new LZMAInputStream (input);
				uncompressedSize = ((LZMAInputStream)input2).UncompressedSize;
			}
			TarInputStream tarInput = new TarInputStream (input2);
			
			TarEntry tarEntry = null;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			OnProgress (0, 1);
			long transferidos = 0;
			while ((tarEntry = tarInput.GetNextEntry ()) != null)
			{
				// Tamaño de la cabecera de la entrada.
				// Nota: TarInputStream ignora sileciosamente algunas entradas,
				// por lo que el progreso no será totalmente preciso.
				transferidos += 512;
				if (tarEntry.IsDirectory) 
				{
					continue;
				}
				Stream entrada = new SizeLimiterStream (tarInput, tarEntry.Size);
				Stream salida = UtilidadesFicheros.CreateWriter (dirDest + Path.DirectorySeparatorChar + tarEntry.Name);
				
				int leidos = 0;

				while ((leidos = entrada.Read (buffer, 0, buffer.Length)) > 0)
				{
					salida.Write (buffer, 0, leidos);
					transferidos += leidos;
					if (uncompressedSize > 0) 
					{
						OnProgress (transferidos, uncompressedSize);
					}
					else {
						OnProgress (input.Position, datosTotales);
					}
				}
				salida.Close ();
				transferidos += 512 - (tarEntry.Size % 512);				
			}
			tarInput.Close ();
			OnProgress (1, 1);
		}
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)) 
			{
				return false;
			}
			string l = fichero.ToLower ();
			return 
					l.EndsWith (".tar.lzma") || 
					l.EndsWith ("tlz") ||
					l.EndsWith (".tar.bz2") || 
					l.EndsWith (".tar.gz") || 
					l.EndsWith (".tgz") ||
					l.EndsWith (".tbz2") ||
					l.EndsWith (".tbz") ||
					l.EndsWith (".tar");

		}
		
		
	}
}


