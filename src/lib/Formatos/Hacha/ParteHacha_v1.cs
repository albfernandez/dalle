/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Hacha.ParteHacha_v1 - 
		Split and Join files in Hacha (<2.79) format.
	
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

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Checksums;

namespace Dalle.Formatos.Hacha
{
	public class ParteHacha_v1 : Parte
	{
		public ParteHacha_v1 ()
		{
			nombre = "hacha1";
			descripcion = "Hacha v1.0";
			web = "http://www.hacha.org";
			parteFicheros = true;
			compatible = false;			
		}
		
		protected void Partir (string fichero, string s1, string dir,long kb, string version)
		{
			CRC crc = null;
			if (version == "1")
				crc = new NullHachaCRC();
			else	
				crc = new HachaCRC(new FileInfo (fichero).Length);
			
			CabeceraHacha_v1 cab = CabeceraHacha_v1.NewFromVersion (version);

			cab.Tamano = new FileInfo(fichero).Length;
			
			// TODO: Conflicto cuando se especifica el n� de fragmentos?
			// cab.TamanoFragmento = kb*1024 - 512;
			cab.TamanoFragmento = kb * 1024;
			
			cab.NombreOriginal = new FileInfo(fichero).Name;
			
			if ((s1 == null) || (s1 == string.Empty))
				s1 = new FileInfo (fichero).Name;
			
			string salida1 = dir + Path.DirectorySeparatorChar + s1;

			UtilidadesFicheros.ComprobarSobreescribir(salida1 + ".0");
			
			//Escribimos la cabecera (para reservar sitio).
			UtilidadesFicheros.Append (salida1 + ".0", cab.ToByteArray());
			OnProgress (0,1);
			long transferidos = 0;
			int fragmento = 0;
			
			do{
				string s = salida1 + "." + fragmento;
				if (fragmento != 0)
					UtilidadesFicheros.ComprobarSobreescribir (s);
					
				transferidos += UtilidadesFicheros.CopiarIntervalo (
					fichero, s, transferidos, cab.TamanoFragmento, crc);
				fragmento++;
				OnProgress (transferidos, cab.Tamano);
				
			} while (transferidos < cab.Tamano);


			// Volvemos a poner la cabecera, con el CRC bien.
			cab.CRC = unchecked ((int)crc.Value);
			UtilidadesFicheros.Sobreescribir (salida1 + ".0", cab.ToByteArray(), 0);
		}
	
		
		protected override  void _Partir (string fichero, string sal1, string dir, long kb)
		{
			Partir (fichero, sal1, dir, kb, "1");
		}
		
		
		protected void UnirHacha(string fichero, string dirDest, CRC crc)
		{
			CabeceraHacha_v1 cab = CabeceraHacha_v1.LeerCabecera(fichero);

			string salida = dirDest + Path.DirectorySeparatorChar + cab.NombreOriginal;
			UtilidadesFicheros.ComprobarSobreescribir (salida);
			
			string b = fichero.Substring (0, fichero.Length - 1);
			OnProgress (0, 1);
			int fragmento = 0;
			long transferidos = 0;
			string fich = b + fragmento;
			transferidos = UtilidadesFicheros.CopiarIntervalo (fich, salida, cab.Size, crc);
			if ((transferidos != cab.TamanoFragmento) && (transferidos != cab.Tamano)){
				throw new Dalle.Formatos.FileFormatException();
			}
			fragmento++;
			while (transferidos < cab.Tamano){
				
				OnProgress (transferidos, cab.Tamano);
				
				fich = b + fragmento;
				long leidos = UtilidadesFicheros.CopiarTodo (fich, salida, crc);
				transferidos += leidos;
				if ( (leidos != cab.TamanoFragmento) && (transferidos != cab.Tamano) ){
					throw new Dalle.Formatos.FileFormatException();
				}
				fragmento++;
			}
		}
		protected override void _Unir (string fichero, string dirDest)
		{			
			UnirHacha (fichero, dirDest, new NullHachaCRC());
		}
		
		public override bool PuedeUnir (string fichero)
		{
			if (! File.Exists (fichero) )
				return false;
			try{
				CabeceraHacha_v1.LeerCabecera (fichero);
				return true;
			}
			catch (Exception){
				return false;
			}
		}
	}
}
