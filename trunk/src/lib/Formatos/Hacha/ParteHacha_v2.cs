/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Hacha.ParteHacha_v2 - 
		Split and Join files in Hacha (>=2.79) format.
	
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

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Checksums;

namespace Dalle.Formatos.Hacha
{
	public class ParteHacha_v2 : ParteHacha_v1
	{

		public ParteHacha_v2 ()
		{
			nombre = "hacha2";
			descripcion = "Hacha v2.79 +";
			web = "http://www.hacha.org";
			parteFicheros = true;
			compatible = false;
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			Partir (fichero, sal1, dir, kb, "2");
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			CRC crc = new HachaCRC (new FileInfo (fichero).Length);
			base.UnirHacha (fichero, dirDest,crc);
			
			// TODO Recuperar comprobacion del 'crc'
			// Comprobamos el CRC.
			//CabeceraHacha_v1 cab = CabeceraHacha_v1.LeerCabecera (fichero);
			/*
			if ( (cab.CRC != 7) && (crc.Value != cab.CRC) ){
				Console.WriteLine("calculado:" + crc.Value);
				Console.WriteLine("leido:" + cab.CRC);
				throw new Dalle.Formatos.FileFormatException();			
			}
			*/
		}
		
		public override bool PuedeUnir (string fichero)
		{
			if (! File.Exists (fichero) )
				return false;
				
			try{
				CabeceraHacha_v1 cab = CabeceraHacha_v1.LeerCabecera (fichero);
				return (cab.Version == "2");
			}
			catch (Exception){
				return false;
			}			
		}
	}
}
