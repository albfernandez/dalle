/*

	Dalle - A split/join file utility library
	Dalle.Formatos.SplitFile.SplitFile_v1 - 
		Split and Join files in SplitFile format.
	
    Copyright (C) 2003  Alberto Fernández <infjaf00@yahoo.es>

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
using System.Collections;

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Checksums;

namespace Dalle.Formatos.SplitFile
{
	
	
	public class SplitFile_v1 : Parte
	{

		private CRC crc = new SplitFileCRC ();
		public SplitFile_v1 ()
		{
			nombre = "splitfile";
			descripcion = "SplitFile v 1.1";
			web = "http://www.methods.co.nz";
			parteFicheros = true;
			compatible = false;
		}

		protected override void _Unir (String fichero, String dirDest)
		{
			
			// TODO:Hacer que utilice la informacion de directorio
			ArrayList lista = new ArrayList ();
			String b = fichero.Substring(0, fichero.Length - 4);
			for (int f=1; File.Exists(b+f+".sf"); f++){
				lista.Add (Cabecera_sf1.LeerCabecera(b+f+".sf"));
			}			
			Cabecera_sf1 cab = (Cabecera_sf1) lista[lista.Count -1];
			
			if (!cab.IsLast){
				//TODO: indicar cuantos ficheros faltan.
				throw new Exception ("Falta ultimo o ultimos");
			}
			String destino = dirDest + Path.DirectorySeparatorChar + cab.FileName;
			UtilidadesFicheros.ComprobarSobreescribir(destino);

			// Comprobamos que todos pertenecen a la secuencia.
			int i=1;
			foreach (Cabecera_sf1 c in lista){
				if (c.Number != i){
					throw new Exception ("Fuera de secuencia");
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
					//TODO: Poner una excepcion personalizada.
					Console.WriteLine ("Checsum error en filesplit");
				}
				transferidos += leidos;		
				OnProgress (transferidos, cab.FileSize);
			}
			File.SetLastWriteTime (cab.FileName, cab.FileTime.ToLocalTime());
		}
		protected override void _Partir (String fichero,String salida1, String dir, long kb)
		{
			
			Cabecera_sf1 cabecera = new Cabecera_sf1();
			// TODO: reducir fichero a su ultimo elemento (quitar el path)
			cabecera.FileName = new FileInfo(fichero).Name;
			cabecera.FileSize = (int) (new FileInfo(fichero).Length);
			
			// TODO: Rellenar el FileTime con datos reales...
			cabecera.FileTime = File.GetLastWriteTime(fichero).ToUniversalTime();
			// Atributos normales por defecto.			
			cabecera.FileAttr = 32;
			int secuencia = 1;
			long transferidos = 0;
			long tamano = (kb * 1024) - 512;
			String bas = new FileInfo (fichero).Name;
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
				
			}while (transferidos < cabecera.FileSize);
		}
		public override bool PuedeUnir (String fichero)
		{
			if (! fichero.ToUpper().EndsWith(".SF"))
				return false;
			try{
				Cabecera_sf1.LeerCabecera (fichero);
			}
			catch (Exception e){
				return false;
			}
			return true;
		}
	}
}
