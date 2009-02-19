/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Kamaleon.MetaInfoKamaleon
	
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
using System.Collections;
using System.IO;

using Dalle.Utilidades;

namespace Dalle.Formatos.Kamaleon
{	
	
	public class MetaInfoKamaleon
	{
		public ArrayList infos = new ArrayList();
		public int Size{
			get{ return ((560*infos.Count) + 19); }
		}
		public MetaInfoKamaleon ()
		{
		}
		public static void CopiarArray (byte[] from, byte[] to, int pos)
		{
			for (int i=0; i < from.Length; i++)
				to[i+pos] = from[i];		
		}
		public byte[] ToByteArray()
		{
			byte[] ret = new byte[Size];
			int contador = 0;
			int tamano_ultimoFragmento = 0;
			foreach (InfoFicheroKamaleon k in infos){
				CopiarArray (k.ToByteArray(), ret, contador);
				contador+=560;
				tamano_ultimoFragmento = k.TamanoFragmento;
			}
			String final = "";
			
			final += UtilidadesCadenas.Format (tamano_ultimoFragmento, 10);
			if (infos[0] is InfoFicheroKamaleon_v2)
				final += "<-LIST2->";
			else if (infos[0] is InfoFicheroKamaleon)
				final += "<-LISTA->";
			
			foreach (char c in final){
				ret[contador++] = Convert.ToByte(c);
			}
			return ret;
		}
		public void Add (InfoFicheroKamaleon k)
		{
			infos.Add (k);
		}
		public InfoFicheroKamaleon PrimerInfo{
			get{
				if (infos.Count == 0)
					return null;
				return ( (InfoFicheroKamaleon) infos[0]);
			}
		}
		public InfoFicheroKamaleon UltimoInfo{
			get{
				if (infos.Count == 0)
					return null;
				return ( (InfoFicheroKamaleon) infos[infos.Count - 1]);
			}
		}
		public string GetNombreUltimoFragmento ()
		{
			if (infos.Count == 0)
				return "";
			InfoFicheroKamaleon k = (InfoFicheroKamaleon) infos[infos.Count-1];
			return k.NombreFragmento;
		}
		public MetaInfoKamaleon(string fichero)
		{		
			FileStream reader = new FileStream (fichero, FileMode.Open);
			reader.Seek(-0x13, SeekOrigin.End);
			byte[] buffer = new byte[0x13];
			reader.Read (buffer, 0, buffer.Length);
			String s = "";
			for (int i=0; i < buffer.Length; i++)
			{
				s += Convert.ToChar(buffer[i]);
			}
			String lista = s.Substring(10);
			String tam = s.Substring(0,10);
			
			if ( (lista != "<-LISTA->") && (lista != "<-LIST2->"))
			{
				throw new NoMetaInfoException ();
			}
			
			long l = Convert.ToInt64 (tam);
			
			reader.Seek (l, SeekOrigin.Begin);
			while ((reader.Length - reader.Position) > 560)
			{
				byte[] b = new byte [560];
				reader.Read (b, 0, b.Length);
				if (lista=="<-LISTA->")
				{
					infos.Add (new InfoFicheroKamaleon (b));	
				}
				else if (lista == "<-LIST2->")
				{
					infos.Add (new InfoFicheroKamaleon_v2 (b));
				}
			}
			reader.Close();		
		}	
	}
}
