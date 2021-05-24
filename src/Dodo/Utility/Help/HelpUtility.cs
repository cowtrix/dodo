using Resources.Location;
using System.Collections.Generic;

namespace Dodo
{
	public static class HelpUtility
	{
		static Dictionary<string, string> m_helpTooltips = new Dictionary<string, string>()
		{
			{
				nameof(GeoLocation),
				"Use the <i class=\"fas fa-search\"></i> Search bar to look for a location.<br/>You can drag the blue marker around for small adjustments."
			},
		#region RESOURCES
			{
				"IResourceReference_Parent",
				"The parent is the group that owns and controls this object. You cannot alter the parent of an object."
			},
			{
				nameof(Resources.IResourceReference.Parent),
				"The parent is the group that owns and controls this object. You cannot alter the parent of an object."
			},
			{
				nameof(Resources.IResourceReference.Name),
				"The public name of this object, visible to everyone. Make this descriptive but short."
			},
			{
				nameof(Resources.IResourceReference.PublicDescription),
				"This is where you should put all the necessary information about this. The maximum length is 2000 characters. " + 
				"To insert hyperlinks, use the following format: [My link text](www.example.com). This will display as: <a href=\"www.example.com\">My link text</a>"
			},
		#endregion
			{
				nameof(LocationResources.LocationResourceBase.ArrestRisk),
				"<b>High</b>: law enforcement is actively making arrests or engaging in crowd suppression at this site. Rebels who are not prepared to be in high-risk situations should not come to this location.<br />" +
				"<b>Moderate</b>: the organiser anticipates or has already experienced a small number of arrests, and that rebels may find themselves in arrestable situations. However, protesters will generally not be arrested without warning or without cause.<br />" +
				"<b>Low</b>: there is or will be law enforcement present. Rebels should stay informed about the ongoing arrest risk, but engagement with law enforcement is unlikely.<br />" +
				"<b>None</b>: there is no risk of arrest at this location right now or for the foreseeable future."
			},
			{
				nameof(Dodo.IVideoResource.VideoEmbedURL),
				"This will appear as a banner video on this page if set. Usually a YouTube embed link, which looks like this: https://www.youtube.com/embed/d4QDM_Isi24"
			},
			{
				nameof(Dodo.ITimeBoundResource.StartDate),
				"When this starts, in Universal Standard Time (UTC+0)."
			},
			{
				nameof(Dodo.ITimeBoundResource.EndDate),
				"When this ends, in Universal Standard Time (UTC+0). Note that the end date always has to be after the start date."
			},
		#region SITE FACILITIES
			{
				nameof(Dodo.LocationResources.SiteFacilities),
				"Here, you can indicate what facilities are available at this location."
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.Toilets)}",
				"Are there toilets available to protestors?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.Bathrooms)}",
				"Are there places available to shower and clean yourself?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.Accomodation)}",
				"Can protestors find a accomodation in a permanent structure here?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.AffinityGroupFormation)}",
				"Can protestors participate in Affinity Group formation workshops here?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.DisabilityFriendly)}",
				"Is this location wheelchair accessible? Will blind, deaf or differently abled rebels be able to participate?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.Electricity)}",
				"Do rebels have access to sources of electricity, e.g. to charge phones or power other appliances?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.FamilyFriendly)}",
				"Is this location suitable for families, including elders and young children?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.Food)}",
				"Is there food available at this location, catering to a reasonable range of dietary requirements?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.IndoorCamping)}",
				"Is there camping space available inside a permanent or temporary structure?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.OutdoorCamping)}",
				"Is there camping space available outside?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.Inductions)}",
				"Are Induction workshops being run here for new members?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.Internet)}",
				"Is there internet access at this location, such as a wired connection or WiFi?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.Kitchen)}",
				"Is there a public kitchen where Rebels can prepare their own food?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.TalksAndTraining)}",
				"Are training workshops being run here, <i>apart</i> from Inductions and Affinity Group formations?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.VolunteersNeeded)}",
				"Are volunteers generally needed to work here?"
			},
			{
				$"{nameof(Dodo.LocationResources.SiteFacilities)}_{nameof(Dodo.LocationResources.SiteFacilities.Welfare)}",
				"Is there a welfare group here that can take care of Rebels in distress?"
			},
		#endregion
		};

		public static string GetHelpHTML(string key)
		{
			if(!m_helpTooltips.TryGetValue(key, out var val))
			{
				return "No Help Defined";
			}
			return val;
		}

		public static bool HasHelp(string key)
		{
			return m_helpTooltips.ContainsKey(key);
		}
	}
}
