import React from 'react'
import PropTypes from 'prop-types'
import { ExpandableList, Video } from "app/components"
import { Description, Header, ParentLink, SignUpButton, Updates } from "app/components/resources"

import styles from './resource-content.module.scss'

const fakeNotificationData = {"notifications":[{"timestamp":"2020-06-25T22:39:21.0773977Z","source":"London Rebellion","message":"A new Site was created: \"Central Occupation\"","guid":"43f5f136-c687-4b2c-8067-69b040cef19a","canDelete":false},{"timestamp":"2020-06-25T22:39:21.224019Z","source":"London Rebellion","message":"A new Event was created: \"Protest For Nature\"","guid":"f146c7f7-0fed-4bfb-84d7-4b136116c0af","canDelete":false},{"timestamp":"2020-06-25T22:39:21.3123595Z","source":"London Rebellion","message":"A new Event was created: \"Youth March\"","guid":"62d1499e-3495-4966-9399-4578bc299425","canDelete":false},{"timestamp":"2020-06-25T22:39:21.4084882Z","source":"London Rebellion","message":"A new Site was created: \"Second Occupation\"","guid":"e2603095-260c-428e-b7ce-ddaa6a720e60","canDelete":false},{"timestamp":"2020-06-25T22:39:21.5325588Z","source":"London Rebellion","message":"A new Event was created: \"Activism Workshop\"","guid":"40deb1ba-54f1-4fab-8112-659535989472","canDelete":false},{"timestamp":"2020-06-25T22:39:21.6853452Z","source":"London Rebellion","message":"A new Working Group was created: \"Action Support\"","guid":"85410beb-1283-4e56-adeb-38b87bbe64e0","canDelete":false},{"timestamp":"2020-06-25T22:39:22.6764966Z","source":"London Rebellion","message":"A new Working Group was created: \"Worldbuilding & Production\"","guid":"92f63fa6-20bb-49fb-953c-60c2fe0d17ea","canDelete":false},{"timestamp":"2020-06-25T22:39:23.8091545Z","source":"London Rebellion","message":"A new Working Group was created: \"Media & Messaging\"","guid":"5252e48e-5280-45b4-ac80-55b698ef3d46","canDelete":false},{"timestamp":"2020-06-25T22:39:24.4370681Z","source":"London Rebellion","message":"A new Working Group was created: \"Movement Support\"","guid":"e06c48ad-993c-4286-ad72-941b7d587d79","canDelete":false}],"totalCount":9,"chunkSize":25}

const JOIN_US_SITES = "Join us at a protest site"
const COME_TO_EVENT = "Come to an resources"
const VOLUNTEER_NOW = "Volunteer now with a working group"

export const ResourceContent = ({ resource, setCenterMap, resourceTypes, resourceColor}) =>
	<div className={styles.resource}>
		<Header resource={resource} setCenterMap={setCenterMap} resourceColor={resourceColor} />
		<ParentLink parent={resource.parent}/>
		<Video videoEmbedURL={resource.videoEmbedURL} />
		<div className={styles.descriptionContainer}>
		<Description description={resource.publicDescription} />
		<Updates notifications={fakeNotificationData.notifications}/>
		</div>
		<SignUpButton resourceColor={resourceColor} />
		<ExpandableList resources={resource.resources} title={COME_TO_EVENT} resourceTypes={resourceTypes} />
		<ExpandableList resources={resource.sites} title={JOIN_US_SITES} resourceTypes={resourceTypes} />
		<ExpandableList resources={resource.workingGroups} title={VOLUNTEER_NOW} resourceTypes={resourceTypes} />
	</div>

ResourceContent.propTypes = {
	resource: PropTypes.object,
	resourceColor: PropTypes.string,
	resourceTypes: PropTypes.array,
	setCenterMap: PropTypes.func
}
