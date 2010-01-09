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
			string destino = dirDest + Path.DirectorySeparatorChar + fi.Name.Substring (0, fi.Name.LastIndexOf ('.'));
			long datosTotales = fi.Length;
			FileStream input = File.OpenRead (fichero);
			Stream input2 = input;
			if (fichero.ToLower ().EndsWith (".bz2")) 
			{
				input2 = new BZip2InputStream (input);
			} 
			else if (fichero.ToLower ().EndsWith (".gz")) 
			{
				input2 = new GZipStream (input, CompressionMode.Decompress);
			}
			TarInputStream tarInput = new TarInputStream (input2);		
			TarEntry tarEntry = null;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			OnProgress(0, datosTotales);
			while ((tarEntry = tarInput.GetNextEntry()) != null)
			{
				if (tarEntry.IsDirectory) continue;
				Stream entrada = new SizeLimiterStream(tarInput, tarEntry.Size);
				Stream salida = UtilidadesFicheros.CreateWriter(dirDest + Path.DirectorySeparatorChar + tarEntry.Name);
				
				int leidos = 0;
				while ((leidos = entrada.Read(buffer,0,buffer.Length))> 0)
				{
					salida.Write(buffer,0,leidos);
					OnProgress(input.Position, datosTotales);
				}
				salida.Close();
			}
			tarInput.Close();
		}
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)) 
			{
				return false;
			}
			string l = fichero.ToLower ();
			return l.EndsWith (".tar.bz2") || l.EndsWith (".tar.gz") || l.EndsWith (".tar");

		}
		
		
	}
}


