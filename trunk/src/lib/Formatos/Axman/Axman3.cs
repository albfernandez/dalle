/*

    Dalle - A split/join file utility library	
    Dalle.Formatos.Axman.Axman3 - Split and Join files in Axman3 format.
	
    Copyright (C) 2003  Alberto Fern�ndez <infjaf00@yahoo.es>

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

*/

using System;
using System.IO;

using Dalle.Utilidades;
using Dalle.Checksums;
using Dalle.Formatos.Zip;


namespace Dalle.Formatos.Axman
{

	public class Axman3 : Axman
	{
		private CRC crc = new AxmanCRC();
		
		public Axman3()
		{
			nombre = "axman3";
			descripcion = "Axman 3";
			web = "http://www.mosaicware.com/";
			parteFicheros = true;
			compatible = false;
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			
			long transferidos = 0;
			String formato = "";
			bool comprimido = false;
			
			String bas = fichero.Substring (0, fichero.LastIndexOf('.', fichero.Length - 7));

			if (bas.EndsWith("ZIP")){
				comprimido = true;
			}
			formato = bas + ".{0}." + fichero.Substring(fichero.Length -5);
			
			int i = 1;
			String f = String.Format (formato, i);
			while (File.Exists(f)){
				i++;
				f = String.Format(formato, i);
			}			
			f = String.Format(formato, i-1);

			// TODO: Terminar el CRC y comprobarlo.
			

			ColaAxman c = ColaAxman3.LoadFromFile (f);

			String destino = dirDest + Path.DirectorySeparatorChar + c.Nombre;
			UtilidadesFicheros.ComprobarSobreescribir (destino);
			for (i=1; i < c.Fragmentos; i++){

				f = String.Format (formato, i);
				CabeceraAxman cabAxman = CabeceraAxman.LoadFromFile (f);
				if (cabAxman.Fragmento != i){
					throw new Exception ("Fragmento fuera de rango");
				}
				crc.Reset();
				transferidos += UtilidadesFicheros.CopiarIntervalo (f, destino, 23, crc);
				// TODO: Cuando funcione crc, descomentar estas lineas.
				// if (crc.Value != cabAxman.Checksum)
				// 		throw new Exception ("CRC incorrecto");
				OnProgress (transferidos, c.TamanoOriginal);
			}
			
			// El �ltimo fragmento...
			f = String.Format (formato, i);
			crc.Reset();
			transferidos += UtilidadesFicheros.CopiarIntervalo (f, destino, 23, c.TamanoOriginal - transferidos,crc);
			// TODO: Cuando funcione crc, descomentar estas lineas.
			// if (crc.Value != cabAxman.Checksum)
			// 		throw new Exception ("CRC incorrecto");
			OnProgress (transferidos, c.TamanoOriginal);
			
			if (comprimido){
				Zip zip = new Zip();
				zip.Progress += new ProgressEventHandler (this.OnProgress);
				zip.Unir (destino);
			}
			
		}
		protected override void _Partir (string fichero,string sal1, String dir, long kb)
		{
			String formato = dir + Path.DirectorySeparatorChar + sal1 + ".{0}.axman";
			int i = 1;
			long transferidos = 0;
			long totales = new FileInfo(fichero).Length;
			if (totales < (kb*1024))
				throw new Exception ("El fichero es m�s peque�o que los fragmentos");
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
			return fichero.ToUpper().EndsWith(".AXMAN");
		}
	}
}
