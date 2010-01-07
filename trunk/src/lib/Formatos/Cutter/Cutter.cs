/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Cutter.Cutter - Basic support for cutter files.
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf00@yahoo.es>

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

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;	

using Dalle.Utilidades;

namespace Dalle.Formatos.Cutter
{	
	public class Cutter : Parte 
	{
		//private static int buffSize = 262144;
		
		public Cutter()
		{
			nombre = "cutter";
			descripcion = "Fichero Cutter";
			web = "http://gcutter.free.fr";
			compatible = false;
			parteFicheros = false;
			
		}
		
		protected override void _Unir (string fichero, string dirDest)
		{
			CutterTail tailInicial = CutterTail.LoadFromFile (fichero);
			if (tailInicial != null)
			{
				string destino = dirDest + Path.DirectorySeparatorChar + tailInicial.Original;
				long total = tailInicial.FileSize;
				long transferidos = 0;
				FileStream fos = UtilidadesFicheros.CreateWriter (destino);
				string ficheroBase = fichero.Substring (0, fichero.Length - 1);
				int contador = 1;
				CutterTail tail = null;
				byte[] buffer = new byte[Consts.BUFFER_LENGTH];
				OnProgress (0, total);
				Crc32 crc = new Crc32 ();
				while ((tail = CutterTail.LoadFromFile (ficheroBase + contador)) != null) {
					int leidos = 0;
					int parcial = 0;
					long fileSize = new FileInfo (ficheroBase + contador).Length;
					FileStream fis = File.OpenRead (ficheroBase + contador);
					
					crc.Reset ();
					while ((leidos = fis.Read (buffer, 0, Math.Min ((int)fileSize - CutterTail.TAIL_SIZE - parcial, buffer.Length))) > 0)
					{
						fos.Write (buffer, 0, leidos);
						crc.Update (buffer, 0, leidos);
						parcial += leidos;
						transferidos += leidos;
					}
					fis.Close ();
					if (crc.Value != tail.Crc)
					{
						throw new Dalle.Formatos.ChecksumVerificationException ("checksum failed on file " + contador);
					}
					contador++;					
				}
				fos.Close();
			}
			
		}
		protected override void _Partir (string fichero,string salida1, string dir, long kb)
		{

		}
		public override bool PuedeUnir (string fichero)
		{
			
			if (!File.Exists (fichero))
				return false;
			// TODO Mejorar para que dectecte *.CUT*
			if (!fichero.ToUpper().EndsWith (".CUT1"))
				return false;
			CutterTail tail = CutterTail.LoadFromFile (fichero);
			return (tail != null);
		}	
	}
}
