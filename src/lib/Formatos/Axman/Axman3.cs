/*

    Dalle - A split/join file utility library	
    Dalle.Formatos.Axman.Axman3 - Split and Join files in Axman3 format.
	
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

using Dalle.Utilidades;
using Dalle.Checksums;
using Dalle.Formatos.Zip;
using Dalle.Streams;

namespace Dalle.Formatos.Axman
{

	public class Axman3 : Axman
	{
		private CRC crc = new AxmanCRC();
		
		public Axman3() : base ("axman3", "Axman 3", "http://www.mosaicware.com/", false, false)
		{
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			
			long transferidos = 0;
			string formato = "";
			bool comprimido = false;
			
			string bas = fichero.Substring (0, fichero.LastIndexOf ('.', fichero.Length - 7));

			if (bas.ToLower ().EndsWith (".zip"))
			{
				comprimido = true;
			}
	
			formato = bas + ".{0}." + fichero.Substring (fichero.Length - 5);
			
			int i = 1;
			string f = String.Format (formato, i);
			while (File.Exists (f)) 
			{
				i++;
				f = String.Format (formato, i);
			}
			f = String.Format (formato, i - 1);
			ColaAxman c = ColaAxman3.LoadFromFile (f);
			string destino = dirDest + Path.DirectorySeparatorChar + c.Nombre;
			Stream os = UtilidadesFicheros.CreateWriter (destino);
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			for (i = 1; i <= c.Fragmentos; i++)
			{
				f = String.Format (formato, i);
				
				Stream ins = File.OpenRead (f);
				if (23 != ins.Read (buffer, 0, 23))
				{
					throw new IOException ("Unexpected end of file:" + f);
				}
				CabeceraAxman cabAxman = CabeceraAxman.LoadFromArray (buffer);
				if (cabAxman.Fragmento != i) {
					throw new Dalle.Formatos.FileFormatException ();
				}
				crc.Reset ();
				Stream inStream = ins;
				if (i == c.Fragmentos) {
					inStream = new SizeLimiterStream (ins, c.TamanoOriginal - transferidos);
				}
				while ((leidos = inStream.Read (buffer, 0, buffer.Length)) > 0)
				{
					os.Write (buffer, 0, leidos);
					transferidos += leidos;
					OnProgress (transferidos, c.TamanoOriginal);
				}
				ins.Close ();

			}
			os.Close ();
			if (comprimido) {
				Dalle.Formatos.Zip.Zip zip = new Dalle.Formatos.Zip.Zip ();
				zip.Progress += new ProgressEventHandler (this.OnProgress);
				zip.Unir (destino);
				try {
					File.Delete (destino);
				}
				catch (Exception) 
				{
				}
			}
			
		}
		protected override void _Partir (string fichero,string sal1, string dir, long kb)
		{
			if ((sal1 == null) || (sal1 == string.Empty))
				sal1 = new FileInfo (fichero).Name;
			string formato = dir + Path.DirectorySeparatorChar + sal1 + ".{0}.axman";
			int i = 1;
			long transferidos = 0;
			long totales = new FileInfo(fichero).Length;
			//if (totales < (kb*1024)){
			//	string msg = string.Format (I.__("Source file is too small"));
			//	throw new Exception ("El fichero es más pequeño que los fragmentos");
			//}
			long tf = kb*1024 - 23;
			OnProgress (0,1);
			CabeceraAxman cab = new CabeceraAxman ();
			AxmanCRC crc = new AxmanCRC();
			do {
				
				cab.Fragmento = i;
				crc.Reset();
				String destino = String.Format(formato, i);
				UtilidadesFicheros.ComprobarSobreescribir(destino);
				UtilidadesFicheros.Append (destino, cab.ToByteArray());
				transferidos += UtilidadesFicheros.CopiarIntervalo (
					fichero, destino, transferidos, tf, crc);
				cab.Checksum = (int) crc.Value;
				
				// Escribimos la cabecera.
				UtilidadesFicheros.Sobreescribir (destino, cab.ToByteArray(), 0);
				
				
				if (transferidos == totales){
					//Escribimos la cola.
					ColaAxman3 cola = new ColaAxman3();
					cola.FicheroOriginal = "C:\\temp\\"+fichero;
					cola.Version = "AXMAN_03-12r   ";
					cola.Ver = 3;
					cola.Fragmentos = i;
					cola.TamanoOriginal = totales;
					UtilidadesFicheros.Append (destino, cola.ToByteArray());
				}
				OnProgress (transferidos, totales);
				
				i++;			
			}while (transferidos < totales);
		}
		
		public override bool PuedeUnir (string fichero)
		{
			if (! File.Exists(fichero) )
				return false;
			return fichero.ToUpper().EndsWith(".AXMAN");
		}
	}
}
