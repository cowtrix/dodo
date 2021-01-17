import React from 'react'
import PropTypes from 'prop-types'
import { Icon } from '../../../../components/icon'
import { useHistory, useLocation } from 'react-router-dom'
import { Video, ExpandableList } from "app/components"
import { Header, Description, SignUpButton, ParentLink, Updates, Role, ArrestRisk, Facilities } from "app/components/resources"

import { ADMIN_PERMISSIONS } from 'app/constants'
import { VOLUNTEER_NOW, JOIN_US_SITES, COME_TO_EVENT } from './constants'

import styles from './resource-content.module.scss'
import { REGISTER_ROUTE } from '../../user-routes/register'
import { addReturnPathToRoute } from '../../../../domain/services/services'

export const ResourceContent =
	({ resource, setCenterMap, resourceTypes, resourceColor, resourceType, subscribeResource, isEmailConfirmed, isLoggedIn, isMember, notifications, getNotifications, isLoadingNotifications, hideMap, resendVerificationEmail }) => {
		const { push } = useHistory()
		const location = useLocation();

		const subscribe = () => subscribeResource(resourceType, resource.guid, 'join')
		const unSubscribe = () => subscribeResource(resourceType, resource.guid, 'leave')

		const shouldDisplayNotifications = resourceType !== 'role' && !!notifications?.notifications?.length
		const shouldShowAdmin = resource.metadata.permission === 'owner' || resource.metadata.permission === 'admin'

		if (location.pathname.slice(-5) === '/join') {
			subscribe();
			push(location.pathname.slice(0, -5));
		}

		return (
			<div className={styles.resource}>
				<Header resource={resource} setCenterMap={setCenterMap} resourceColor={resourceColor} hideMap={hideMap} />
				{resource.arrestRisk && (
					<ArrestRisk level={resource.arrestRisk.toLowerCase()} />
				)}
				<ParentLink parent={resource.parent} />
				<Video videoEmbedURL={resource.videoEmbedURL} />
				{shouldShowAdmin ? <a href={'../edit' + location.pathname} className={styles.adminbutton}><h2>EDIT  <Icon icon="edit" size="1x" /></h2></a> : null}
				<div className={styles.descriptionContainer}>
					<Description description={resource.publicDescription} />
					{(shouldDisplayNotifications || resource.facilities) && (
						<div className={styles.panelsContainer}>
							{shouldDisplayNotifications &&
								<Updates notifications={notifications} loadMore={getNotifications} isLoadingMore={isLoadingNotifications} />
							}
							<Facilities facilities={resource.facilities} />
						</div>
					)}
				</div>
				{resourceType === 'role' ?
					<Role
						resource={resource}
						isEmailConfirmed={isEmailConfirmed}
						isLoggedIn={isLoggedIn}
						hasApplied={resource.metadata.applied}
						resendVerificationEmail={resendVerificationEmail}
					/>
					: null
				}
				<SignUpButton
					disable={resource.metadata.permission === ADMIN_PERMISSIONS}
					resourceColor={resourceColor}
					isLoggedIn={isLoggedIn}
					isSubscribed={isMember}
					onClick={
						!isLoggedIn
							? () => push(addReturnPathToRoute(REGISTER_ROUTE, location.pathname + '/join'))
							: !isMember ? subscribe : unSubscribe}
				/>
				<ExpandableList resources={resource.events} title={COME_TO_EVENT} resourceTypes={resourceTypes} />
				<ExpandableList resources={resource.sites} title={JOIN_US_SITES} resourceTypes={resourceTypes} />
				<ExpandableList resources={resource.workingGroups} title={VOLUNTEER_NOW} resourceTypes={resourceTypes} />
				<ExpandableList resources={resource.roles} title={VOLUNTEER_NOW} resourceTypes={resourceTypes} />
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
