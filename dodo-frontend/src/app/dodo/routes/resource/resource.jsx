import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { Container } from "app/components/resources"
import { SiteMap, Loader } from "app/components"
import { ResourceContent } from './resource-content'
import { shouldHideMap } from './services'


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
		}, [match])


		const resourceColor =
			resourceTypes.length &&
			'#' + resourceTypes.find(resource => resourceType === resource.value).displayColor || '000000'

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
					sites={resource.sites && [...resource.sites, ...resource.workingGroups]}
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
	fetchingUser: PropTypes.bool
}
