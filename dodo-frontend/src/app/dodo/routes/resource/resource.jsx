import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { Container } from "app/components/resources"
import { SiteMap, Loader, DateLayout, PageTitle } from "app/components"
import { ResourceContent } from './resource-content'

export const Resource =
	(
		{
			match,
			getResource,
			getNotifications,
			resource,
			notifications,
			isLoading,
			centerMap,
			setCenterMap,
			resourceTypes = [],
			joinResource
		}
	) => {
		const { resourceId, resourceType } = match.params

		useEffect(() => {
			getResource(resourceType, resourceId, setCenterMap)
			getNotifications(resourceType, resourceId);
		}, [match])


		const resourceColor =
			resource.metadata &&
			resourceTypes.length &&
			'#' + resourceTypes.find(resType => resource.metadata.type === resType.value).displayColor

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
							<Loader display={isLoading}/>
							{resource.metadata && !isLoading && (
								<ResourceContent
									resource={resource}
									notifications={notifications}
									setCenterMap={setCenterMap}
									resourceTypes={resourceTypes}
									resourceColor={resourceColor}
									resourceType={resourceType}
									joinResource={joinResource}
								/>
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
