/*

	Dalle - A split/join file utility library
	Dalle.Formatos.MaxSplitter.MXSInfo - 
		Support of aux MXS file in MaxSplitter format.		
	
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
using System.Reflection;
	
using Dalle.Utilidades;
using I = Dalle.I18N.GetText;

namespace Dalle.Formatos.MaxSplitter
{	

	public class MXSInfo
	{
		private string nombreOriginal;
		private long tamanoOriginal;
		private int fragmentos;
		private bool zipped = false;
		private string version = "Created by Dalle v." + 
					Assembly.GetExecutingAssembly().GetName().Version;
		private string atributos = "a";
		private int batch = 1;
	
		public MXSInfo()
		{			
		}
		public MXSInfo (string fichero)
		{
			// TODO: Poner excepciones personalizada.
			if ((new FileInfo(fichero).Length) > 512){
				throw new Exception (I._("Too large to be a MXS file"));
			}
			
			byte[] datos = UtilidadesFicheros.LeerTodo(fichero);
			
			if ((datos[0] != 0x4d) || (datos[1] != 0x58) || (datos[2] != 0x53))
				throw new Exception ();
			
			int i = 3;
			
			
			// TODO: Cambiar estas funciones por UtArrays ?
			nombreOriginal=string.Empty;
			
			while (datos[i] != 0){
				nombreOriginal += Convert.ToChar(datos[i]);
				i++;
			}
			i++;
			String tmp = string.Empty;
			while (datos[i] != 0){
				tmp += Convert.ToChar(datos[i]);
				i++;
			}
			TamanoOriginal = Convert.ToInt32 (tmp);
			
			i++;
			tmp = string.Empty;
			while (datos[i] != 0){
				tmp += Convert.ToChar (datos[i]);
				i++;
			}
			Fragmentos = Convert.ToInt32 (tmp);
						
			i++;
			
			version = string.Empty;
			while (datos[i] != 0){
				version += Convert.ToChar(datos[i]);
				i++;
			}
			
			i++;
			
			atributos = string.Empty;
			while (datos[i] != 0){
				atributos+= Convert.ToChar (datos[i]);
				i++;
			}
			
			i++;
			
			//Aqui nos deberia indicar si es zipped o no
			tmp = string.Empty;
			while (datos[i] != 0){
				tmp += Convert.ToChar (datos[i]);
				i++;
			}
			zipped = ! (tmp == "N");
			
			i++;
			tmp = string.Empty;
			while (datos[i] != 0){
				tmp += Convert.ToChar(datos[i]);
				i++;
			}
			batch = Convert.ToInt32 (tmp);
			// TODO: Mejorar esta excepción.
			if (i != (datos.Length -1))
				throw new Exception ();	
		}
		
		public string NombreOriginal{
			get{ return nombreOriginal;	}
			set{ nombreOriginal = value;}
		}
		public long TamanoOriginal{
			get{ return tamanoOriginal; }
			set{ tamanoOriginal = value; }
		}
		public int Fragmentos{
			get{ return fragmentos; }
			set{ fragmentos = value;}
		}
		public bool Zipped {
			get{return zipped;}
			set{zipped = value;}
		}
		public int Size{
			get{
				return (7 + 3 + 1 + NombreOriginal.Length + 
					TamanoOriginal.ToString().Length +
					Fragmentos.ToString().Length + version.Length
					+ atributos.Length + batch.ToString().Length);
				// 7 nulos + 3 letras de MXS + 1 letra del campo zip +
				// las longitudes de las cadenas que se guardan.
			}
		}
		
		public byte[] ToByteArray()
		{
			
			byte[] ret = new byte[this.Size];
			String[] campos = new String[]{
				"MXS"+ this.NombreOriginal,
				this.TamanoOriginal.ToString(),
				this.Fragmentos.ToString(),
				this.version,
				this.atributos,
				(this.zipped) ? "Y" : "N",
				this.batch.ToString()
			};
			
			int i=0;
			foreach (String cadena in campos){
				foreach (char c in cadena){
					ret[i++] = Convert.ToByte(c);
				}
				i++;
			}
			return ret;
		}
	}
}
