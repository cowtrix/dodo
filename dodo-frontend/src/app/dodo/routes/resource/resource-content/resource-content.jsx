import React from 'react'
import PropTypes from 'prop-types'
import { useHistory } from 'react-router-dom'
import { Video, ExpandableList } from "app/components"
import { Header, Description, SignUpButton, ParentLink, Updates, Role } from "app/components/resources"

import { ADMIN_PERMISSIONS } from 'app/constants'
import { isSubscribedToResource } from './services'
import { VOLUNTEER_NOW, JOIN_US_SITES, COME_TO_EVENT } from './constants'

import styles from './resource-content.module.scss'
import { REGISTER_ROUTE } from '../../user-routes/register'

export const ResourceContent =
	({ resource, setCenterMap, resourceTypes, resourceColor, resourceType, subscribeResource, memberOf, isLoggedIn, notifications, getNotifications, isLoadingNotifications, hideMap }) => {
		const { push } = useHistory()

		const isSubscribed = isSubscribedToResource(memberOf, resource.guid)
		const subscribe = () => subscribeResource(resourceType, resource.guid, 'join')
		const unSubscribe = () => subscribeResource(resourceType, resource.guid, 'leave')
		const apply = (body) => subscribeResource(resourceType, resource.guid, 'apply', { content: body })

		const shouldDisplayNotifications = resourceType !== 'role'

		return (
			<div className={styles.resource}>
				<Header resource={resource} setCenterMap={setCenterMap} resourceColor={resourceColor} hideMap={hideMap}/>
				<ParentLink parent={resource.parent}/>
				<Video videoEmbedURL={resource.videoEmbedURL}/>
				<div className={styles.descriptionContainer}>
					<Description description={resource.publicDescription}/>
					{shouldDisplayNotifications &&
					<Updates notifications={notifications} loadMore={getNotifications} isLoadingMore={isLoadingNotifications}/>
					}
				</div>
				<Role
					applicantQuestion={resource.applicantQuestion}
					resourceColor={resourceColor}
					applyForRole={apply}
					isLoggedIn={isLoggedIn}
					hasApplied={resource.metadata.applied}
				/>
				<SignUpButton
					disable={(isLoggedIn && resource.applicantQuestion) || resource.metadata.permission === ADMIN_PERMISSIONS}
					resourceColor={resourceColor}
					isLoggedIn={isLoggedIn}
					isSubscribed={isSubscribed}
					onClick={!isLoggedIn ? () => push(REGISTER_ROUTE) : !isSubscribed ? subscribe : unSubscribe}
				/>
				<ExpandableList resources={resource.events} title={COME_TO_EVENT} resourceTypes={resourceTypes}/>
				<ExpandableList resources={resource.sites} title={JOIN_US_SITES} resourceTypes={resourceTypes}/>
				<ExpandableList resources={resource.workingGroups} title={VOLUNTEER_NOW} resourceTypes={resourceTypes}/>
				<ExpandableList resources={resource.roles} title={VOLUNTEER_NOW} resourceTypes={resourceTypes}/>
			</div>
		)
	}


ResourceContent.propTypes = {
	subscribeResource: PropTypes.func,
	resource: PropTypes.object,
	notifications: PropTypes.object,
	resourceColor: PropTypes.string,
	resourceTypes: PropTypes.array,
	setCenterMap: PropTypes.func,
	hideMap: PropTypes.bool
}
