import { SITES, REBELLIONS } from "app/domain/urls"
import { api } from "./api-service"
import { memoize } from "./memoize"

export const fetchRebellion = memoize(async rebellionId =>
	api(`${REBELLIONS}${rebellionId}`)
)

const EVENT_SITE_TYPES = ["Dodo.Sites.EventSite", "Dodo.Sites.MarchSite"]

export const fetchRebellionEvents = memoize(async rebellionId => {
	// TODO this would be better converted in to an API call:
	// /sites?rebellion={REBELLION_ID}&type={EVENT}

	const sites = await api(SITES)
	return sites.filter(
		site =>
			site.Parent.guid === rebellionId &&
			EVENT_SITE_TYPES.includes(site.Type)
	)
})
