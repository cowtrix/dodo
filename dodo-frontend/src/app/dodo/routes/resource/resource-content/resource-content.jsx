import React from 'react'
import PropTypes from 'prop-types'
import { Video, ExpandableList } from "app/components"
import { Header, Description, SignUpButton, ParentLink } from "app/components/resources"

import styles from './resource-content.module.scss'


const JOIN_US_SITES = "Join us at a protest site"
const COME_TO_EVENT = "Come to an resources"
const VOLUNTEER_NOW = "Volunteer now with a working group"

export const ResourceContent =
	({ resource, setCenterMap, resourceTypes, resourceColor, resourceType, joinResource}) =>
	<div className={styles.resource}>
		<Header resource={resource} setCenterMap={setCenterMap} resourceColor={resourceColor} />
		<ParentLink parent={resource.parent}/>
		<Video videoEmbedURL={resource.videoEmbedURL} />
		<Description description={resource.publicDescription} />
		<SignUpButton resourceColor={resourceColor} onClick={() => joinResource(resourceType, resource.guid)} />
		<ExpandableList resources={resource.resources} title={COME_TO_EVENT} resourceTypes={resourceTypes} />
		<ExpandableList resources={resource.sites} title={JOIN_US_SITES} resourceTypes={resourceTypes} />
		<ExpandableList resources={resource.workingGroups} title={VOLUNTEER_NOW} resourceTypes={resourceTypes} />
	</div>

ResourceContent.propTypes = {
	joinResource: PropTypes.func,
	resource: PropTypes.object,
	resourceColor: PropTypes.string,
	resourceTypes: PropTypes.array,
	setCenterMap: PropTypes.func
}