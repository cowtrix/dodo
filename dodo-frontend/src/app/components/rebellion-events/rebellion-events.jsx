import React from "react";

import { SiteMap } from "app/components/site-map";
import { Button } from "app/components/button";
import styles from "./rebellion-events.module.scss";

const getDescription = event => {
	const description = event.PublicDescription || "";
	return description.slice(0, 180);
};

export const RebellionEvents = ({ events }) => {
	if (!events) {
		return <div>Loading</div>;
	}

	return (
		<div className={styles.RebellionEvents}>
			<h3>Upcoming Events</h3>
			<SiteMap sites={events} />
			<div className={styles.events}>
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

			<Button
				to={"/sites/search"}
				variant="outline"
				type="button"
				style={{ width: "100%", textAlign: "center" }}
			>
				View All Events ({events.length})
			</Button>
		</div>
	);
};
