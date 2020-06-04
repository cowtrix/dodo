import React from 'react'
import PropTypes from 'prop-types'
import { Loader, DateLayout, PageTitle } from "app/components"
import { List, Header, Description, SignUpButton, Video } from "app/components/resources"

import styles from './resource-content.module.scss'

const JOIN_US_SITES = "Join us at a protest site"
const COME_TO_EVENT = "Come to an resources"
const VOLUNTEER_NOW = "Volunteer now with a working group"

export const ResourceContent = ({ resource, setCenterMap, resourceTypes}) =>
	<div className={styles.resource}>
		<Header resource={resource} setCenterMap={setCenterMap} />
		<Video videoEmbedURL={resource.videoEmbedURL} />
		<Description description={resource.publicDescription} />
		<SignUpButton />
		<List resources={resource.resources} title={COME_TO_EVENT} resourceTypes={resourceTypes} />
		<List resources={resource.sites} title={JOIN_US_SITES} resourceTypes={resourceTypes} />
		<List resources={resource.workingGroups} title={VOLUNTEER_NOW} resourceTypes={resourceTypes} />
	</div>

ResourceContent.propTypes = {
	resource: PropTypes.object,
	resourceTypes: PropTypes.array,
	setCenterMap: PropTypes.func
}