/*

	Dalle - A split/join file utility library
	Dalle.Formatos.MaxSplitter.MaxSplitter -
		Split and Join files in MaxSplitter format.		
	
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
using Dalle.Formatos.Generico;
using Dalle.Utilidades;
using Dalle.Streams;


namespace Dalle.Formatos.MaxSplitter
{	
	
	public class MaxSplitter : Parte
	{	
		public MaxSplitter() : base (
				"mxs", 
				"MaxSplitter v 1.x Free Edition", 
				"http://www.acc.umu.se/~max/", 
				true, 
				false)
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
					throw new System.IO.FileNotFoundException("", f);
				}
			}
			OnProgress (0,1);
			for (int i=1; i<= info.Fragmentos; i++){
				string f = String.Format (bas + ".{0:000}", i);
				UtilidadesFicheros.CopiarTodo (f, destino);
				OnProgress (i, info.Fragmentos);
			}
			if ( (new FileInfo (destino).Length) != info.TamanoOriginal){
				throw new Dalle.Formatos.FileFormatException();
			}
			if (info.Zipped){
				Dalle.Formatos.Zip.Zip zip = new Dalle.Formatos.Zip.Zip ();
				zip.Progress += new ProgressEventHandler (this.OnProgress);
				zip.Unir (destino);
			}
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			
			JoinInfo info = new JoinInfo ();
			
			info.InitialFragment = 1;
			info.Digits = 3;
			info.OriginalFile = new FileInfo (fichero).Name;
			info.BaseName = info.OriginalFile;
			info.Directory = new DirectoryInfo (dir);
			// TODO Rewrite to use new generic
			int fragmentos = new ParteGenerico().Partir (fichero, sal1, dir, kb, info);
			
			MXSInfo mxsinfo = new MXSInfo ();
			mxsinfo.NombreOriginal = info.OriginalFile;
			mxsinfo.TamanoOriginal = new FileInfo (fichero).Length;
			mxsinfo.Fragmentos = fragmentos;
			
			Stream fos = UtilidadesFicheros.CreateWriter(dir+Path.DirectorySeparatorChar+fichero+".mxs");
			byte[] mxs = mxsinfo.ToByteArray();
			fos.Write(mxs, 0, mxs.Length);
			fos.Close();

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
