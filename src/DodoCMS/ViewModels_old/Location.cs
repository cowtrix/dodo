using System.ComponentModel.DataAnnotations;

namespace Dodo.ViewModels
{
	public class Location
	{
		[Range(-90, 90)]
		public double Latitude { get; set; }
		[Range(-180, 180)]
		public double Longitude { get; set; }
	}
}
