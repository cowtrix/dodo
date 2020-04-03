import React from "react"
import PropTypes from "prop-types"

import { fetchSite } from "app/domain/services/site"
import { useFetch } from "app/domain/services/useFetch"
import { ContentPage, SiteDetail, SiteInfo } from "app/components"

export const Site = ({ match }) => {
	const { siteId } = match.params
	const site = useFetch(fetchSite, siteId)

	if (!site) {
		return <div>Loading</div>
	}

	return (
		<ContentPage sideBar={<SiteInfo site={site} />}>
			<SiteDetail site={site} />
		</ContentPage>
	)
}

Site.propTypes = {
	match: PropTypes.object.isRequired
}
