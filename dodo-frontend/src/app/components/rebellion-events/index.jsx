import React from "react";

import SiteMap from "app/components/site-map";
import styles from "./rebellion-events.module.scss";

const getDescription = event => {
	const description = event.PublicDescription || "";
	return description.slice(0, 180);
};

const RebellionEvents = ({ events }) => {
	if (!events) {
		return <div>Loading</div>;
	}

	return (
		<div className={styles.events}>
			<h3>Upcoming Events</h3>
			<SiteMap sites={events} />
			{events.map(event => (
				<div key={event.GUID} className={styles.event}>
					<strong>{event.Name}</strong>
					<div className={styles.date}>26th March 2020</div>
					<div className={styles.description}>
						{getDescription(event)}
					</div>
				</div>
			))}
		</div>
	);
};

export default RebellionEvents;
