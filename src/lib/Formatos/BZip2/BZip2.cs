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

using Dalle.Utilidades;
namespace Dalle.Formatos.BZip2
{


	public class BZip2 : Parte
	{

		public BZip2 ()
		{
			nombre = "bzip2";
			descripcion = "BZip2";
			web = "http://www.bzip.org/";
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
			BZip2InputStream bzipInput = new BZip2InputStream (input);
			Stream fos = UtilidadesFicheros.CreateWriter (destino);
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			OnProgress (0, datosTotales);
			while ((leidos = bzipInput.Read (buffer, 0, buffer.Length)) > 0) {
				fos.Write (buffer, 0, leidos);
				OnProgress (input.Position, datosTotales);
			}
			bzipInput.Close ();
			fos.Close ();
			
		}
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)) {
				return false;
			}
			return fichero.ToLower ().EndsWith (".bz2");
		}
		
		
	}
}
