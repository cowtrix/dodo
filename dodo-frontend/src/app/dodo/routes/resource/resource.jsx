import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { Container } from "app/components/resources"
import { SiteMap, Loader } from "app/components"
import { ResourceContent } from './resource-content'
import { getResourceTypeData, shouldHideMap } from './services'

export const Resource =
	(
		{
			match,
			getResource,
			resource,
			notifications,
			isLoading,
			centerMap,
			setCenterMap,
			resourceTypes = [],
			subscribeResource,
			leaveResource,
			memberOf = [],
			isLoggedIn,
			fetchingUser
		}
	) => {
		const { resourceId, resourceType } = match.params

		useEffect(() => {
			getResource(resourceType, resourceId, setCenterMap)
		}, [ getResource, resourceId, resourceType, setCenterMap])

		const resourceTypeData = getResourceTypeData(resourceTypes, resourceType);
		const resourceColor = '#' + (resourceTypeData.displayColor || '22A73D');

		const { location } = resource
		const defaultLocation = resource.location
			? [location.latitude, location.longitude]
			: []

		const hideMap = shouldHideMap(resourceType)

		return (
			<Fragment>
				<SiteMap
					display={!hideMap}
					centerMap={centerMap}
					setCenterMap={setCenterMap}
					center={defaultLocation}
					sites={resource.sites && [...resource.sites, ...resource.workingGroups, ...resource.events]}
					resourceType={resourceType}
				/>
				<Container
					hideMap={hideMap}
					content={
						<Fragment>
							<Loader display={isLoading || fetchingUser}/>
							{resource.metadata && !isLoading && (
								<ResourceContent
									hideMap={hideMap}
									resource={resource}
									notifications={notifications}
									setCenterMap={setCenterMap}
									resourceTypes={resourceTypes}
									resourceColor={resourceColor}
									resourceType={resourceType}
									subscribeResource={subscribeResource}
									leaveResource={leaveResource}
									memberOf={memberOf}
									isLoggedIn={isLoggedIn}
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
	resourceType: PropTypes.string,
	fetchingUser: PropTypes.bool

}
