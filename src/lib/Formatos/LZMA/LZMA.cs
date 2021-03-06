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

using Dalle.Utilidades;
using Dalle.Compression.LZMA;
namespace Dalle.Formatos.LZMA
{


	public class LZMA : Parte
	{

		public LZMA ()
		{
			nombre = "lzma";
			descripcion = "LZMA";
			web = "http://www.7-zip.org/";
			parteFicheros = false;
			compatible = false;
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			if (!File.Exists (fichero)) 
			{
				return;
			}
			FileInfo fi = new FileInfo (fichero);
			string destino = dirDest + Path.DirectorySeparatorChar + fi.Name.Substring (0, fi.Name.LastIndexOf ('.'));
			long datosTotales = fi.Length;
			FileStream input = File.OpenRead (fichero);
			LZMAInputStream lzmaInput = new LZMAInputStream (input);
			datosTotales = lzmaInput.UncompressedSize;
			Stream fos = UtilidadesFicheros.CreateWriter (destino);
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			long transferidos = 0;
			OnProgress (0, datosTotales);
			while ((leidos = lzmaInput.Read (buffer, 0, buffer.Length)) > 0)
			{
				fos.Write (buffer, 0, leidos);
				transferidos += leidos;
				//OnProgress (input.Position, datosTotales);
				OnProgress (transferidos, datosTotales);
			}
			lzmaInput.Close ();
			fos.Close ();
			
		}
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero))
			{
				return false;
			}
			return fichero.ToLower ().EndsWith (".lzma");
		}

		
	}
}
