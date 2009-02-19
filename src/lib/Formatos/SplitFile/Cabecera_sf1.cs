/*

	Dalle - A split/join file utility library
	Dalle.Formatos.SplitFile.Cabecera_sf1 - Header of SplitFile files.
	
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
using System.Reflection;

namespace Dalle.Formatos.SplitFile
{
	
	public class Cabecera_sf1
	{
		//TODO: Que ponemos en comentario.
		public const String COMENTARIO = "";
		private String comentario = null;
		private String fileName = null;
		private long fileSize = 0;
		
		/// <summary>La fecha de modificaci�n del archivo original (UTC).</summary>
		
		private DateTime fileTime = DateTime.Now.ToUniversalTime();
		private int fileAttr = 32;
		private int number = 0;
		private long dataSize = 0;
		private bool isLast = false;
		private int checkSum = 0;
		public const int CAB_SIZE = 512;
		
		public Cabecera_sf1 () 
		{
			this.comentario = 
			";Created by Dalle v." + 
				Assembly.GetExecutingAssembly().GetName().Version;

		}
		
		
		private Cabecera_sf1 (char[] buffer) : this()
		{
			
			int cont = 0;
			while (buffer[cont] != '\0')
			{
				cont++;
			}
			String cabecera = new String (buffer, 0, cont);
			
			String[] partes = cabecera.Split("\n".ToCharArray());
			for (int i=0; i < partes.Length; i++){
				partes[i] = partes[i].Trim();
			}
			if (partes[1] != "[Split]"){
				throw new Exception (" ____ ");
			}
			if (! partes[2].StartsWith ("FileName=")){
				throw new Exception (" --- ");
			}
			if (! partes[3].StartsWith ("FileSize=")){
				throw new Exception (" --- ");
			}
			if (! partes[4].StartsWith ("FileTime=")){
				throw new Exception (" --- ");
			}
			if (! partes[5].StartsWith ("FileAttr=")){
				throw new Exception (" --- ");
			}
			if (! partes[6].StartsWith ("Number=")){
				throw new Exception (" --- ");
			}
			if (! partes[7].StartsWith ("DataSize=")){
				throw new Exception (" --- ");
			}
			if (! partes[8].StartsWith ("IsLast=")){
				throw new Exception (" --- ");
			}
			if (! partes[9].StartsWith ("CheckSum=")){
				throw new Exception (" --- ");
			}
			this.Comentario = partes[1].Trim();
			this.FileName = partes[2].Split("=".ToCharArray())[1].Trim();
			this.FileSize = Convert.ToInt64 (partes[3].Split("=".ToCharArray())[1].Trim());
			this.FileTime = this.ProcesarFecha (partes[4].Split("=".ToCharArray())[1].Trim());
			this.FileAttr = Convert.ToInt32(partes[5].Split("=".ToCharArray())[1].Trim());
			this.Number = Convert.ToInt32(partes[6].Split("=".ToCharArray())[1].Trim());
			this.DataSize = Convert.ToInt64(partes[7].Split("=".ToCharArray())[1].Trim());
			this.IsLast = Convert.ToInt32 (partes[8].Split ("=".ToCharArray())[1].Trim()) == 1;
			this.CheckSum = Convert.ToInt32(partes[9].Split ("=".ToCharArray())[1].Trim());
		}
		
		public String Comentario{
			get{ return comentario; }
			set{ comentario = value; }
		}
		
		public String FileName{
			get{ return fileName; }
			set{ fileName = value; }
		}
		public long FileSize{
			get{ return fileSize; }
			set{ fileSize = value; }
		}
		

		/// <summary>La fecha de modificaci�n del fichero original (UTC).</summary>
		public DateTime FileTime{
			get{ return fileTime; }
			set{ fileTime = value; }
		}
		public int FileAttr{
			get{ return fileAttr; }
			set{ fileAttr = value; }
		}
		
		public int Number{
			get{ return number;	}
			set{ number=value; }
		}
		
		public long DataSize {
			get{ return dataSize; }
			set{ dataSize = value; }
		}		
		public bool IsLast{
			get{ return isLast; }
			set{ isLast = value; }
		}
		public int CheckSum{
			get{ return checkSum; }
			set{ checkSum = value; }
		}
		
		
		public override String ToString()
		{
			String ret = "";
			ret += Comentario + "\r\n";
			ret += "[Split]\r\n";
			ret += "FileName=" + FileName + "\r\n";
			ret += "FileSize=" + FileSize + "\r\n";
			// TODO:Hacer que funcione bien el filetime
			ret += "FileTime=" + FileTime.ToString ("d MMM yyyy HH:mm:ss G\\MT") + "\r\n";
			ret += "FileAttr=" + FileAttr +"\r\n";
			ret += "Number=" + Number + "\r\n";
			ret += "DataSize=" + DataSize + "\r\n";
			ret += "IsLast=" + ( IsLast ? 1 : 0 ) + "\r\n";
			ret += "CheckSum=" + CheckSum + "\r\n";
			return ret;
		}
		
		public byte[] ToByteArray()
		{
			// TODO: Comprobar que la cabecera es menor que 512b
			String cab = this.ToString();
			byte[] b = new byte[CAB_SIZE];
			for (int i=0; (i < cab.Length) && (i < CAB_SIZE); i++)
				b[i] = Convert.ToByte(cab[i]);
			return b;
		}
		
		private DateTime ProcesarFecha (String fecha)
		{
			try{
				String[] datos = fecha.Split(" :".ToCharArray());
				String[] meses = {"Jan", "Feb", "Mar", "Apr", "May", "Jun", 
						"Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
				
				int dia = Convert.ToInt32 (datos[0]);
				int mes = 1;
				for (int i=0; i < meses.Length; i++){
					if (meses[i] == datos[1]){
						mes = i+1;
					}						
				}

				int anio = Convert.ToInt32 (datos[2]);
				int hora = Convert.ToInt32 (datos[3]);
				int minuto = Convert.ToInt32(datos[4]);
				int segundo = Convert.ToInt32 (datos[5]);
				DateTime ret = new DateTime(anio, mes, dia, hora, minuto, segundo);
				if (datos.Length == 6){
					return ret =  ret.ToUniversalTime();
				}
				return ret;
			}
			catch (Exception){
				return DateTime.Now;
			}
			
		}
		public static Cabecera_sf1 LeerCabecera (String fichero)
		{
			byte[] buffer = new byte[CAB_SIZE];
			char[] buffer2 = new char[CAB_SIZE];			
			
			FileStream f = new FileStream (fichero, FileMode.Open);
			int leidos = f.Read (buffer, 0, CAB_SIZE);
			f.Close();
			
			if (leidos != CAB_SIZE)
			{
				throw new Exception ("Tama�o cabecera " + leidos);
			}
			
			for (int i=0; i < CAB_SIZE; i++)
			{
				buffer2[i] = Convert.ToChar(buffer[i]);
			}			
			Cabecera_sf1 ret = new Cabecera_sf1(buffer2);
			return ret;		
		}
	}
	
}
