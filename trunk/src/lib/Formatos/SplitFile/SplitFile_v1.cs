/*

	Dalle - A split/join file utility library
	Dalle.Formatos.SplitFile.SplitFile_v1 - 
		Split and Join files in SplitFile format.
	
    Copyright (C) 2003-2009  Alberto Fernández <infjaf00@yahoo.es>

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
using System.Collections;

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Checksums;

using I = Dalle.I18N.GetText;

namespace Dalle.Formatos.SplitFile
{
	
	
	public class SplitFile_v1 : Parte
	{

		//private CRC crc = new SplitFileCRC ();
		public SplitFile_v1 ()
		{
			/*nombre = "splitfile";
			descripcion = "SplitFile v 1.1";
			web = "http://www.methods.co.nz";
			parteFicheros = true;
			compatible = false;*/
		}

		protected override void _Unir (string fichero, string dirDest)
		{
			
			/*ArrayList lista = new ArrayList ();
			string b = fichero.Substring(0, fichero.Length - 4);
			for (int f=1; File.Exists(b+f+".sf"); f++){
				lista.Add (Cabecera_sf1.LeerCabecera(b+f+".sf"));
			}			
			Cabecera_sf1 cab = (Cabecera_sf1) lista[lista.Count -1];
			
			if (!cab.IsLast){
				//TODO: indicar cuantos ficheros faltan.ç
				//TODO: I18N
				throw new Exception ("Falta ultimo o ultimos");
			}
			string destino = dirDest + Path.DirectorySeparatorChar + cab.FileName;
			UtilidadesFicheros.ComprobarSobreescribir(destino);

			// Comprobamos que todos pertenecen a la secuencia.
			int i=1;
			foreach (Cabecera_sf1 c in lista){
				if (c.Number != i){
					// TODO: indicar en q fichero.
					string msg = string.Format (I._("Single verification failed"));
					throw new Exception (msg);
				}				
				i++;
			}
			long transferidos = 0;
			OnProgress (0, cab.FileSize);
			foreach (Cabecera_sf1 c in lista){
				crc.Reset();
				long leidos = UtilidadesFicheros.CopiarIntervalo(
					b + c.Number + ".sf", destino, c.CAB_SIZE , crc);
				if ( crc.Value != c.CheckSum){
					// TODO: Indicar en q fichero.
					throw new Exception (I._("Checksum verfication failed"));
					
				}
				transferidos += leidos;		
				OnProgress (transferidos, cab.FileSize);
			}
			File.SetLastWriteTime (cab.FileName, cab.FileTime.ToLocalTime());*/
		}
		protected override void _Partir (string fichero,string salida1, string dir, long kb)
		{
			/*
			Cabecera_sf1 cabecera = new Cabecera_sf1();
			cabecera.FileName = new FileInfo(fichero).Name;
			cabecera.FileSize = (int) (new FileInfo(fichero).Length);
			
			cabecera.FileTime = File.GetLastWriteTime(fichero).ToUniversalTime();
			// Atributos normales por defecto.			
			cabecera.FileAttr = 32;
			int secuencia = 1;
			long transferidos = 0;
			long tamano = (kb * 1024) - 512;
			string bas = new FileInfo (fichero).Name;
			bas = bas.Substring (0, bas.LastIndexOf('.')) + "_";
			OnProgress (0, cabecera.FileSize);
			do {
				String nuevoFrag = bas + secuencia + ".sf";
				crc.Reset();
				
				UtilidadesFicheros.ComprobarSobreescribir (nuevoFrag);
				
				//Reservamos el espacio para la cabecera.
				UtilidadesFicheros.Append (nuevoFrag, cabecera.ToByteArray());

				long tr = UtilidadesFicheros.CopiarIntervalo (
					fichero, nuevoFrag, transferidos, tamano, crc);

				cabecera.Number = secuencia;
				cabecera.DataSize = (int) tr;
				transferidos += tr;
				cabecera.IsLast = (transferidos == cabecera.FileSize);
				cabecera.CheckSum = unchecked ((int) crc.Value);
				UtilidadesFicheros.Sobreescribir (nuevoFrag, cabecera.ToByteArray(), 0);
				secuencia++;
				OnProgress (transferidos, cabecera.FileSize);
				
			}while (transferidos < cabecera.FileSize);*/
		}
		public override bool PuedeUnir (string fichero)
		{
			/*if (! File.Exists (fichero) )
				return false;
			
			if (! fichero.ToUpper().EndsWith(".SF"))
				return false;
			try{
				Cabecera_sf1.LeerCabecera (fichero);
			}
			catch (Exception e){
				return false;
			}
			return true;*/
			return false;
		}
	}
}
