/*

	Dalle - A split/join file utility library
	Dalle.Formatos.MaxSplitter.MaxSplitter -
		Split and Join files in MaxSplitter format.		
	
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
using Dalle.Formatos.Generico;
using Dalle.Utilidades;
using I = Dalle.I18N.GetText;


namespace Dalle.Formatos.MaxSplitter
{	
	
	public class MaxSplitter : Parte
	{	
		public MaxSplitter() : base (
				"mxs", 
				"MaxSplitter v 1.x Free Edition", 
				"http://www.acc.umu.se/~max/", 
				true, 
				true)
		{
	
		}
		
		protected override void _Unir (string fichero, string dirDest)
		{
			
			
			string bas = fichero.Substring (0, fichero.LastIndexOf("."));
			
			MXSInfo info = new MXSInfo (fichero);
			string destino = dirDest + Path.DirectorySeparatorChar + info.NombreOriginal;
			UtilidadesFicheros.ComprobarSobreescribir (destino);

			for (int i=1; i<= info.Fragmentos; i++){
				string f = String.Format (bas + ".{0:000}" , i);
				if (!File.Exists(f)){
					//TODO Mensaje bien puesto.
					string msg = string.Format (I._("MXS: File not found {0}"), f);
					throw new Exception (msg);
				}
			}
			OnProgress (0,1);
			for (int i=1; i<= info.Fragmentos; i++){
				string f = String.Format (bas + ".{0:000}", i);
				UtilidadesFicheros.CopiarTodo (f, destino);
				OnProgress (i, info.Fragmentos);
			}
			if ( (new FileInfo (destino).Length) != info.TamanoOriginal){
				// TODO: Poner una excepción bien.
				//Console.WriteLine ("Size of generated file is incorrect");
				string msg = I._("MXS: Incorrect size of regenerated file");
				throw new Exception (msg);
			}
			if (info.Zipped){
				Dalle.Formatos.Zip.Zip zip = new Dalle.Formatos.Zip.Zip ();
				zip.Progress += new ProgressEventHandler (this.OnProgress);
				zip.Unir (destino);
			}
		}
		protected override void _Partir (string fichero,string sal1, string dir, long kb){

			int fragmentos = new ParteGenerico().Partir(fichero, kb, 
				dir + Path.DirectorySeparatorChar + fichero + ".{0}", 1, 3);
			long tamano = new FileInfo(fichero).Length;
			
			MXSInfo info = new MXSInfo ();
			info.NombreOriginal = new FileInfo(fichero).Name;
			info.TamanoOriginal = tamano;
			info.Fragmentos = fragmentos;

			UtilidadesFicheros.ComprobarSobreescribir (
				dir + Path.DirectorySeparatorChar + fichero + ".MXS");
			UtilidadesFicheros.Append (
				dir + Path.DirectorySeparatorChar + fichero + ".MXS", 
				info.ToByteArray());
		}

		public override bool PuedeUnir (string fichero)
		{
			if (! File.Exists (fichero) )
				return false;
			try {
				new MXSInfo (fichero);
				return fichero.ToUpper().EndsWith (".MXS");
			}catch (Exception){
			}
			return false;
		}	
	}
}
