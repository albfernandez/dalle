/*

    Dalle - A split/join file utility library
    Dalle.Formatos.Astrotite.Astrotite
          Join files in astrotite format.
	
    Copyright (C) 2004-2010
    Original author (C - code) - Daniel Martínez Contador <dmcontador@terra.es>
    C# translation by - Alberto Fernández  <infjaf@gmail.com>

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
using System.Text;

using Dalle.Formatos;
using Dalle.Utilidades;

namespace Dalle.Formatos.Astrotite
{

	public struct Descript
	{
		public int length;
		public int blocks;
		public int namelength;
		public string name;
		
	}
	public struct Block 
	{
		public String block;
		public int size;
		public int crc;
	}
	
	public class Astrotite : Parte 
	{
		
		public Astrotite (): base ("astrotite", "Astrotite", "www.fantiusen.com/astrotite.html", false, false)
		{
		}

		private Descript LeerDescripcionFichero (Stream astReader)
		{
			byte[] initbuffer = new byte[255];
			Descript d;
			astReader.Read (initbuffer, 0, 9);
			d.length = UtArrays.LeerInt32 (initbuffer, 0);
			// Astrotite genera siempre un bloque por archivo. Si el archivo está vacio, tambien
			// Pero graba un 0 en el numero de bloques.
			d.blocks = UtArrays.LeerInt32 (initbuffer, 4);
			if (d.blocks == 0)
			{
				d.blocks = 1;
			}
			d.namelength = initbuffer[8];
			astReader.Read (initbuffer, 0, d.namelength);
			d.name = UtArrays.LeerTexto (initbuffer, 0, d.namelength);
			d.name = d.name.Replace ('\\', Path.DirectorySeparatorChar);
			return d;
		}
		
		private ArrayList LeerListaFicheros (Stream astReader)
		{
			ArrayList listaFicheros = new ArrayList ();
			byte[] initbuffer = new byte[2];
			astReader.Read (initbuffer, 0, 2);
			int narchivos = UtArrays.LeerInt16 (initbuffer, 0);
			for (int i = 0; i < narchivos; i++) 
			{
				listaFicheros.Add (LeerDescripcionFichero (astReader));		
			}
			return listaFicheros;
		}
		protected override void _Unir (string fichero, string dirDest)
		{			

			long leidos = 0;
			long totales = 0;
			
			byte[] initbuffer = new byte[Dalle.Consts.BUFFER_LENGTH];
			
			FileStream astReader = new FileStream (fichero, FileMode.Open);
			
			if (astReader.Read (initbuffer, 0, 20) < 20) {
				throw new Dalle.Formatos.FileFormatException ();
			}
			StringBuilder tmp = new StringBuilder ();
			for (int i = 0; i < "AST2www.astroteam.tk".Length; i++)
			{
				tmp.Append (Convert.ToChar (initbuffer[i]));
			}
			Console.WriteLine (tmp.ToString ());
			if (!"AST2www.astroteam.tk".Equals (tmp.ToString ()))
			{
				throw new Dalle.Formatos.FileFormatException ();
			}

			OnProgress (0, 1);
			
			ArrayList listaFicheros = LeerListaFicheros (astReader);
			foreach (Descript des in listaFicheros)
			{
				totales += des.length;
			}
			
			AstrotiteCRC crc = new AstrotiteCRC ();
			foreach (Descript des in listaFicheros) 
			{
				Console.WriteLine ("Procsando " + des.name);
				byte[] initialMark = new byte[3];
				astReader.Read (initialMark, 0, initialMark.Length);
				string iMark = UtArrays.LeerTexto (initialMark, 0);
				
				if (!"SDT".Equals (iMark) && !"FDA".Equals (iMark))
				{
					throw new FileFormatException ();
				}

				Stream writer = UtilidadesFicheros.CreateWriter (dirDest + Path.DirectorySeparatorChar + des.name);

				int l = 0;
				long bloquesLeidos = 0;
				while (bloquesLeidos < des.blocks)
				{					
					Block block = readBlock (astReader);
					crc.Reset ();					
					int quedan1 = block.size;
					while (quedan1 > 0) 
					{
						l = astReader.Read(initbuffer, 0, Dalle.Consts.BUFFER_LENGTH < quedan1 ? Dalle.Consts.BUFFER_LENGTH: quedan1 );
						writer.Write (initbuffer, 0, l);						
						
						if ((long)block.crc != 0xFFFFFFFF && block.crc != 0){
							crc.Update(initbuffer, 0,  l);
						}
						quedan1 -= l;				
						leidos += l;
						OnProgress (leidos, totales);
					}					
					
					if ((long)block.crc != 0xFFFFFFFF && block.crc != 0 && (long) block.crc != crc.Value){
						throw new Dalle.Formatos.ChecksumVerificationException();
					}	
					bloquesLeidos++;
				}
				writer.Close();							
			}
			astReader.Close();			
		}
		private Block readBlock(FileStream astReader){	
			byte[] info = new byte[9];		
			astReader.Read(info, 0, 9);
			Block block = new Block();
			block.block = UtArrays.LeerTexto (info, 0, 3);
			block.size  = UtArrays.LeerUInt16 (info, 3);
			block.crc   = UtArrays.LeerInt32 (info, 5);
			return block;
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
		}
		
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)) {
				return false;
			}
			if (!fichero.ToUpper ().EndsWith (".AST2")) {
				return false;
			}
			
			byte[] initbuffer = new byte[22];
			FileStream astReader = new FileStream (fichero, FileMode.Open);
			
			if (astReader.Read (initbuffer, 0, initbuffer.Length) < 22) {
				astReader.Close ();
				return false;
			}
			astReader.Close ();
			string tmp="";
			for (int i = 0; i < "AST2www.astroteam.tk".Length; i++){
				tmp += Convert.ToChar (initbuffer[i]);
			}
			if (tmp != "AST2www.astroteam.tk"){
				return false;
			}
			return true;
			
		}		
	}	
}
