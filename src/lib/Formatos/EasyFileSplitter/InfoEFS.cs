/*

	Dalle - A split/join file utility library
	Dalle.Formatos.EasyFileSplitter.InfoEFS -
		
	
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

namespace Dalle.Formatos.EasyFileSplitter
{
	
	public class InfoEFS
	{
		private int totalFragmentos;
		private int fragmento;
		private string nombreOriginal;
		
		public InfoEFS ()
		{
		}
		public InfoEFS (string fichero)
		{
			string b = fichero.Substring (0, fichero.LastIndexOf('.'));
			string c = fichero.Substring (fichero.LastIndexOf('.')+1);
			if (b==String.Empty){
				// TODO: Poner una excepcion personalizada.
				throw new Exception ("...");
			}
			string tmp = new FileInfo (fichero).Name;
			tmp = tmp.Substring (0, tmp.LastIndexOf('.'));
			nombreOriginal = tmp;
			string nf = c.Substring (0, c.LastIndexOf("_")).Trim();
			string f = c.Substring (c.LastIndexOf ("_") + 1).Trim();
			Fragmento = Convert.ToInt32 (f);
			totalFragmentos = Convert.ToInt32 (nf);
		}
		
		public string NombreOriginal{
			get{ return nombreOriginal;	}
			set{ nombreOriginal = value; }
		}
		
		public int TotalFragmentos{
			get{ return totalFragmentos; }
			set{ totalFragmentos = value; }
		}
		public int Fragmento{
			get{ return fragmento; }
			set{ fragmento = value;	}
		}
		public String Formato{
			get{ return (NombreOriginal + "." + TotalFragmentos + "_{0}"); }
		}
		public override string ToString ()
		{
			return String.Format(
				"{0}.{1}_{2}", NombreOriginal, TotalFragmentos, Fragmento);
		}
	}
}
