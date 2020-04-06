import { api } from "./api-service";
import { SITES, REBELLIONS } from "app/domain/urls";

export const fetchRebellion = rebellionId => api(`${REBELLIONS}${rebellionId}`);

const EVENT_SITE_TYPES = ["Dodo.Sites.EventSite", "Dodo.Sites.MarchSite"];

export const fetchRebellionEvents = async rebellionId => {
	// TODO this would be better converted in to an API call:
	// /sites?rebellion={REBELLION_ID}&type={EVENT}

	const sites = await api(SITES);
	return sites.filter(
		site =>
			site.Parent.guid === rebellionId &&
			EVENT_SITE_TYPES.includes(site.Type)
	);
};
