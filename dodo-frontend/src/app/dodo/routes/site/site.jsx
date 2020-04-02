import React, { useEffect, useState } from "react"
import PropTypes from "prop-types"

import { fetchSite } from "app/domain/services/site"
import { ContentPage, SiteDetail } from "app/components"

export const Site = ({ match }) => {
	const { siteId } = match.params
	const [site, setSite] = useState()

	useEffect(() => {
		const load = async () => {
			setSite(await fetchSite(siteId))
		}
		load()
	}, [siteId])

	if (!site) {
		return <div>Loading</div>
	}

	return (
		<ContentPage sideBar={<div />}>
			<SiteDetail site={site} />
		</ContentPage>
	)
}

Site.propTypes = {
	match: PropTypes.object.isRequired
}
