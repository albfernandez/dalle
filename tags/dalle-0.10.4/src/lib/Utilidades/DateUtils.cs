
using System;

namespace Dalle.Utilidades
{


	public class DateUtils
	{

		static readonly DateTime StartOfEpoch = new DateTime (1970, 1, 1);
		
		// TODO Comprobar esta funcion
		public static long CurrentUnixTimeMillis 
		{
			get{
				long millis = (DateTime.UtcNow - StartOfEpoch).Ticks / 10000;
				return millis;
			}
		}
		
		public DateUtils ()
		{
		}
	}
}
