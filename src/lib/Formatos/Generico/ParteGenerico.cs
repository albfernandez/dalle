/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Generico.ParteGenerico
	
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

using Dalle.Formatos;
using Dalle.Utilidades;

namespace Dalle.Formatos.Generico
{

	public class ParteGenerico : Parte
	{
		public ParteGenerico()
		{
			nombre = "generico";
			descripcion = "generico";
			web = "_";
			compatible = true;
			parteFicheros= true;
		}
		
		/// <returns>El numero de fragmentos que se han creado.</returns>
		// TODO: Cambiar el orden de los parametros (String, st, long, int, int).
		
		public int Partir (string fichero, long kb, string formato, int ini, int digitos)
		{

			long tamano = new FileInfo (fichero).Length;
			long tFragmento = 1024 * kb;
			long transferidos = 0;
			InfoGenerico inf = new InfoGenerico ();
			inf.Fragmento = ini;
			inf.Formato = formato;
			inf.Digitos = digitos;
			OnProgress (0, tamano);
			do{

				string nombre = inf.ToString();
				UtilidadesFicheros.ComprobarSobreescribir (nombre);
				transferidos += UtilidadesFicheros.CopiarIntervalo 
						(fichero, nombre, transferidos, tFragmento);
				inf.Fragmento++;
				OnProgress(transferidos, tamano);
			} while (transferidos < tamano);
			
			return (inf.Fragmento - ini);
		
		}
		
		protected override void _Partir (string fichero,string sal1, string dir, long kb)
		{
			Partir (fichero, kb, dir + Path.DirectorySeparatorChar + sal1 + ".{0}", 0, 3);	
		}
		
		protected override void _Unir (string fichero, string dirDest)
		{
			string bas = fichero.Substring(0, fichero.LastIndexOf("."));
			//--
			string destino = new FileInfo(fichero).Name;
			destino = destino.Substring (0, destino.LastIndexOf("."));
			destino = dirDest + Path.DirectorySeparatorChar + destino;
			//--
			
			string num = fichero.Substring(fichero.LastIndexOf(".") + 1);
			int ini = Convert.ToInt32 (num);
			Unir (bas + ".{0}", destino, ini, num.Length);
		}		
		public void Unir (string formato, int ini, int digitos)
		{
			Unir (formato, formato.Substring (0, formato.LastIndexOf('.')), 
				ini, digitos);
		}
		public void Unir (string formato, string destino, int ini, int digitos)
		{
			UtilidadesFicheros.ComprobarSobreescribir (destino);
			InfoGenerico info = new InfoGenerico();
			
			info.Formato = formato;
			info.Digitos = digitos;
			info.Fragmento = ini;
			
			long total = 0;
			while (File.Exists (info.ToString())){
				total += new FileInfo (info.ToString()).Length;
				info.Fragmento++;
			}
			
			long transferidos = 0;
			info.Fragmento = ini;
			OnProgress (0, total);
			while (File.Exists (info.ToString())){
				transferidos += UtilidadesFicheros.CopiarTodo (info.ToString(), destino);				
				info.Fragmento++;
				OnProgress (transferidos, total);
			}
		}

		public override bool PuedeUnir (String fichero)
		{
			if (! File.Exists (fichero))
				return false;
				
			if (fichero.LastIndexOf(".") < 0)
				return false;
			
			string bas = fichero.Substring (0, fichero.LastIndexOf("."));
			
			if (fichero.EndsWith(".0")||fichero.EndsWith(".000") || fichero.EndsWith(".00"))
				return true;
			else if (fichero.EndsWith(".1"))
				return (! File.Exists (bas + ".0"));
			else if (fichero.EndsWith(".01"))
				return (! File.Exists (bas+".00"));
			else if (fichero.EndsWith(".001")){
				return (! File.Exists (bas + ".000"));
			}
			return false;
		}		
	}	
}
