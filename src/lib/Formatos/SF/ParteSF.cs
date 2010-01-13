/*

	Dalle - A split/join file utility library
	Dalle.Formatos.SF.ParteSF - Split and Join files in sf format.
	
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

using Dalle.Utilidades;

namespace Dalle.Formatos.SF
{
		
	public class ParteSF : Parte
	{
		public ParteSF()
		{
			nombre = "sf";
			descripcion = "sf v0.06";
			web = "http://www.terra.es/personal2/el.maquinas";
			compatible = false;
			parteFicheros = false;
		}
		protected override void _Unir (string fichero, string dirDest)
		{

			ArrayList lista = new ArrayList();
			string destino = "";			
			string f = new FileInfo(fichero).DirectoryName;
			f += Path.DirectorySeparatorChar + "SF{0:0000}.SF";
			for (int i = 1 ; File.Exists (string.Format (f,i)); i++)
			{
				CabeceraSF c = new CabeceraSF(string.Format (f,i));
				if (c.Numero != i){
					//TODO: Lanzar excepcion personalizada.
					throw new Exception ();
				}
				destino = dirDest + Path.DirectorySeparatorChar + c.Nombre;
				lista.Add (c);
			}
			
			UtilidadesFicheros.ComprobarSobreescribir(destino);
			int contador = 0;
			OnProgress (0, 1);
			foreach (CabeceraSF c in lista){
				String nombre = string.Format (f, c.Numero);
				UtilidadesFicheros.CopiarIntervalo(nombre, destino, c.Tamano);
				OnProgress (++contador, lista.Count);
			}
		}
		// TODO: Comprobar que no se excederá el limite de ficheros..
		protected override void _Partir (string fichero,string sal1, string dir, long kb)
		{

			string bas = dir + Path.DirectorySeparatorChar + "SF{0:0000}.SF";
			long tamano = kb*1024;
			CabeceraSF cab = new CabeceraSF();
			cab.Nombre = fichero;
			tamano -= cab.Tamano;
			int contador = 1;
			long total_a_leer = new FileInfo(fichero).Length;
			if ((total_a_leer / tamano) > 255){
				throw new Dalle.Formatos.FileFormatException();
			}
			long transferidos  = 0;
			OnProgress (0, 1);
			do{
				byte[] b = UtilidadesFicheros.LeerSeek(fichero, transferidos, tamano);
				cab.Numero = contador;
				string nombreFichero = string.Format (bas, contador);
				transferidos += b.Length;
				UtilidadesFicheros.Append (nombreFichero, cab.ToByteArray());
				UtilidadesFicheros.Append (nombreFichero, b);
				contador++;
				OnProgress (transferidos, total_a_leer);
			}while (transferidos < total_a_leer);
		}
		
		public override bool PuedeUnir (string fichero)
		{
			if (! File.Exists (fichero) )
				return false;
				
			try{
				new CabeceraSF(fichero);
			}
			catch (Exception)
			{
				return false;
			}
			
			return (fichero.ToUpper()=="SF0001.SF");
		}
	}
}
