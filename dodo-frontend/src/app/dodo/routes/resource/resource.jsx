import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { Container } from "app/components/events"
import { SiteMap, Loader, DateLayout, PageTitle } from "app/components"
import { ResourceContent } from './resource-content'

export const Resource = ({ match, getResource, resource, isLoading, centerMap, setCenterMap, resourceTypes }) => {
	const { eventId, eventType } = match.params

	useEffect(() => {
		getResource(eventType, eventId)
	}, [match])

	const { location } = resource
	const defaultLocation = resource.location
		? [location.latitude, location.longitude]
		: []

	return (
		<Fragment>
			<SiteMap
				centerMap={centerMap}
				setCenterMap={setCenterMap}
				center={defaultLocation}
				sites={resource.sites && [...resource.sites, ...resource.workingGroups]}
			/>
			<Container
				content={
					<Fragment>
						<Loader display={isLoading} />
						{resource.metadata && !isLoading && (
							<ResourceContent resource={resource} setCenterMap={setCenterMap} resourceTypes={resourceTypes}/>
						)}
					</Fragment>
				}
			/>
		</Fragment>
	)
}

Resource.propTypes = {
	match: PropTypes.object.isRequired,
	getResource: PropTypes.func,
	event: PropTypes.object,
	resourceTypes: PropTypes.array,
}
