using System.ComponentModel.DataAnnotations;

namespace DodoServer.ViewModels
{
	public class Location
	{
		[Range(-90, 90)]
		public double Latitude { get; set; }
		[Range(-180, 180)]
		public double Longitude { get; set; }
	}
}
