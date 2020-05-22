import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"

import { Container } from "app/components/events"
import { SiteMap, Loader, DateLayout, PageTitle } from "app/components"
import styles from "./event.module.scss"
import { List } from "app/components/events"

export const Event = ({ match, getEvent, event, isLoading }) => {
	const { eventId, eventType } = match.params

	useEffect(() => {
		getEvent(eventType, eventId)
	}, [match])

	const { location } = event
	const defaultLocation = event.location
		? [location.latitude, location.longitude]
		: []

	return (
		<Fragment>
			<SiteMap
				defaultLocation={defaultLocation}
				sites={event.sites && [...event.sites, ...event.workingGroups]}
			/>
			<Container
				content={
					<Fragment>
						<Loader display={isLoading} />
						{event.metadata && !isLoading && (
							<div className={styles.event}>
								<DateLayout
									startDate={
										event.startDate
											? new Date(event.startDate)
											: null
									}
									endDate={
										event.endDate
											? new Date(event.endDate)
											: null
									}
									title={
										<PageTitle
											title={event.name}
											subTitle={event.metadata.type}
										/>
									}
								>
									<div className={styles.detail}>
										{event.publicDescription}
									</div>
								</DateLayout>
								<List events={event.sites} />
								<List events={event.workingGroups} />
							</div>
						)}
					</Fragment>
				}
			/>
		</Fragment>
	)
}

Event.propTypes = {
	match: PropTypes.object.isRequired,
	getEvent: PropTypes.func,
	event: PropTypes.object
}
