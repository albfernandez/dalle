/*

    Dalle - A split/join file utility library
    Dalle.Formatos.Astrotite.Astrotite
          Join files in astrotite format.
	
    Copyright (C) 2004
    Original author (C - code) - Daniel Martínez Contador <dmcontador@terra.es>
    C# translation by - Alberto Fernández  <infjaf00@yahoo.es>

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

using I = Dalle.I18N.GetText;

namespace Dalle.Formatos.Astrotite
{

	public struct Descript
	{
		public int length;
		public int blocks;
		public int namelength;
		public string name;
	}
	
	public class Astrotite : Parte 
	{
		public const int BUFFER_LENGTH = 32*1024;
		
		public Astrotite (): base ("astrotite", "Astrotite", "www.fantiusen.com/astrotite.html", false, false)
		{
		}

		protected override void _Unir (string fichero, string dirDest)
		{
			ArrayList listaFicheros = new ArrayList();
			int narchivos = 0;
			int i;
			long startdata = 0;
			long leidos = 0;
			long totales = 0;
			
			byte[] initbuffer = new byte[BUFFER_LENGTH];
			
			FileStream astReader = new FileStream (fichero, FileMode.Open);
			
			if (astReader.Read (initbuffer, 0, initbuffer.Length) < 22)
				throw new Exception (I._("Bad file ") + fichero);
			string tmp="";
			for (i = 0; i < "AST2www.astroteam.tk".Length; i++){
				tmp += Convert.ToChar (initbuffer[i]);
			}
			if (tmp != "AST2www.astroteam.tk")
				throw new Exception (I._("Bad file") +  " " + fichero);
			
			narchivos = UtArrays.LeerInt16(initbuffer, 20);
			
			startdata = 22;
			OnProgress (0, 1);
			
			// TODO: Mejorar, poner buffer.
			for (i = 0; i < narchivos; i++){
				astReader.Seek (startdata, SeekOrigin.Begin);
				astReader.Read (initbuffer, 0, 255);
				Descript d;
				d.length = UtArrays.LeerInt32(initbuffer, 0);
				d.blocks = UtArrays.LeerInt32(initbuffer, 4);
				d.namelength = initbuffer[8];
				d.name = UtArrays.LeerTexto (initbuffer, 9, d.namelength);				
				d.name = d.name.Replace ('\\', Path.DirectorySeparatorChar);
				startdata += 9 + d.namelength;				
				listaFicheros.Add (d);	
				totales += d.length;
			
			}
			
			// TODO Excepción personalizada.
			if (i != narchivos)
				throw new Exception ("AstrotiteException");
			
			astReader.Seek (startdata, SeekOrigin.Begin);
			
			foreach (Descript des in listaFicheros){
				astReader.Seek (3, SeekOrigin.Current);
				
				FileStream writer = UtilidadesFicheros.CreateWriter(des.name);				
				
				int quedan = des.length;
				long l = 0;
				while (quedan > 0){
					astReader.Seek(9, SeekOrigin.Current);
					l = astReader.Read (initbuffer, 0, Math.Min(quedan, BUFFER_LENGTH));
					writer.Write (initbuffer, 0, (int)l);
					quedan -= (int) l;
					leidos += l;
					OnProgress (leidos, totales);
				}
				writer.Close();			
			}
			astReader.Close();			
			
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
		}
		
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero))
				return false;
			string t = fichero.ToUpper();
			return (t.EndsWith(".AST2"));
		}		
	}	
}
