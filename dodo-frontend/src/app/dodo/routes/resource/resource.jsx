import React, { useEffect } from "react"
import PropTypes from "prop-types"
import { Container } from "app/components/resources"
import { SiteMap, Loader } from "app/components"
import { ResourceContent } from './resource-content'
import { getResourceTypeData, shouldHideMap } from './services'
import { NotFound } from '../error';

export const Resource =
	(
		{
			match,
			getResource,
			getNotifications,
			resource,
			notifications,
			error,
			hasFailed,
			isLoading,
			isLoadingNotifications,
			centerMap,
			setCenterMap,
			resourceTypes = [],
			subscribeResource,
			leaveResource,
			isLoggedIn,
			isMember,
			fetchingUser
		}
	) => {
		const { slug, resourceType } = match.params

		useEffect(() => {
			getResource(resourceType, slug, setCenterMap)
		}, [ getResource, slug, resourceType, setCenterMap])

		useEffect(() => {
			getNotifications(resourceType, slug)
		}, [ getNotifications, slug, resourceType])

		const resourceTypeData = getResourceTypeData(resourceTypes, resourceType);
		const resourceColor = '#' + (resourceTypeData.displayColor || '22A73D');

		const { location } = resource
		const defaultLocation = resource.location
			? [location.latitude, location.longitude]
			: []

		const hideMap = shouldHideMap(resourceType)

		return (
			<>
				{!hasFailed
					? <>
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
								<>
									<Loader display={isLoading || fetchingUser}/>
									{resource.metadata && !isLoading && (
										<ResourceContent
											hideMap={hideMap}
											resource={resource}
											notifications={notifications}
											getNotifications={() => getNotifications(resourceType, slug, notifications.nextPageToLoad)}
											isLoadingNotifications={isLoadingNotifications}
											setCenterMap={setCenterMap}
											resourceTypes={resourceTypes}
											resourceColor={resourceColor}
											resourceType={resourceType}
											subscribeResource={subscribeResource}
											leaveResource={leaveResource}
											isLoggedIn={isLoggedIn}
											isMember={isMember}
										/>
									)}
								</>
							}
						/>
					</>
					: <>
						{error.status === 404 && (
							<NotFound/>
						)}
					</>
				}
			</>
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
