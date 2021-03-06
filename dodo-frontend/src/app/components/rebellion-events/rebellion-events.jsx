import React from "react"
import { Link } from "react-router-dom"

import { SiteMap } from "app/components/site-map"
import { Button } from "app/components/button"
import styles from "./rebellion-events.module.scss"

const getDescription = event => {
	const description = event.publicDescription || ""
	return description.slice(0, 180)
}

export const RebellionEvents = ({ events }) => {
	if (!events) {
		return <div>Loading</div>
	}

	return (
		<div className={styles.RebellionEvents}>
			<h3>Upcoming Events</h3>
			<SiteMap sites={events} className={styles.miniMap} />
			<div className={styles.events}>
				{events.map(event => (
					<div key={event.guid} className={styles.event}>
						<Link to={`/site/${event.slug}`}>
							<strong>{event.name}</strong>
						</Link>
						<div className={styles.date}>26th March 2020</div>
						<div className={styles.description}>
							{getDescription(event)}
						</div>
					</div>
				))}
			</div>

			<Button
				to={"/sites/search"}
				variant="outlinePrimary"
				type="button"
				style={{ width: "100%", textAlign: "center" }}
			>
				View All Events ({events.length})
			</Button>
		</div>
	)
}
